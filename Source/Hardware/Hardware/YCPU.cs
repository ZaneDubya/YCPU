using System;

namespace Ypsilon.Hardware
{
    /// <summary>
    /// A processor defined by the YCPU Specification.
    /// </summary>
    public partial class YCPU
    {
        /// <summary>
        /// The hardware bus, which hosts all hardware devices.
        /// </summary>
        public YBUS BUS
        {
            get { return m_Bus; }
        }

        public long Cycles
        {
            get { return m_Cycles; }
        }
        
        /// <summary>
        /// Is this YCPU currently executing?
        /// </summary>
        public bool Running
        {
            get { return m_Running; }
        }

        /// <summary>
        /// Initializes a new YCPU.
        /// </summary>
        public YCPU()
        {
            m_Bus = new YBUS(this);
            m_RTC = new YRTC();

            InitializeOpcodes();
            InitializeMemory();
            MMU_Disable();
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
                ushort word = ReadMemInt16(PC, true);
                if (!m_ExecuteFail)
                {
                    PC += 2;
                    
                    // Check for hardware interrupt:
                    if (PS_I && !PS_Q && m_Bus.IRQ)
                        Interrupt_HWI();

                    // Check for RTC interrupt:
                    if (m_RTC.IsEnabled && m_RTC.IRQ(m_Cycles))
                        Interrupt_Clock();
                    
                    // Execute Memory[PC] and increment the cycle counter:
                    YCPUInstruction opcode = Opcodes[word & 0x00FF];
                    opcode.Opcode(word);
                    m_Cycles += opcode.Cycles;

                    // Check to see if we've exceeded our cycle target:
                    if (m_Cycles >= cycles_target)
                        m_Pausing = true;
                    if (m_Pausing)
                    {
                        m_Running = false;
                        m_Pausing = false;
                    }
                }
            }
        }

        /// <summary>
        /// Executes one instruction and returns.
        /// </summary>
        public void RunOneInstruction()
        {
            ushort word = ReadMemInt16(PC, true);
            if (!m_ExecuteFail)
            {
                PC += 2;

                // Check for hardware interrupt:
                if (PS_I && !PS_Q && m_Bus.IRQ)
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
        }

        /// <summary>
        /// Pauses a currently executing YCPU.
        /// </summary>
        public void Pause()
        {
            if (!m_Running)
                return;
            m_Pausing = true;
            while (m_Pausing == true)
            {
                // wait until the cpu has stopped running, then return.
                // we wait 1ms between each try so we don't lock this variable.
                System.Threading.Thread.Sleep(1);
            }
        }

        #region General Purpose Registers
        public enum RegGPIndex
        {
            R0, R1, R2, R3, R4, R5, R6, R7,
            Count,
            None
        }
        private ushort[] R = new ushort[(int)RegGPIndex.Count];
        public ushort R0 { get { return R[0]; } }
        public ushort R1 { get { return R[1]; } }
        public ushort R2 { get { return R[2]; } }
        public ushort R3 { get { return R[3]; } }
        public ushort R4 { get { return R[4]; } }
        public ushort R5 { get { return R[5]; } }
        public ushort R6 { get { return R[6]; } }
        public ushort R7 { get { return R[7]; } }
        #endregion

        #region Status Registers
        enum RegSPIndex
        {
            FL, PC, PS, P2, II, IA, USP, SSP,
            Count
        }

        ushort ReadStatusRegister(RegSPIndex index)
        {
            switch (index)
            {
                case RegSPIndex.FL:
                    return m_FL;
                case RegSPIndex.PC:
                    return m_PC;
                case RegSPIndex.PS:
                    if (PS_S)
                        return m_PS;
                    else
                    {
                        Interrupt_UnPrivOpcode();
                        return 0;
                    }
                case RegSPIndex.P2:
                    if (PS_S)
                        return m_P2;
                    else
                    {
                        Interrupt_UnPrivOpcode();
                        return 0;
                    }
                case RegSPIndex.II:
                    if (PS_S)
                        return m_II;
                    else
                    {
                        Interrupt_UnPrivOpcode();
                        return 0;
                    }
                case RegSPIndex.IA:
                    if (PS_S)
                        return m_IA;
                    else
                    {
                        Interrupt_UnPrivOpcode();
                        return 0;
                    }
                case RegSPIndex.USP:
                    return m_USP;
                case RegSPIndex.SSP:
                    if (PS_S)
                        return m_SSP;
                    else
                        return m_USP;
                default:
                    throw new Exception();
            }
        }

        void WriteStatusRegister(RegSPIndex index, ushort value)
        {
            switch (index)
            {
                case RegSPIndex.FL:
                    m_FL = value;
                    break;

                case RegSPIndex.PC:
                    m_PC = value;
                    break;

                case RegSPIndex.PS:
                    if (PS_S)
                        m_PS = value;
                    else
                        Interrupt_UnPrivOpcode();
                    break;

                case RegSPIndex.P2:
                    if (PS_S)
                        m_P2 = value;
                    else
                        Interrupt_UnPrivOpcode();
                    break;

                case RegSPIndex.II:
                    if (PS_S)
                        m_II = value;
                    else
                        Interrupt_UnPrivOpcode();
                    break;

                case RegSPIndex.IA:
                    if (PS_S)
                        m_IA = value;
                    else
                        Interrupt_UnPrivOpcode();
                    break;

                case RegSPIndex.USP:
                    m_USP = value;
                    break;

                case RegSPIndex.SSP:
                    if (PS_S)
                        m_SSP = value;
                    else
                        m_USP = value;
                    break;

                default:
                    throw new Exception();
            }
        }

        #region FL
        private ushort m_FL = 0x0000;
        private const ushort c_FL_N = 0x8000, c_FL_Z = 0x4000, c_FL_C = 0x2000, c_FL_V = 0x1000;
        public ushort FL
        {
            get { return m_FL; }
            set { m_FL = value; }
        }
        public ushort Carry
        {
            get { return ((m_FL & c_FL_C) != 0) ? (ushort)1 : (ushort)0; }
        }
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
                else if (value == true)
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
                else if (value == true)
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
                else if (value == true)
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
                else if (value == true)
                {
                    m_FL |= c_FL_V;
                }
            }
        }
        #endregion
        #region IA
        private ushort m_IA = 0x0000;
        public ushort IA
        {
            get { return m_IA; }
            set { m_IA = value; }
        }
        #endregion
        #region II
        private ushort m_II = 0x0000;
        public ushort II
        {
            get { return m_II; }
            set { m_II = value; }
        }
        #endregion
        #region PS
        private ushort m_PS = 0x0000;
        private const ushort c_PS_S = 0x8000, c_PS_M = 0x4000, c_PS_I = 0x2000, c_PS_R = 0x1000;
        private const ushort c_PS_Q = 0x0800, c_PS_U = 0x0400, c_PS_W = 0x0200, c_PS_E = 0x0100;
        private bool m_PS_S = false;
        public ushort PS
        {
            private set
            {
                PS_R = (value & 0x1000) != 0;
                PS_I = (value & 0x2000) != 0;
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
        /// If PS_S is true, then the processor is in Supervisor mode.
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
                    Mode_UserMode();
                }
                else if (value == true)
                {
                    m_PS |= c_PS_S;
                    Mode_SupervisorMode();
                }
                m_PS_S = value;
            }
        }

        /// <summary>
        /// If PS_M is true, then the MMU is active.
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
                        MMU_Disable();
                    }
                    else if (value == true)
                    {
                        m_PS |= c_PS_M;
                        MMU_Enable();
                    }
                }
            }
        }

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
                else if (value == true)
                {
                    m_PS |= c_PS_I;
                }
            }
        }

        public bool PS_R
        {
            get
            {
                return ((m_PS & c_PS_R) != 0);
            }
            set
            {
                if (value == false)
                {
                    m_PS &= unchecked((ushort)~c_PS_R);
                    MMU_PS_R_Updated();
                }
                else if (value == true)
                {
                    m_PS |= c_PS_R;
                    MMU_PS_R_Updated();
                }
            }
        }

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
                else if (value == true)
                {
                    m_PS |= c_PS_Q;
                }
            }
        }

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
                else if (value == true)
                {
                    m_PS |= c_PS_U;
                }
            }
        }

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
                else if (value == true)
                {
                    m_PS |= c_PS_W;
                }
            }
        }

        public bool PS_E
        {
            get
            {
                return ((m_PS & c_PS_E) != 0);
            }
            private set
            {
                if (value == false)
                {
                    m_PS &= unchecked((ushort)~c_PS_E);
                }
                else if (value == true)
                {
                    m_PS |= c_PS_E;
                }
            }
        }
        #endregion
        #region P2
        private ushort m_P2 = 0x0000;
        public ushort P2
        {
            get { return m_P2; }
            set { m_P2 = value; }
        }
        #endregion
        #region PC
        private ushort m_PC = 0x0000;
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
        private ushort m_USP = 0x0000;
        public ushort USP
        {
            get { return m_USP; }
            set { m_USP = value; }
        }
        #endregion
        #region SSP
        private ushort m_SSP = 0x0000;
        public ushort SSP
        {
            get { return m_SSP; }
            set { m_SSP = value; }
        }
        #endregion
        #endregion

        #region Interrupts
        private void Interrupt(ushort interrupt_number)
        {
            interrupt_number &= 0x000F;
            // If this is not a reset interrupt, we should save PS and PC.
            if (interrupt_number != 0x00)
            {
                ushort ps = PS;
                PS_S = true;
                PS_M = false;
                PS_Q = (interrupt_number == 0x0C);
                StackPush(ps);
                // rewind the instruction if this is an error interrupt ($02 - $07)
                if (interrupt_number >= 0x02 && interrupt_number <= 0x07)
                    StackPush((ushort)(PC - SizeOfLastInstruction(PC)));
                else
                    StackPush(PC);
            }
            PC = ReadMemInt16((ushort)(IA + interrupt_number));
            m_Cycles += 31;
        }

        private void ReturnFromInterrupt()
        {
            PC = StackPop();
            PS = StackPop();
            m_PS &= 0xF0FF; // clear Q, U, W, and E.
        }

        public void Interrupt_Reset()
        {
            m_RTC.DisableInterrupt();
            bool r = PS_R;
            PS = 0x8000;
            if (r) PS_R = true;
            IA = 0x0000;
            Interrupt(0x00);
        }

        public void Interrupt_Clock()
        {
            Interrupt(0x01);
        }

        private void Interrupt_DivideByZero()
        {
            Interrupt(0x02);
        }

        private void Interrupt_FPUError()
        {
            Interrupt(0x03);
        }

        private void Interrupt_StackFault()
        {
            Interrupt(0x04);
        }

        private void Interrupt_BankFault(bool execute)
        {
            if (execute)
                m_ExecuteFail = true;   // the processor loop will skip the next instruction (which will be 0x0000).
                                        // then, it will load the next instruction, with PC = InterruptTable[0x05];
            Interrupt(0x05);
        }

        private void Interrupt_UnPrivOpcode()
        {
            Interrupt(0x06);
        }

        private void Interupt_UndefOpcode()
        {
            Interrupt(0x07);
        }

        public void Interrupt_HWI()
        {
            PS_Q = true;
            II = m_Bus.II;
            m_Bus.AcknowledgeIRQ(II);
            Interrupt(0x0C);
        }

        public void Interrupt_BusRefresh()
        {
            Interrupt(0x0D);
        }

        public void Interrupt_DebugQuery()
        {
            Interrupt(0x0E);
        }

        public void Interrupt_SWI()
        {
            Interrupt(0x0F);
        }
        #endregion

        #region Supervisor Mode
        private void Mode_SupervisorMode()
        {

        }

        private void Mode_UserMode()
        {

        }
        #endregion

        private ushort SizeOfLastInstruction(ushort current_address)
        {
            ushort word = ReadMemInt16((ushort)(current_address - 2));
            YCPUInstruction opcode = Opcodes[word & 0x00FF];
            if (opcode.UsesNextWord(word))
                return 4;
            else
                return 2;
        }

        #region Stack
        private void StackPush(ushort value)
        {
            SP -= 2;
            WriteMemInt16(SP, value);
        }

        private ushort StackPop()
        {
            ushort value = ReadMemInt16(SP);
            SP += 2;
            return value;
        }
        #endregion

        private YRTC m_RTC;
        private YBUS m_Bus;

        private long m_Cycles;

        private bool m_Running = false, m_Pausing = false, m_ExecuteFail = false;
    }
}
