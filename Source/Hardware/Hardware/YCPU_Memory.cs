using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace YCPU.Hardware
{
    partial class YCPU
    {
        #region Memory (RAM/ROM/MMU)

        private IMemoryBank[] m_M = new IMemoryBank[0x10]; // the loaded memory banks.

        // Internal Processor Memory and ROM banks.
        private MemoryBank[] m_Mem_CPU;
        private MemoryBank[] m_Rom_CPU;
        private ushort m_Mem_CPU_Count = 0x0010;
        private ushort m_Rom_CPU_Count = 0x0001;

        // MMU Tables
        private ushort[][] m_MMU;
        private const ushort c_MMUCache_H = 0x1000, c_MMUCache_E = 0x2000, c_MMUCache_W = 0x4000, c_MMUCache_S = 0x8000;
        private const ushort c_MMUCache_A = 0x0800, c_MMUCache_P = 0x0400, c_MMUCache_ESP = 0xA400;
        private const ushort c_MMUCache_Rom = 0x0008;

        public void InitializeMemory()
        {
            m_Mem_CPU = new MemoryBank[m_Mem_CPU_Count];
            for (int i = 0; i < m_Mem_CPU_Count; i += 1)
                m_Mem_CPU[i] = new MemoryBank();
            m_Rom_CPU = new MemoryBank[m_Rom_CPU_Count];
            for (int i = 0; i < m_Rom_CPU_Count; i += 1)
                m_Rom_CPU[i] = new MemoryBank();
            m_MMU = new ushort[0x10][];
            for (int i = 0; i < 0x10; i += 1)
                m_MMU[i] = new ushort[2] { (ushort)i, 0x0000 };
        }

        public delegate ushort ReadMemInt16Method(ushort address, bool execute = false);
        private delegate void WriteMemInt16Method(ushort address, ushort value);
        public delegate byte ReadMemInt8Method(ushort address, bool execute = false);
        private delegate void WriteMemInt8Method(ushort address, byte value);

        public ReadMemInt16Method ReadMemInt16 = null;
        private WriteMemInt16Method WriteMemInt16 = null;
        public ReadMemInt8Method ReadMemInt8 = null;
        private WriteMemInt8Method WriteMemInt8 = null;

        private bool MMU_CheckRead(ushort address, bool execute)
        {
            int bank = (address & 0xF000) >> 12;
            ushort bits = m_MMU[bank][1];

            if ((bits & c_MMUCache_P) != 0)
            {
                // memory access failed, attempted read of bank that is not loaded.
                PS_U = false;
                PS_W = false;
                PS_E = false;
                P2 = address;
                Interrupt_BankFault(execute);
                return false;
            }
            else if (!m_PS_S && ((bits & c_MMUCache_S) != 0))
            {
                // memory access failed, attempted to access supervisor memory while not in supervisor mode.
                PS_U = true;
                PS_W = false;
                PS_E = false;
                P2 = address;
                Interrupt_BankFault(execute);
                return false;
            }
            else if (execute && ((bits & c_MMUCache_E) != 0))
            {
                // memory access failed, attempted to execute execute-protected memory.
                PS_U = false;
                PS_W = false;
                PS_E = true;
                P2 = address;
                Interrupt_BankFault(execute);
                return false;
            }
            else
            {
                // memory access is successful, return true.
                return true;
            }
        }

        private bool MMU_CheckWrite(ushort address)
        {
            int bank = (address & 0xF000) >> 12;
            ushort bits = m_MMU[bank][1];
            // if not in supervisor mode and attempting to access supervisor memory...
            if (!PS_S && ((bits & c_MMUCache_S) != 0))
            {
                PS_U = true;
                PS_W = true;
                PS_E = false;
                P2 = address;
                Interrupt_BankFault(false);
                return false;
            }
            // if attempting to write to read-only memory...
            else if (((bits & c_MMUCache_W) != 0))
            {
                PS_U = false;
                PS_W = true;
                PS_E = false;
                P2 = address;
                Interrupt_BankFault(false);
                return false;
            }
            else
            {
                return true;
            }
        }

        public byte ReadMemInt8_MMUEnabled(ushort address, bool execute = false)
        {
            // Int8 accesses do not have to be 16-bit aligned. Only one memory access is needed.
            // If the bank access is successful, return the value of the Int8 at this address, else return 0x00.
            if (MMU_CheckRead(address, execute))
                return m_M[(address & 0xF000) >> 12][(address)];
            else
                return 0x00;
        }

        public ushort ReadMemInt16_MMUEnabled(ushort address, bool execute = false)
        {
            if ((address & 0x0001) == 0x0001)
            {
                // This read is not 16-bit aligned. Two memory accesses needed.
                // If both bank accesses are successful, return the value of the Int16 at this address, else return 0x0000.
                byte byte0, byte1;
                if (MMU_CheckRead(address, execute))
                {
                    int bank = (address & 0xF000) >> 12;
                    byte0 = m_M[bank][(address)];
                }
                else
                {
                    return 0x0000;
                }

                address += 1;

                if (MMU_CheckRead(address, execute))
                {
                    int bank = (address & 0xF000) >> 12;
                    byte1 = m_M[bank][(address)];
                }
                else
                {
                    return 0x0000;
                }
                return (ushort)((byte1 << 8) + byte0);
            }
            else
            {
                // This read is 16-bit aligned.. Only one memory access is needed.
                // If the bank access is successful, return the value of the Int16 at this address, else return 0x0000.
                if (MMU_CheckRead(address, execute))
                {
                    int bank = (address & 0xF000) >> 12;
                    return (ushort)(m_M[bank][(address + 1)] << 8 | (m_M[bank][(address + 0)]));
                }
                else
                {
                    return 0x0000;
                }
            }
        }

        public byte ReadMemInt8_MMUDisabled(ushort address, bool execute = false)
        {
            // Int8 accesses do not have to be 16-bit aligned. Only one memory access is needed.
            int bank = (address & 0xF000) >> 12;
            return m_M[bank][(address)];
        }

        public ushort ReadMemInt16_MMUDisabled(ushort address, bool execute = false)
        {
            if ((address & 0x0001) == 0x0001)
            {
                // This read is not 16-bit aligned. Two memory accesses needed.
                byte byte0, byte1;
                int bank = (address & 0xF000) >> 12;
                byte0 = m_M[bank][(address)];

                address += 1;
                bank = (address & 0xF000) >> 12;
                byte1 = m_M[bank][(address)];

                return (ushort)(byte1 << 8 + byte0);
            }
            else
            {
                // This read is 16-bit aligned.. Only one memory access is needed.
                int bank = (address & 0xF000) >> 12;
                return (ushort)((m_M[bank][address]) + (m_M[bank][address + 1] << 8));
            }

        }

        private void WriteMemInt8_MMUEnabled(ushort address, byte value)
        {
            // Int8 accesses do not have to be 16-bit aligned. Only one memory access is needed.
            // If the bank access is successful, write the value of the Int8 to this address.
            if (MMU_CheckWrite(address))
            {
                int bank = (address & 0xF000) >> 12;
                m_M[bank][(address)] = value;
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
                        int bank = (address & 0xF000) >> 12;
                        m_M[bank][address] = (byte)(value & 0x00ff);

                        address += 1;

                        bank = (address & 0xF000) >> 12;
                        m_M[bank][address] = (byte)((value & 0xff00) >> 8);
                    }
                }
            }
            else
            {
                // 16-bit word aligned access. Only one memory access is needed.
                if (MMU_CheckWrite(address))
                {
                    int bank = (address & 0xF000) >> 12;
                    m_M[bank][(address)] = (byte)(value & 0x00ff);
                    m_M[bank][(address + 1)] = (byte)((value & 0xff00) >> 8);
                }
            }
        }

        private void WriteMemInt8_MMUDisabled(ushort address, byte value)
        {
            // Int8 accesses do not have to be 16-bit aligned. Only one memory access is needed.
            int bank = (address & 0xF000) >> 12;
            m_M[bank][(address)] = value;
        }

        private void WriteMemInt16_MMUDisabled(ushort address, ushort value)
        {
            if ((address & 0x0001) == 0x0001)
            {
                // This write is not 16-bit aligned. Two memory accesses needed.
                int bank = (address & 0xF000) >> 12;
                m_M[bank][address] = (byte)((value & 0xff00) >> 8);

                address += 1;

                bank = (address & 0xF000) >> 12;
                m_M[bank][address] = (byte)(value & 0x00ff);
            }
            else
            {
                // 16-bit word aligned access. Only one memory access is needed.
                int bank = (address & 0xF000) >> 12;
                m_M[bank][(address)] = (byte)((value & 0xff00) >> 8);
                m_M[bank][(address + 1)] = (byte)(value & 0x00ff);
            }
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
            for (int i = 0; i < 0x10; i += 1)
            {
                WriteMemInt16(address, m_MMU[i][0]);
                address += 2;
                WriteMemInt16(address, m_MMU[i][1]);
                address += 2;
            }
        }

        private void MMU_StoreCacheDataFromMemory(ushort address)
        {
            for (int i = 0; i < 0x10; i += 1)
            {
                m_MMU[i][0] = ReadMemInt16(address);
                address += 2;
                m_MMU[i][1] = ReadMemInt16(address);
                address += 2;
            }
        }

        private void MMU_Disable()
        {
            ReadMemInt8 = ReadMemInt8_MMUDisabled;
            ReadMemInt16 = ReadMemInt16_MMUDisabled;
            WriteMemInt8 = WriteMemInt8_MMUDisabled;
            WriteMemInt16 = WriteMemInt16_MMUDisabled;

            MMU_PS_R_Updated(); // set the first bank based on the r processor status bit.
            for (int i = 0x01; i < 0x10; i += 1)
            {
                m_M[i] = m_Mem_CPU[i];
                m_M[i].ReadOnly = false;
            }
        }

        private void MMU_Enable()
        {
            ReadMemInt8 = ReadMemInt8_MMUEnabled;
            ReadMemInt16 = ReadMemInt16_MMUEnabled;
            WriteMemInt8 = WriteMemInt8_MMUEnabled;
            WriteMemInt16 = WriteMemInt16_MMUEnabled;

            for (int i = 0x00; i < 0x10; i += 1)
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
                    ushort device_index = (ushort)(((m_MMU[i][1] & 0x000F) << 4) + ((m_MMU[i][0] & 0xF000) >> 12));
                    ushort bank_index = (ushort)(m_MMU[i][0] & 0x0FFF);
                    m_M[i] = (IMemoryBank)m_Bus.GetMemoryBank(device_index, bank_index);
                }
            }
        }

        private void MMU_PS_R_Updated()
        {
            if (!PS_M)
                m_M[0x00] = (PS_R) ? m_Mem_CPU[0x00] : m_Rom_CPU[0x00];
            m_M[0x00].ReadOnly = !PS_R;
        }

        public void MMU_SwitchInHardwareBank(ushort bank_index, ushort device_index, ushort device_bank)
        {
            MMU_Write((ushort)(bank_index * 2), 0x0000);
            MMU_Write((ushort)(bank_index * 2 + 1), 0x1000);
        }
        #endregion
    }
}
