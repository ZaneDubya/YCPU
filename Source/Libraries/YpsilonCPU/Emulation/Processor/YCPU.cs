using System.Threading;

namespace Ypsilon.Emulation.Processor
{
    /// <summary>
    /// A processor defined by the YCPU Specification.
    /// </summary>
    public partial class YCPU
    {
        public const int ClockRateHz = 10240; // 10khz

        /// <summary>
        /// The hardware bus, which hosts all hardware devices.
        /// </summary>
        public YBUS BUS => m_Bus;

        public long Cycles => m_Cycles;

        /// <summary>
        /// Is this YCPU currently executing?
        /// </summary>
        public bool Running => m_Running;

        /// <summary>
        /// Initializes a new YCPU.
        /// </summary>
        public YCPU()
        {
            m_Bus = new YBUS(this);
            m_RTC = new YRTC();

            InitializeOpcodes();
            InitializeMemory();
            PS = 0x0000;
        }

        /// <summary>
        /// Executes a set number of cycles, or infinite cycles.
        /// </summary>
        /// <param name="cyclecount">How many cycles to run before stopping. Default value of -1 will run until Pause() is called.</param>
        public void Run(int cyclecount = -1)
        {
            // Count Cycles:
            long cycles_start = m_Cycles;
            long cycles_target = (cyclecount == -1) ? long.MaxValue : cyclecount + m_Cycles;

            // Run the processor for cyclecount Cycles:
            m_Running = true;
            while (m_Running)
            {
                try
                {
                    ushort word = ReadMemInt16(PC, SegmentIndex.CS);
                    PC += 2;

                    // Execute Memory[PC] and increment the cycle counter:
                    YCPUInstruction opcode = Opcodes[word & 0x00FF];
                    opcode.Opcode(word);
                    m_Cycles += opcode.Cycles;

                    // Check for hardware interrupt:
                    if (PS_H && !PS_Q && m_Bus.IsIRQ)
                        Interrupt_HWI();

                    // Check for RTC interrupt:
                    if (m_RTC.IsEnabled && m_RTC.IRQ(m_Cycles))
                        Interrupt_Clock();

                    // Check to see if we've exceeded our cycle target:
                    if (m_Cycles >= cycles_target)
                        m_Pausing = true;
                    if (m_Pausing)
                    {
                        m_Running = false;
                        m_Pausing = false;
                    }
                }
                catch (SegFaultException e)
                {
                    Interrupt_SegFault(e.SegmentType, 0xffff, PC);
                }
            }
        }

        /// <summary>
        /// Executes one instruction and returns.
        /// </summary>
        public void RunOneInstruction()
        {
            try
            {
                ushort word = ReadMemInt16(PC, SegmentIndex.CS);
                PC += 2;

                // Check for hardware interrupt:
                if (PS_H && !PS_Q && m_Bus.IsIRQ)
                    Interrupt_HWI();

                // Check for RTC interrupt:
                if (m_RTC.IsEnabled && m_RTC.IRQ(m_Cycles))
                    Interrupt_Clock();

                // Execute Memory[PC]
                YCPUInstruction opcode = Opcodes[word & 0x00FF];
                opcode.Opcode(word);
                // Increment the Cycle counter.
                m_Cycles += opcode.Cycles;
            }
            catch (SegFaultException e)
            {
                Interrupt_SegFault(e.SegmentType, 0xffff, PC);
            }
        }

        /// <summary>
        /// Pauses a currently executing YCPU.
        /// </summary>
        public void Pause()
        {
            if (!m_Running)
                return;
            m_Pausing = true;
            while (m_Pausing)
            {
                // wait until the cpu has stopped running, then return.
                // we wait 1ms between each try so we don't lock this variable.
                Thread.Sleep(1);
            }
        }

        #region General Purpose Registers
        public enum RegGeneral
        {
            R0, R1, R2, R3, R4, R5, R6, R7,
            Count,
            None
        }
        private ushort[] R = new ushort[(int)RegGeneral.Count];
        public ushort R0 => R[0];
        public ushort R1 => R[1];
        public ushort R2 => R[2];
        public ushort R3 => R[3];
        public ushort R4 => R[4];
        public ushort R5 => R[5];
        public ushort R6 => R[6];
        public ushort R7 => R[7];

        #endregion

        #region Control Registers

        private enum RegControl
        {
            FL = 0,
            PC = 1,
            PS = 2,
            USP = 6,
            SSP = 7
        }

        private ushort ReadControlRegister(ushort operand, RegControl index)
        {
            switch (index)
            {
                case RegControl.FL:
                    return m_FL;
                case RegControl.PC:
                    return m_PC;
                case RegControl.PS:
                    if (PS_S)
                        return m_PS;
                    Interrupt_UnPrivFault(operand);
                    return 0;
                case RegControl.USP:
                    return m_USP;
                case RegControl.SSP:
                    if (PS_S)
                        return m_SSP;
                    return m_USP;
                default:
                    Interrupt_UndefFault(operand);
                    return 0;
            }
        }

        private void WriteControlRegister(ushort operand, RegControl index, ushort value)
        {
            switch (index)
            {
                case RegControl.FL:
                    FL = value;
                    break;

                case RegControl.PC:
                    PC = value;
                    break;

                case RegControl.PS:
                    if (PS_S)
                        PS = value;
                    else
                        Interrupt_UnPrivFault(operand);
                    break;

                case RegControl.USP:
                    USP = value;
                    break;

                case RegControl.SSP:
                    if (PS_S)
                        SSP = value;
                    else
                        USP = value;
                    break;

                default:
                    Interrupt_UndefFault(operand);
                    break;
            }
        }

        #region FL
        private ushort m_FL;
        private const ushort c_FL_N = 0x8000, c_FL_Z = 0x4000, c_FL_C = 0x2000, c_FL_V = 0x1000;
        public ushort FL
        {
            get { return m_FL; }
            set { m_FL = value; }
        }
        public ushort Carry => ((m_FL & c_FL_C) != 0) ? (ushort)1 : (ushort)0;

        public bool FL_N
        {
            get
            {
                return ((m_FL & c_FL_N) != 0);
            }
            private set
            {
                if (value == false)
                {
                    m_FL &= unchecked((ushort)~c_FL_N);
                }
                else if (value)
                {
                    m_FL |= c_FL_N;
                }
            }
        }
        public bool FL_Z
        {
            get
            {
                return ((m_FL & c_FL_Z) != 0);
            }
            private set
            {
                if (value == false)
                {
                    m_FL &= unchecked((ushort)~c_FL_Z);
                }
                else if (value)
                {
                    m_FL |= c_FL_Z;
                }
            }
        }
        public bool FL_C
        {
            get
            {
                return ((m_FL & c_FL_C) != 0);
            }
            private set
            {
                if (value == false)
                {
                    m_FL &= unchecked((ushort)~c_FL_C);
                }
                else if (value)
                {
                    m_FL |= c_FL_C;
                }
            }
        }
        public bool FL_V
        {
            get
            {
                return ((m_FL & c_FL_V) != 0);
            }
            private set
            {
                if (value == false)
                {
                    m_FL &= unchecked((ushort)~c_FL_V);
                }
                else if (value)
                {
                    m_FL |= c_FL_V;
                }
            }
        }
        #endregion
        #region PS
        private ushort m_PS;
        private const ushort c_PS_S = 0x8000, c_PS_M = 0x4000, c_PS_H = 0x2000, c_PS_I = 0x1000;
        private const ushort c_PS_Q = 0x0800, c_PS_U = 0x0400, c_PS_V = 0x0200, c_PS_W = 0x0100;
        private bool m_PS_S;
        public ushort PS
        {
            private set
            {
                PS_H = (value & 0x2000) != 0;
                PS_M = (value & 0x4000) != 0;
                PS_S = (value & 0x8000) != 0;
                m_PS = value;
            }
            get
            {
                return m_PS;
            }
        }

        /// <summary>
        /// [S]upervisor Mode enabled.
        /// </summary>
        public bool PS_S
        {
            get
            {
                return ((m_PS & c_PS_S) != 0);
            }
            private set
            {
                if (value == false)
                {
                    m_PS &= unchecked((ushort)~c_PS_S);
                }
                else if (value)
                {
                    m_PS |= c_PS_S;
                }
                m_PS_S = value;
            }
        }

        /// <summary>
        /// [M]emory segmenting hardware enabled.
        /// </summary>
        public bool PS_M
        {
            get
            {
                return ((m_PS & c_PS_M) != 0);
            }
            set
            {
                if (value != PS_M)
                {
                    if (value == false)
                    {
                        m_PS &= unchecked((ushort)~c_PS_M);
                    }
                    else if (value)
                    {
                        m_PS |= c_PS_M;
                    }
                }
            }
        }

        /// <summary>
        /// [H]ardware Interrupts enabled.
        /// </summary>
        public bool PS_H
        {
            get
            {
                return ((m_PS & c_PS_H) != 0);
            }
            private set
            {
                if (value == false)
                {
                    m_PS &= unchecked((ushort)~c_PS_H);
                }
                else if (value)
                {
                    m_PS |= c_PS_H;
                }
            }
        }

        /// <summary>
        /// Processor handling [I]nterrupt.
        /// </summary>
        public bool PS_I
        {
            get
            {
                return ((m_PS & c_PS_I) != 0);
            }
            private set
            {
                if (value == false)
                {
                    m_PS &= unchecked((ushort)~c_PS_I);
                }
                else if (value)
                {
                    m_PS |= c_PS_I;
                }
            }
        }

        /// <summary>
        /// Interrupt Re[Q]uest in process, blocks hardware interrupts.
        /// </summary>
        public bool PS_Q
        {
            get
            {
                return ((m_PS & c_PS_Q) != 0);
            }
            private set
            {
                if (value == false)
                {
                    m_PS &= unchecked((ushort)~c_PS_Q);
                }
                else if (value)
                {
                    m_PS |= c_PS_Q;
                }
            }
        }

        /// <summary>
        /// Fault in [U]ser mode.
        /// </summary>
        public bool PS_U
        {
            get
            {
                return ((m_PS & c_PS_U) != 0);
            }
            private set
            {
                if (value == false)
                {
                    m_PS &= unchecked((ushort)~c_PS_U);
                }
                else if (value)
                {
                    m_PS |= c_PS_U;
                }
            }
        }

        /// <summary>
        /// Segment Fault bit 0.
        /// </summary>
        public bool PS_V
        {
            get
            {
                return ((m_PS & c_PS_V) != 0);
            }
            private set
            {
                if (value == false)
                {
                    m_PS &= unchecked((ushort)~c_PS_V);
                }
                else if (value)
                {
                    m_PS |= c_PS_V;
                }
            }
        }

        /// <summary>
        /// Segment Fault bit 1.
        /// </summary>
        public bool PS_W
        {
            get
            {
                return ((m_PS & c_PS_W) != 0);
            }
            private set
            {
                if (value == false)
                {
                    m_PS &= unchecked((ushort)~c_PS_W);
                }
                else if (value)
                {
                    m_PS |= c_PS_W;
                }
            }
        }
        #endregion
        #region PC
        private ushort m_PC;
        public ushort PC
        {
            get { return m_PC; }
            set { m_PC = value; }
        }
        #endregion
        #region SP
        public ushort SP
        {
            get
            {
                return (PS_S) ? SSP : USP;
            }
            set
            {
                if (PS_S)
                    SSP = value;
                else
                    USP = value;
            }
        }
        #endregion
        #region USP
        private ushort m_USP;
        public ushort USP
        {
            get { return m_USP; }
            set { m_USP = value; }
        }
        #endregion
        #region SSP
        private ushort m_SSP;
        public ushort SSP
        {
            get { return m_SSP; }
            set { m_SSP = value; }
        }
        #endregion
        #endregion

        #region Stack
        private void StackPush(ushort operand, ushort value)
        {
            SP -= 2;
            try
            {
                WriteMemInt16(SP, value, SegmentIndex.SS);
            }
            catch (SegFaultException e)
            {
                Interrupt_StackFault(operand, e.Address);
            }
        }

        private ushort StackPop(ushort operand)
        {
            try
            {
                ushort value = ReadMemInt16(SP, SegmentIndex.SS);
                SP += 2;
                return value;
            }
            catch (SegFaultException e)
            {
                Interrupt_StackFault(operand, e.Address);
                return 0;
            }
        }
        #endregion

        private YRTC m_RTC;
        private YBUS m_Bus;

        private long m_Cycles;

        private bool m_Running;
        private bool m_Pausing;
    }
}
