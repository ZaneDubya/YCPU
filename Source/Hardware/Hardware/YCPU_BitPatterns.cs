using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCPU.Hardware
{
    partial class YCPU
    {
        #region YCPU 16bit ALU/STO
        private void BitPatternALU_Immediate(ushort operand, out ushort value, out RegGPIndex destination)
        {
            // int AddressMode = (operand & 0x0007);
            destination = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex source = (RegGPIndex)((operand & 0x1C00) >> 10);
            int index_bits = ((operand & 0x0300) >> 8);

            // nextworld & 0x0100 == 0x0000 is immediate, 0x0100 is absolute.
            ushort nextword = ReadMemInt16(PC, true);
            if ((operand & 0x0100) != 0x0000)
                nextword = ReadMemInt16(nextword, false);
            value = nextword;
            
            PC += 2; // advance PC two bytes because we're reading an immediate value.
        }

        private void BitPatternALU_Register(ushort operand, out ushort value, out RegGPIndex destination)
        {
            // int AddressMode = (operand & 0x0007);
            destination = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex source = (RegGPIndex)((operand & 0x1C00) >> 10);
            int index_bits = ((operand & 0x0300) >> 8);
            value = R[(int)source];
        }

        private void BitPatternALU_Indirect(ushort operand, out ushort value, out RegGPIndex destination)
        {
            // int AddressMode = (operand & 0x0007);
            destination = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex source = (RegGPIndex)((operand & 0x1C00) >> 10);
            int index_bits = ((operand & 0x0300) >> 8);
            value = ReadMemInt16(R[(int)source]);
        }

        private void BitPatternALU_IndirectOffset(ushort operand, out ushort value, out RegGPIndex destination)
        {
            // int AddressMode = (operand & 0x0007);
            destination = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex source = (RegGPIndex)((operand & 0x1C00) >> 10);
            int index_bits = ((operand & 0x0300) >> 8);
            ushort nextword = ReadMemInt16(PC, true);
            value = ReadMemInt16((ushort)(R[(int)source] + nextword));
            PC += 2; // advance PC two bytes because we're reading an immediate value.
        }

        private void BitPatternALU_IndirectPostInc(ushort operand, out ushort value, out RegGPIndex destination)
        {
            // int AddressMode = (operand & 0x0007);
            destination = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex source = (RegGPIndex)((operand & 0x1C00) >> 10);
            int index_bits = ((operand & 0x0300) >> 8);
            value = ReadMemInt16(R[(int)source]);
            R[(int)source] += 2; // post increment by data size in bytes (16-bit = 2 bytes).
        }

        private void BitPatternALU_IndirectPreDec(ushort operand, out ushort value, out RegGPIndex destination)
        {
            // int AddressMode = (operand & 0x0007);
            destination = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex source = (RegGPIndex)((operand & 0x1C00) >> 10);
            int index_bits = ((operand & 0x0300) >> 8);
            R[(int)source] -= 2; // pre decrement by data size in bytes (16-bit = 2 bytes).
            value = ReadMemInt16(R[(int)source]);
        }

        private void BitPatternALU_IndirectIndexed(ushort operand, out ushort value, out RegGPIndex destination)
        {
            // int AddressMode = (operand & 0x0007);
            destination = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex source = (RegGPIndex)((operand & 0x1C00) >> 10);
            int index_bits = ((operand & 0x0300) >> 8);
            value = ReadMemInt16((ushort)(R[(int)source] + R[index_bits]));
        }

        private void BitPatternALU_IndirectIndexedHi(ushort operand, out ushort value, out RegGPIndex destination)
        {
            // int AddressMode = (operand & 0x0007);
            destination = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex source = (RegGPIndex)((operand & 0x1C00) >> 10);
            int index_bits = ((operand & 0x0300) >> 8);
            value = ReadMemInt16((ushort)(R[(int)source] + R[index_bits + 4]));
        }

        private void BitPatternSTO_Immediate(ushort operand, out ushort dest_address, out RegGPIndex source)
        {
            // int AddressMode = (operand & 0x0007);
            source = (RegGPIndex)((operand & 0xE000) >> 13);
            if ((operand & 0x0100) == 0x0100)
            {
                // absolute store
                ushort nextword = ReadMemInt16(PC, true);
                dest_address = nextword;
                PC += 2; // advance PC two bytes because we're reading a 16-bit immediate value.
            }
            else
            {
                // immediate store, no write.
                source = RegGPIndex.Error;
                dest_address = 0xffff;
            }
        }

        private void BitPatternSTO_Indirect(ushort operand, out ushort dest_address, out RegGPIndex source)
        {
            // int AddressMode = (operand & 0x0007);
            source = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex r2 = (RegGPIndex)((operand & 0x1C00) >> 10);
            int index_bits = ((operand & 0x0300) >> 8);
            dest_address = R[(int)r2];
        }

        private void BitPatternSTO_IndirectOffset(ushort operand, out ushort dest_address, out RegGPIndex source)
        {
            // int AddressMode = (operand & 0x0007);
            source = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex r2 = (RegGPIndex)((operand & 0x1C00) >> 10);
            int index_bits = ((operand & 0x0300) >> 8);
            ushort nextword = ReadMemInt16(PC, true);
            dest_address = (ushort)(R[(int)r2] + nextword);
            PC += 2; // advance PC two bytes because we're reading a 16-bit immediate value.
        }

        private void BitPatternSTO_IndirectPostInc(ushort operand, out ushort dest_address, out RegGPIndex source)
        {
            // int AddressMode = (operand & 0x0007);
            source = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex r2 = (RegGPIndex)((operand & 0x1C00) >> 10);
            int index_bits = ((operand & 0x0300) >> 8);
            dest_address = R[(int)r2];
            R[(int)r2] += 2; // post increment by data size in bytes (16-bit = 2 bytes).
        }

        private void BitPatternSTO_IndirectPreDec(ushort operand, out ushort dest_address, out RegGPIndex source)
        {
            // int AddressMode = (operand & 0x0007);
            source = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex r2 = (RegGPIndex)((operand & 0x1C00) >> 10);
            int index_bits = ((operand & 0x0300) >> 8);
            R[(int)r2] -= 2; // pre decrement by data size in bytes (16-bit = 2 bytes).
            dest_address =R[(int)r2];
        }

        private void BitPatternSTO_IndirectIndexed(ushort operand, out ushort dest_address, out RegGPIndex source)
        {
            // int AddressMode = (operand & 0x0007);
            source = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex r2 = (RegGPIndex)((operand & 0x1C00) >> 10);
            int index_bits = ((operand & 0x0300) >> 8);
            dest_address = (ushort)(R[(int)r2] + R[index_bits]);
        }

        private void BitPatternSTO_IndirectIndexedHi(ushort operand, out ushort dest_address, out RegGPIndex source)
        {
            // int AddressMode = (operand & 0x0007);
            source = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex r2 = (RegGPIndex)((operand & 0x1C00) >> 10);
            int index_bits = ((operand & 0x0300) >> 8);
            dest_address = (ushort)(R[(int)r2] + R[index_bits + 4]);
        }
        #endregion

        #region YCPU 8bit ALU/STO
        private void BitPatternAL8_Immediate(ushort operand, out ushort value, out RegGPIndex destination)
        {
            // int AddressMode = (operand & 0x0007);
            destination = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex source = (RegGPIndex)((operand & 0x1C00) >> 10);
            int index_bits = ((operand & 0x0300) >> 8);
            ushort nextword = ReadMemInt16(PC, true);
            if ((operand & 0x0100) != 0x0000)
                nextword = ReadMemInt16(nextword, false);
            value = (ushort)(nextword & 0x00FF);
            PC += 2; // advance PC two bytes because we're reading an immediate value.
        }

        private void BitPatternAL8_Register(ushort operand, out ushort value, out RegGPIndex destination)
        {
            // int AddressMode = (operand & 0x0007);
            destination = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex source = (RegGPIndex)((operand & 0x1C00) >> 10);
            int index_bits = ((operand & 0x0300) >> 8);
            value = (ushort)(R[(int)source] & 0x00FF);
        }

        private void BitPatternAL8_Indirect(ushort operand, out ushort value, out RegGPIndex destination)
        {
            // int AddressMode = (operand & 0x0007);
            destination = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex source = (RegGPIndex)((operand & 0x1C00) >> 10);
            int index_bits = ((operand & 0x0300) >> 8);
            value = ReadMemInt8(R[(int)source]);
        }

        private void BitPatternAL8_IndirectOffset(ushort operand, out ushort value, out RegGPIndex destination)
        {
            // int AddressMode = (operand & 0x0007);
            destination = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex source = (RegGPIndex)((operand & 0x1C00) >> 10);
            int index_bits = ((operand & 0x0300) >> 8);
            ushort nextword = ReadMemInt16(PC, true);
            value = ReadMemInt8((ushort)(R[(int)source] + nextword));
            PC += 2; // advance PC two bytes because we're reading an immediate value.
        }

        private void BitPatternAL8_IndirectPostInc(ushort operand, out ushort value, out RegGPIndex destination)
        {
            // int AddressMode = (operand & 0x0007);
            destination = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex source = (RegGPIndex)((operand & 0x1C00) >> 10);
            int index_bits = ((operand & 0x0300) >> 8);
            value = ReadMemInt8(R[(int)source]);
            R[(int)source] += 1; // post increment by data size in bytes (8-bit = 1 byte).
        }

        private void BitPatternAL8_IndirectPreDec(ushort operand, out ushort value, out RegGPIndex destination)
        {
            // int AddressMode = (operand & 0x0007);
            destination = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex source = (RegGPIndex)((operand & 0x1C00) >> 10);
            int index_bits = ((operand & 0x0300) >> 8);
            R[(int)source] -= 1; // pre decrement by data size in bytes (8-bit = 1 byte).
            value = ReadMemInt8(R[(int)source]);
        }

        private void BitPatternAL8_IndirectIndexed(ushort operand, out ushort value, out RegGPIndex destination)
        {
            // int AddressMode = (operand & 0x0007);
            destination = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex source = (RegGPIndex)((operand & 0x1C00) >> 10);
            int index_bits = ((operand & 0x0300) >> 8);
            value = ReadMemInt8((ushort)(R[(int)source] + R[index_bits]));
        }

        private void BitPatternAL8_IndirectIndexedHi(ushort operand, out ushort value, out RegGPIndex destination)
        {
            // int AddressMode = (operand & 0x0007);
            destination = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex source = (RegGPIndex)((operand & 0x1C00) >> 10);
            int index_bits = ((operand & 0x0300) >> 8);
            value = ReadMemInt8((ushort)(R[(int)source] + R[index_bits + 4]));
        }

        private void BitPatternST8_Immediate(ushort operand, out ushort dest_address, out RegGPIndex source)
        {
            // int AddressMode = (operand & 0x0007);
            source = (RegGPIndex)((operand & 0xE000) >> 13);
            if ((operand & 0x0100) == 0x0100)
            {
                // absolute store
                ushort nextword = ReadMemInt16(PC, true);
                dest_address = nextword;
                PC += 2; // advance PC two bytes because we're reading a 16-bit immediate value.
            }
            else
            {
                // immediate store, no write.
                source = RegGPIndex.Error;
                dest_address = 0xffff;
            }
        }

        private void BitPatternST8_Indirect(ushort operand, out ushort dest_address, out RegGPIndex source)
        {
            // int AddressMode = (operand & 0x0007);
            source = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex r2 = (RegGPIndex)((operand & 0x1C00) >> 10);
            int index_bits = ((operand & 0x0300) >> 8);
            dest_address = R[(int)r2];
        }

        private void BitPatternST8_IndirectOffset(ushort operand, out ushort dest_address, out RegGPIndex source)
        {
            // int AddressMode = (operand & 0x0007);
            source = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex r2 = (RegGPIndex)((operand & 0x1C00) >> 10);
            int index_bits = ((operand & 0x0300) >> 8);
            ushort nextword = ReadMemInt16(PC, true);
            dest_address = (ushort)(R[(int)r2] + nextword);
            PC += 2; // advance PC two bytes because we're reading a 16-bit immediate value.
        }

        private void BitPatternST8_IndirectPostInc(ushort operand, out ushort dest_address, out RegGPIndex source)
        {
            // int AddressMode = (operand & 0x0007);
            source = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex r2 = (RegGPIndex)((operand & 0x1C00) >> 10);
            int index_bits = ((operand & 0x0300) >> 8);
            dest_address = R[(int)r2];
            R[(int)r2] += 1; // post increment by data size in bytes (8-bit = 1 bytes).
        }

        private void BitPatternST8_IndirectPreDec(ushort operand, out ushort dest_address, out RegGPIndex source)
        {
            // int AddressMode = (operand & 0x0007);
            source = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex r2 = (RegGPIndex)((operand & 0x1C00) >> 10);
            int index_bits = ((operand & 0x0300) >> 8);
            R[(int)r2] -= 1; // pre decrement by data size in bytes (8-bit = 1 bytes).
            dest_address = R[(int)r2];
        }

        private void BitPatternST8_IndirectIndexed(ushort operand, out ushort dest_address, out RegGPIndex source)
        {
            // int AddressMode = (operand & 0x0007);
            source = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex r2 = (RegGPIndex)((operand & 0x1C00) >> 10);
            int index_bits = ((operand & 0x0300) >> 8);
            dest_address = (ushort)(R[(int)r2] + R[index_bits]);
        }

        private void BitPatternST8_IndirectIndexedHi(ushort operand, out ushort dest_address, out RegGPIndex source)
        {
            // int AddressMode = (operand & 0x0007);
            source = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex r2 = (RegGPIndex)((operand & 0x1C00) >> 10);
            int index_bits = ((operand & 0x0300) >> 8);
            dest_address = (ushort)(R[(int)r2] + R[index_bits + 4]);
        }
        #endregion

        private void BitPatternBIT(ushort operand, out ushort value, out RegGPIndex destination)
        {
            destination = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex source = (RegGPIndex)((operand & 0x1C00) >> 10);
            bool as_register = ((operand & 0x0100) != 0);
            value = (as_register) ?
                (ushort)(R[(int)source] & 0x000F) :
                (ushort)((operand & 0x1E00) >> 9);
        }

        private void BitPatternBRA(ushort operand, out ushort value, out RegGPIndex destination)
        {
            destination = RegGPIndex.R0; // not used
            value = (ushort)((operand & 0xFF00) >> 8);
        }

        private void BitPatternFLG(ushort operand, out ushort value, out RegGPIndex destination)
        {
            destination = RegGPIndex.R0; // unused
            value = (ushort)((operand & 0xF000)); // flags to set
        }

        private void BitPatternFPU(ushort operand, out ushort value, out RegGPIndex destination)
        {
            destination = (RegGPIndex)((operand & 0xE000) >> 13);
            value = (ushort)((operand & 0x1C00) >> 10);
        }

        private void BitPatternHWQ(ushort operand, out ushort value, out RegGPIndex destination)
        {
            destination = RegGPIndex.R0; // Unused.
            value = (ushort)((operand & 0xFF00) >> 8);
        }

        private void BitPatternINC(ushort operand, out ushort value, out RegGPIndex destination)
        {
            destination = (RegGPIndex)((operand & 0xE000) >> 13);
            value = (ushort)(((operand & 0x1F00) >> 8) + 1);
        }

        private void BitPatternJMP(ushort operand, out ushort value, out RegGPIndex destination)
        {
            int AddressMode = ((operand & 0xE000) >> 13);
            RegGPIndex source = (RegGPIndex)((operand & 0x1C00) >> 10);
            int index_bits = ((operand & 0x0300) >> 8);
            destination = RegGPIndex.R0; // unused.
            ushort nextword;

            switch (AddressMode)
            {
                case 0: // Immediate
                    nextword = ReadMemInt16(PC, true);
                    PC += 2; // advance PC two bytes because we're reading an immediate value.
                    if ((operand & 0x0100) != 0x0000)
                        nextword = ReadMemInt16(nextword, false);
                    value = nextword;

                    break;
                case 1: // Register
                    value = R[(int)source];
                    break;
                case 2: // Indirect
                    value = ReadMemInt16(R[(int)source]);
                    break;
                case 3: // Indirect Offset (also Absolute Offset)
                    nextword = ReadMemInt16(PC, true);
                    PC += 2; // advance PC two bytes because we're reading an immediate value.
                    value = ReadMemInt16((ushort)(R[(int)source] + nextword));
                    break;
                case 4: // Indirect PostInc
                    value = ReadMemInt16(R[(int)source]);
                    R[(int)source] += 2; // post increment by data size in bytes (16-bit = 2 bytes).
                    break;
                case 5: // Indirect PreDec
                    R[(int)source] -= 2; // pre decrement by data size in bytes (16-bit = 2 bytes).
                    value = ReadMemInt16(R[(int)source]);
                    break;
                case 6: // Indirect Indexed
                    value = ReadMemInt16((ushort)(R[(int)source] + R[index_bits]));
                    break;
                case 7:
                    value = ReadMemInt16((ushort)(R[(int)source] + R[index_bits + 4]));
                    break;
                default:
                    // can't ever get here.
                    value = 0xDEAD;
                    break;
            }
        }

        private void BitPatternMMU(ushort operand, out ushort mmuIndex, out RegGPIndex regValue)
        {
            regValue = (RegGPIndex)((operand & 0xE000) >> 13);
            mmuIndex = (ushort)((operand & 0x1C) >> 10);
        }

        private void BitPatternPSH(ushort operand, out ushort value, out RegGPIndex destination)
        {
            destination = RegGPIndex.R0; // unused
            value = (ushort)(operand & 0xFF01);
        }

        private void BitPatternSHF(ushort operand, out ushort value, out RegGPIndex destination)
        {
            destination = (RegGPIndex)((operand & 0xE000) >> 13);
            if ((operand & 0x1000) == 0)
                value = (ushort)(((operand & 0x0F00) >> 8));
            else
                value = R[(operand & 0x0700) >> 8];
        }

        private void BitPatternSWO(ushort operand, out ushort value, out RegGPIndex destination)
        {
            destination = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex source = (RegGPIndex)((operand & 0x1C) >> 10);
            int operation = (operand & 0x0300) >> 8;

            switch (operation)
            {
                case 0x00: // src low octet -> dest low octet, clear dest upper 8 bits.
                    value = (ushort)(R[(int)source] & 0x00FF);
                    break;
                case 0x01: // src high octect -> dest low octet, clear dest upper 8 bits.
                    value = (ushort)((R[(int)source] & 0xFF00) >> 8);
                    break;
                case 0x02: // src low octect -> dest low octet, preserves dest upper 8 bits.
                    value = (ushort)((R[(int)source] & 0x00FF) | (R[(int)destination] & 0xFF00));
                    break;
                case 0x03: // src low octect -> dest high octect, preserves dest lower 8 bits.
                    value = (ushort)(((R[(int)source] & 0x00FF) << 8) | (R[(int)destination] & 0x00FF));
                    break;
                default:
                    value = 0xDEAD;
                    break;
            }
        }

        private void BitPatternTSR(ushort operand, out ushort value, out RegGPIndex destination)
        {
            destination = (RegGPIndex)((operand & 0xE000) >> 13);
            value = (ushort)(((operand & 0x1F00) >> 8));
        }
    }
}
