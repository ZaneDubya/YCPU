using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCPU.Platform
{
    public partial class YCPU
    {
        private YBUS m_Bus;
        private YRTC m_RTC;
        private long m_Cycles;
        private bool m_Running = false, m_Pausing = false;

        private System.Diagnostics.Stopwatch m_Stopwatch;
        private int m_LastRunCycles = 0;
        private int m_LastRunMS = 0;
        public int LastRunCycles
        {
            get { return m_LastRunCycles; }
        }
        public int LastRunMS
        {
            get { return m_LastRunMS; }
        }

        private const ushort c_GetMemory_ExecuteFail = 0xFFFF;

        public YCPU()
        {
            m_Bus = new YBUS();
            m_RTC = new YRTC();
            m_Stopwatch = new System.Diagnostics.Stopwatch();

            InitializeOpcodes();
            InitializeMemory();
            PS = 0x0000;

            Random r = new Random();
            for (int i = 0; i < 0x10000; i++)
                SetMemory((ushort)i, (ushort)r.Next(0, 0x10000));

            m_Rom_CPU[0x00][0x0000] = 0x0000;
            m_Rom_CPU[0x00][0x0001] = 0xFFFF;
            m_Rom_CPU[0x00][0x0002] = 0x2000;
            m_Rom_CPU[0x00][0x0003] = 0x0004;
            m_Rom_CPU[0x00][0x0004] = 0x2011;
            m_Rom_CPU[0x00][0x0005] = 0x04C4;
        }

        public void Run(int cyclecount = -1)
        {
            // Count Cycles:
            long cycles_start = m_Cycles;
            long cycles_target = (cyclecount == -1) ? long.MaxValue : cyclecount + m_Cycles;

            // To measure the speed of the processor:
            m_Stopwatch.Reset();
            m_Stopwatch.Start();

            // Run the processor for cyclecount Cycles:
            m_Running = true;
            while (m_Running)
            {
                ushort word = GetMemory(PC++, true);
                if (word != c_GetMemory_ExecuteFail)
                {
                    // Check for hardware interrupt:
                    if (PS_I && !PS_Q && m_Bus.IRQ)
                    {
                        Interrupt_HWI();
                    }
                    // Check for RTC interrupt:
                    if (m_RTC.IRQ(m_Cycles))
                    {
                        Interrupt_Clock();
                    }
                    // Execute Memory[PC]
                    ushort nextword = GetMemory(PC);
                    YCPUInstruction opcode = m_Opcodes[word & 0x00FF];
                    opcode.Opcode(word, nextword, opcode.BitPattern);
                    // Increment the Cycle counter. Check to see if the
                    // processor has run the desired number of Cycles.
                    m_Cycles += opcode.Cycles;
                    if (m_Cycles >= cycles_target)
                        m_Pausing = true;
                    if (m_Pausing)
                    {
                        m_Running = false;
                        m_Pausing = false;
                    }
                }
            }
            // Get the speed of the processor:
            m_Stopwatch.Stop();
            m_LastRunMS = (int)m_Stopwatch.ElapsedMilliseconds;
            m_LastRunCycles = (int)(m_Cycles - cycles_start);
        }

        public void Pause()
        {
            if (!m_Running)
                return;
            m_Pausing = true;
            while (m_Pausing == true)
            {
                // wait until the cpu has stopped running, then return.
            }
        }

        public void Benchmark(bool mmu_enabled, int count_runs)
        {
            string[] benchmark = new string[0x101];
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            System.Diagnostics.Stopwatch total = new System.Diagnostics.Stopwatch();
            total.Reset();
            total.Start();
            int count = 0;
            int cycles = 0;
            for (int i = 0; i < 0x100; i++)
            {
                timer.Reset();
                timer.Start();
                PS_M = mmu_enabled;
                if (m_Opcodes[i].IsNOP)
                {
                    // Ignore NOPs for benchmarking.
                }
                else
                {
                    for (int j = 0; j < count_runs; j++)
                    {
                        ushort word = (ushort)(i & ((j & 0xFF) << 8));
                        m_Opcodes[i].Opcode(word, 0x0000, m_Opcodes[i].BitPattern);
                        count++;
                        cycles += m_Opcodes[i].Cycles;
                    }
                }
                timer.Stop();
                benchmark[i] = string.Format("{0}     {1}", m_Opcodes[i].Name, timer.ElapsedMilliseconds);
            }
            total.Stop();
            benchmark[0x100] = string.Format("{0} opcodes, {1} cycles in {2} ms.", count, cycles, total.ElapsedMilliseconds);
            System.IO.File.WriteAllLines(string.Format("Benchmark{0}.txt", PS_M ? "M" : ""), benchmark);

        }

        #region General Purpose Registers
        public enum RegGPIndex
        {
            R0, R1, R2, R3, R4, R5, R6, R7,
            Count
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
            FL, IA, II, PC, PS, P2, USP, SSP,
            Count
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
            }
        }

        public bool PS_M
        {
            get
            {
                return ((m_PS & c_PS_M) != 0);
            }
            private set
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
            private set
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
            PC = GetMemory((ushort)(IA + interrupt_number));
            m_Cycles += 32;
        }

        private void ReturnFromInterrupt()
        {
            PC = StackPop();
            PS = StackPop();
            PS &= 0xF0FF; // clear Q, U, W, and E.
        }

        public void Interrupt_Reset()
        {
            m_RTC.DisableInterrupt();
            PS = 0x8000;
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

        private void Interrupt_BankFault()
        {
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
            USP = R[(int)RegGPIndex.R7];
            R[(int)RegGPIndex.R7] = SSP;
        }

        private void Mode_UserMode()
        {
            SSP = R[(int)RegGPIndex.R7];
            R[(int)RegGPIndex.R7] = USP;
        }
        #endregion

        #region Memory (RAM/ROM/MMU)
        private ushort[][] m_M = new ushort[0x10][]; // the loaded memory banks.
        private bool[] m_M_ROM = new bool[0x10]; // setting these values to true will cause writes to fail silently.
        // Internal Processor Memory and ROM banks.
        private ushort[][] m_Mem_CPU;
        private ushort[][] m_Rom_CPU;
        private ushort m_Mem_CPU_Count = 0x0010;
        private ushort m_Rom_CPU_Count = 0x0001;
        // MMU Tables
        private ushort[][] m_MMU;
        private const ushort c_MMUCache_H = 0x1000, c_MMUCache_E = 0x2000, c_MMUCache_W = 0x4000, c_MMUCache_S = 0x8000;
        private const ushort c_MMUCache_A = 0x0800, c_MMUCache_P = 0x0400;
        private const ushort c_MMUCache_Rom = 0x0008;

        public void InitializeMemory()
        {
            m_Mem_CPU = new ushort[m_Mem_CPU_Count][];
            for (int i = 0; i < m_Mem_CPU_Count; i++)
                m_Mem_CPU[i] = new ushort[0x1000];
            m_Rom_CPU = new ushort[m_Rom_CPU_Count][];
            for (int i = 0; i < m_Rom_CPU_Count; i++)
                m_Rom_CPU[i] = new ushort[0x1000];
            m_MMU = new ushort[0x10][];
            for (int i = 0; i < 0x10; i++)
                m_MMU[i] = new ushort[2] { 0x0000, 0x0000 };
        }

        public ushort GetMemory(ushort address, bool execute = false)
        {
            int bank = (address & 0xF000) >> 12;
            if (PS_M)
            {
                // access to not present bank.
                if ((m_MMU[bank][1] & c_MMUCache_P) != 0)
                {
                    PS_U = false;
                    PS_W = false;
                    PS_E = false;
                    P2 = address;
                    Interrupt_BankFault();
                    return 0x0000;
                }
                // if not in supervisor mode and attempting to access supervisor memory...
                else if (!PS_S && ((m_MMU[bank][1] & c_MMUCache_S) != 0))
                {
                    PS_U = true;
                    PS_W = false;
                    PS_E = false;
                    P2 = address;
                    Interrupt_BankFault();
                    return 0x0000;
                }
                // if attempting to execute execute-protected memory...
                else if (execute && ((m_MMU[bank][1] & c_MMUCache_E) != 0))
                {
                    PS_U = false;
                    PS_W = false;
                    PS_E = true;
                    P2 = address;
                    Interrupt_BankFault();
                    return c_GetMemory_ExecuteFail; // this value will cause the instruction not to execute
                }
            }
            return m_M[bank][(address & 0x0FFF)];
        }

        private void SetMemory(ushort address, ushort value)
        {
            int bank = (address & 0xF000) >> 12;
            if (PS_M)
            {
                // if not in supervisor mode and attempting to access supervisor memory...
                if (!PS_S && ((m_MMU[bank][1] & c_MMUCache_S) != 0))
                {
                    PS_U = true;
                    PS_W = true;
                    PS_E = false;
                    P2 = address;
                    Interrupt_BankFault();
                    return;
                }
                // if attempting to write to read-only memory...
                else if (((m_MMU[bank][1] & c_MMUCache_W) != 0))
                {
                    PS_U = false;
                    PS_W = true;
                    PS_E = false;
                    P2 = address;
                    Interrupt_BankFault();
                    return;
                }
                m_MMU[bank][1] |= c_MMUCache_A;
            }
            if (!m_M_ROM[bank])
                m_M[bank][(address & 0x0FFF)] = value;
        }

        private ushort MMU_Read(ushort index)
        {
            int bank = (index & 0x001E) >> 1;
            int hi_lo_select = (index & 0x0001);
            return m_MMU[bank][hi_lo_select];
        }

        private void MMU_Write(ushort index, ushort value)
        {
            int bank = (index & 0x001E) >> 1;
            int hi_lo_select = (index & 0x0001);
            m_MMU[bank][hi_lo_select] = value;
        }

        private void MMU_LoadMemoryWithCacheData(ushort address)
        {
            for (int i = 0; i < 0x10; i++)
            {
                SetMemory(address++, m_MMU[i][0]);
                SetMemory(address++, m_MMU[i][1]);
            }
        }

        private void MMU_StoreCacheDataFromMemory(ushort address)
        {
            for (int i = 0; i < 0x10; i++)
            {
                m_MMU[i][0] = GetMemory(address++);
                m_MMU[i][1] = GetMemory(address++);
            }
        }

        private void MMU_Disable()
        {
            MMU_PS_R_Updated(); // set the first bank based on the r processor status bit.
            for (int i = 0x01; i < 0x10; i++)
            {
                m_M[i] = m_Mem_CPU[i];
                m_M_ROM[i] = false;
            }
        }

        private void MMU_Enable()
        {
            for (int i = 0x00; i < 0x10; i++)
            {
                if ((m_MMU[i][1] & c_MMUCache_H) == 0) // select internal memory space
                {
                    if ((m_MMU[i][1] & c_MMUCache_Rom) == 0) // select internal ram
                    {
                        m_M[i] = m_Mem_CPU[i % m_Mem_CPU_Count];
                    }
                    else
                    {
                        m_M[i] = m_Rom_CPU[i % m_Rom_CPU_Count];
                    }
                }
                else
                {
                    // select external bank. not yet implemented.
                    m_M[i] = null;
                }
                m_M[i] = m_Mem_CPU[i];
            }
        }

        private void MMU_PS_R_Updated()
        {
            if (!PS_M)
                m_M[0x00] = (PS_R) ? m_Mem_CPU[0x00] : m_Rom_CPU[0x00];
            m_M_ROM[0x00] = !PS_R;
        }
        #endregion

        private ushort SizeOfLastInstruction(ushort current_address)
        {
            ushort word = GetMemory((ushort)(current_address - 2));
            YCPUInstruction opcode = m_Opcodes[word & 0x00FF];
            if (opcode.UsesNextWord(word))
                return 2;
            else
                return 1;
        }

        #region Stack
        private void StackPush(ushort value)
        {
            R[(int)RegGPIndex.R7]--;
            SetMemory(R[(int)RegGPIndex.R7], value);
        }

        private ushort StackPop()
        {
            ushort value = GetMemory(R[(int)RegGPIndex.R7]);
            R[(int)RegGPIndex.R7]++;
            return value;
        }
        #endregion
    }
}
