
namespace Ypsilon.Hardware
{
    partial class YCPU
    {
        // Four loaded memory banks that are addressed by operations. Each memory bank is 0x4000 = 16kb bytes.
        private IMemoryBank[] m_Mem;

        // Internal Processor Memory and ROM banks.
        private MemoryBank[] m_Mem_CPU;
        private MemoryBank[] m_Rom_CPU;
        private const ushort m_Mem_CPU_Count = 0x0004; // YCPU is guaranteed to have at least 4x16kb banks of internal memory.
        private const ushort m_Rom_CPU_Count = 0x0001;

        // MMU Cache: 4 x 16-bit words for both Supervisor and User modes.
        private ushort[] m_MMU;
        private const ushort c_MMUCache_A = 0x1000, c_MMUCache_P = 0x2000, c_MMUCache_W = 0x4000, c_MMUCache_S = 0x8000;
        private const ushort c_MMUCache_Device = 0x0F00, c_MMUCache_Bank = 0x00FF;
        private const ushort c_MMUCache_ProcessorRom = 0x0080;

        /// <summary>
        /// Initializes arrays for memory banks, internal ram, and internal rom.
        /// Must be called before running any operations.
        /// </summary>
        public void InitializeMemory()
        {
            m_Mem = new IMemoryBank[0x04];
            m_MMU = new ushort[8];

            m_Mem_CPU = new MemoryBank[m_Mem_CPU_Count];
            for (int i = 0; i < m_Mem_CPU_Count; i += 1)
                m_Mem_CPU[i] = new MemoryBank();

            m_Rom_CPU = new MemoryBank[m_Rom_CPU_Count];
            for (int i = 0; i < m_Rom_CPU_Count; i += 1)
                m_Rom_CPU[i] = new MemoryBank();
        }

        public delegate ushort ReadMemInt16Method(ushort address, bool execute = false);
        public delegate void WriteMemInt16Method(ushort address, ushort value);
        public delegate byte ReadMemInt8Method(ushort address, bool execute = false);
        public delegate void WriteMemInt8Method(ushort address, byte value);

        public ReadMemInt16Method ReadMemInt16 = null;
        public WriteMemInt16Method WriteMemInt16 = null;
        public ReadMemInt8Method ReadMemInt8 = null;
        public WriteMemInt8Method WriteMemInt8 = null;

        /// <summary>
        /// Checks if a memory address is accessible. Fails if (1) address is not loaded, (2) attempt access of 
        /// supervisor-only bank in user mode, (3) attempts execute of no-execute bank.
        /// </summary>
        /// <param name="address">The address to check.</param>
        /// <param name="execute">Set to true if address read is for execution purposes, false if only read as data.</param>
        /// <returns>True if address is accessible for reading in the current processor state.</returns>
        private bool MMU_CheckRead(ushort address, bool execute)
        {
            int bank = (address & 0xC000) >> 14;
            uint mmu = m_MMU[bank];

            if ((mmu & c_MMUCache_P) != 0)
            {
                // memory access failed, attempted access of bank that is not loaded.
                PS_U = false;
                PS_W = false;
                P2 = address;
                Interrupt_BankFault(execute);
                return false;
            }
            else if (!m_PS_S && ((mmu & c_MMUCache_S) != 0))
            {
                // memory access failed, attempted to access supervisor memory while not in supervisor mode.
                PS_U = true;
                PS_W = false;
                P2 = address;
                Interrupt_BankFault(execute);
                return false;
            }
            else
            {
                // memory access is successful.
                return true;
            }
        }

        /// <summary>
        /// Checks if a memory address is accessible and writeable. Fails if (1) address is not loaded, (2) attempt access of 
        /// supervisor-only bank in user mode, (3) attempts execute of no-execute bank.
        /// </summary>
        /// <param name="address">The address to check.</param>
        /// <returns>True if address is accessible for writing in the current processor state.</returns>
        private bool MMU_CheckWrite(ushort address)
        {
            int bank = (address & 0xC000) >> 14;
            uint mmu = m_MMU[bank];

            if ((mmu & c_MMUCache_P) != 0)
            {
                // memory access failed, attempted write to bank that is not loaded.
                PS_U = false;
                PS_W = false;
                P2 = address;
                Interrupt_BankFault(false);
                return false;
            }
            else if (!PS_S && ((mmu & c_MMUCache_S) != 0))
            {
                // if not in supervisor mode and attempting to write to supervisor memory...
                PS_U = true;
                PS_W = true;
                P2 = address;
                Interrupt_BankFault(false);
                return false;
            }
            else if (((mmu & c_MMUCache_W) != 0))
            {
                // if attempting to write to read-only memory...
                PS_U = false;
                PS_W = true;
                P2 = address;
                Interrupt_BankFault(false);
                return false;
            }
            else
            {
                // memory access is successful.
                return true;
            }
        }

        /// <summary>
        /// Reads an 8-bit value from the address specified. If memory access fails, will raise an interrupt.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="execute"></param>
        /// <returns>If the bank access is successful, return the value of the Int8 at this address, else return 0x00.</returns>
        public byte ReadMemInt8_MMUEnabled(ushort address, bool execute = false)
        {
            m_Cycles += 1;
            // Int8 accesses do not have to be 16-bit aligned. Only one memory access is needed.
            if (MMU_CheckRead(address, execute))
                return m_Mem[(address & 0xC000) >> 14][(address)];
            else
                return 0x00;
        }

        /// <summary>
        /// Reads a 16-bit value from the address specified. If memory access fails, will raise an interrupt.
        /// </summary>
        /// <param name="address">The address to read from. 16-bit aligned accesses are ~2x faster.</param>
        /// <param name="execute"></param>
        /// <returns>If the bank access is successful, return the value of the Int16 at this address, else return 0x0000.</returns>
        public ushort ReadMemInt16_MMUEnabled(ushort address, bool execute = false)
        {
            if ((address & 0x0001) == 0x0001)
            {
                // This read is not 16-bit aligned. Two memory accesses needed.
                m_Cycles += 2;

                // If both bank accesses are successful, return the value of the Int16 at this address, else return 0x0000.
                byte byte0, byte1;
                if (MMU_CheckRead(address, execute))
                {
                    int bank = (address & 0xC000) >> 14;
                    byte0 = m_Mem[bank][(address)];
                }
                else
                {
                    return 0x0000;
                }

                address += 1;

                if (MMU_CheckRead(address, execute))
                {
                    int bank = (address & 0xC000) >> 14;
                    byte1 = m_Mem[bank][(address)];
                }
                else
                {
                    return 0x0000;
                }
                return (ushort)((byte1 << 8) + byte0);
            }
            else
            {
                // This read is 16-bit aligned. Only one memory access is needed.
                m_Cycles += 1;

                // If the bank access is successful, return the value of the Int16 at this address, else return 0x0000.
                if (MMU_CheckRead(address, execute))
                {
                    int bank = (address & 0xC000) >> 14;
                    return (ushort)((m_Mem[bank][(address + 1)] << 8) | (m_Mem[bank][(address + 0)]));
                }
                else
                {
                    return 0x0000;
                }
            }
        }

        /// <summary>
        /// Reads an 8-bit value from the address specified.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="execute"></param>
        /// <returns>Return the value of the Int8 at this address.</returns>
        public byte ReadMemInt8_MMUDisabled(ushort address, bool execute = false)
        {
            m_Cycles += 1;
            // Int8 accesses do not have to be 16-bit aligned. Only one memory access is needed.
            int bank = (address & 0xC000) >> 14;
            return m_Mem[bank][(address)];
        }

        /// <summary>
        /// Reads a 16-bit value from the address specified.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="execute"></param>
        /// <returns>Return the value of the Int16 at this address.</returns>
        public ushort ReadMemInt16_MMUDisabled(ushort address, bool execute = false)
        {
            if ((address & 0x0001) == 0x0001)
            {
                m_Cycles += 2;
                // This read is not 16-bit aligned. Two memory accesses needed.
                byte byte0, byte1;
                int bank = (address & 0xC000) >> 14;
                byte0 = m_Mem[bank][(address)];

                address += 1;
                bank = (address & 0xC000) >> 14;
                byte1 = m_Mem[bank][(address)];

                return (ushort)(byte1 << 8 + byte0);
            }
            else
            {
                m_Cycles += 1;
                // This read is 16-bit aligned.. Only one memory access is needed.
                int bank = (address & 0xC000) >> 14;
                return (ushort)((m_Mem[bank][address]) + (m_Mem[bank][address + 1] << 8));
            }

        }

        private void WriteMemInt8_MMUEnabled(ushort address, byte value)
        {
            // Int8 accesses do not have to be 16-bit aligned. Only one memory access is needed.
            // If the bank access is successful, write the value of the Int8 to this address.
            if (MMU_CheckWrite(address))
            {
                int bank = (address & 0xC000) >> 14;
                m_Mem[bank][(address)] = value;
            }
        }

        private void WriteMemInt16_MMUEnabled(ushort address, ushort value)
        {
            if ((address & 0x0001) == 0x0001)
            {
                // This write is not 16-bit aligned. Two memory accesses needed.
                // If both bank accesses are successful, write the value of the Int16 at this address.
                if (MMU_CheckWrite(address))
                {
                    if (MMU_CheckWrite((ushort)(address + 1)))
                    {
                        int bank = (address & 0xC000) >> 14;
                        m_Mem[bank][address] = (byte)(value & 0x00ff);

                        address += 1;

                        bank = (address & 0xC000) >> 14;
                        m_Mem[bank][address] = (byte)((value & 0xff00) >> 8);
                    }
                }
            }
            else
            {
                // 16-bit word aligned access. Only one memory access is needed.
                if (MMU_CheckWrite(address))
                {
                    int bank = (address & 0xC000) >> 14;
                    m_Mem[bank][(address)] = (byte)(value & 0x00ff);
                    m_Mem[bank][(address + 1)] = (byte)((value & 0xff00) >> 8);
                }
            }
        }

        private void WriteMemInt8_MMUDisabled(ushort address, byte value)
        {
            // Int8 accesses do not have to be 16-bit aligned. Only one memory access is needed.
            int bank = (address & 0xC000) >> 14;
            m_Mem[bank][(address)] = value;
        }

        private void WriteMemInt16_MMUDisabled(ushort address, ushort value)
        {
            if ((address & 0x0001) == 0x0001)
            {
                // This write is not 16-bit aligned. Two memory accesses needed.
                int bank = (address & 0xC000) >> 14;
                m_Mem[bank][address] = (byte)((value & 0xff00) >> 8);

                address += 1;

                bank = (address & 0xC000) >> 14;
                m_Mem[bank][address] = (byte)(value & 0x00ff);
            }
            else
            {
                // 16-bit word aligned access. Only one memory access is needed.
                int bank = (address & 0xC000) >> 14;
                m_Mem[bank][address++] = (byte)(value & 0x00ff);
                m_Mem[bank][address] = (byte)((value & 0xff00) >> 8); 
            }
        }

        /// <summary>
        /// Reads a word from the Memory Management Cache into a register.
        /// The value in Ra is interpreted as follows:
        /// $0000 - first word of user cache
        /// ...
        /// $0007 - last word of user cache
        /// $0008 - first word of supervisor cache
        /// ...
        /// $000F - last word of supervisor cache  
        /// </summary>
        public ushort MMU_Read(ushort index)
        {
            return m_MMU[index];
        }

        /// <summary>
        /// Writes a word to the Memory Management Cache from a register.
        /// See MMR for how value of Ra is interpreted.
        /// </summary>
        private void MMU_Write(ushort index, ushort value)
        {
            m_MMU[index] = value;
        }

        /// <summary>
        /// Pushes 4 words of MMU cache data onto the stack.
        /// </summary>
        /// <param name="regIndex">If bit 0 of this register are clear, user cache is pushed; bit set, superviser cache.</param>
        private void MMU_PushCacheOntoStack(ushort value)
        {
            bool isSupervisor = (value & 0x01) != 0;
            int mmuBase = (int)(isSupervisor ? 4 : 0);

            for (int i = 0; i < 4; i++)
            {
                ushort mmuWord = MMU_Read((ushort)(i + mmuBase));
                StackPush(mmuWord);
            }
        }

        /// <summary>
        /// Pulls 4 words from stack and write these to the MMU cache.
        /// </summary>
        /// <param name="regIndex">If bit 0 of this register are clear, user cache is pulled; bit set, superviser cache.</param>
        private void MMU_PullCacheFromStack(ushort value)
        {
            bool isSupervisor = (value & 0x01) != 0;
            int mmuBase = (int)(isSupervisor ? 4 : 0);

            for (int i = 0; i < 4; i++)
            {
                ushort mmuWord = StackPop();
                MMU_Write((ushort)(mmuBase - 1 - i), mmuWord);
            }
        }

        private void MMU_Disable()
        {
            ReadMemInt8 = ReadMemInt8_MMUDisabled;
            ReadMemInt16 = ReadMemInt16_MMUDisabled;
            WriteMemInt8 = WriteMemInt8_MMUDisabled;
            WriteMemInt16 = WriteMemInt16_MMUDisabled;

            m_Mem[0] = m_Rom_CPU[0];

            for (int i = 0x01; i < 0x04; i++)
            {
                m_Mem[i] = m_Mem_CPU[i];
                m_Mem[i].ReadOnly = false;
            }
        }

        private void MMU_Enable()
        {
            ReadMemInt8 = ReadMemInt8_MMUEnabled;
            ReadMemInt16 = ReadMemInt16_MMUEnabled;
            WriteMemInt8 = WriteMemInt8_MMUEnabled;
            WriteMemInt16 = WriteMemInt16_MMUEnabled;

            int mmuCacheBase = (PS_S) ? 4 : 0;
            for (int i = mmuCacheBase; i < mmuCacheBase + 4; i++)
            {
                ushort device = (ushort)((m_MMU[i] & c_MMUCache_Device) >> 8); // 0x0f00
                ushort bank = (ushort)(m_MMU[i] & c_MMUCache_Bank); // 0x00ff
                if (device == 0x00) // select internal memory space
                {
                    if ((bank & c_MMUCache_ProcessorRom) == 0) // select internal ram
                    {
                        m_Mem[i - mmuCacheBase] = m_Mem_CPU[bank % m_Mem_CPU_Count];
                    }
                    else // select internal rom
                    {
                        m_Mem[i - mmuCacheBase] = m_Rom_CPU[(bank & 0x7f) % m_Rom_CPU_Count];
                    }
                }
                else
                {
                    m_Mem[i - mmuCacheBase] = (IMemoryBank)m_Bus.GetMemoryBank(device, bank);
                }
            }
        }

        public void MMU_SwitchInHardwareBank(ushort mmuBankIndex, ushort deviceBusIndex, ushort deviceBankIndex)
        {
            ushort w00 = (ushort)((deviceBankIndex & 0x00FF) + ((deviceBusIndex & 0x00FF) << 8));
            ushort w01 = (ushort)0; // no protection.

            MMU_Write((ushort)(mmuBankIndex * 2), w00);
            MMU_Write((ushort)(mmuBankIndex * 2 + 1), w01);
        }

        public ushort DebugReadMemory(ushort address)
        {
            long cycles = m_Cycles;
            ushort memory = ReadMemInt16(address);
            m_Cycles = cycles;
            return memory;
        }
    }
}
