using System;

namespace Ypsilon.Hardware.Processor
{
    partial class YCPU
    {
        #region YCPU ALU (includes STO)
        private void BitPatternALU(ushort operand, out ushort value, out RegGPIndex destination)
        {
            // FEDC BA98 7654 3210                             
            // rrrE AAAA OOOO ORRR

            // R = destination register
            destination = (RegGPIndex)(operand & 0x0007);
            // r = source register
            RegGPIndex source = (RegGPIndex)((operand & 0xE000) >> 13);
            // E = 8-bit mode
            bool eightBitMode = (operand & 0x1000) != 0;
            // A = addressing mode
            int addressingMode = (operand & 0x0F00) >> 8;

            ushort nextword;
            switch (addressingMode)
            {
                case 0: // Immediate
                    value = ReadMemInt16(PC, true);
                    PC += 2; // advance PC two bytes because we're reading an immediate value.
                    break;
                case 1: // Absolute
                    nextword = ReadMemInt16(PC, true);
                    value = ReadMemInt16(nextword, false);
                    PC += 2; // advance PC two bytes because we're reading an immediate value.
                    break;
                case 2: // Register
                    value = R[(int)source];
                    break;
                case 3: // Indirect
                    value = ReadMemInt16(R[(int)source]);
                    break;
                case 4: // Absolute Offset OR Indirect Offset
                    nextword = ReadMemInt16(PC, true);
                    value = ReadMemInt16((ushort)(R[(int)source] + nextword), false);
                    PC += 2; // advance PC two bytes because we're reading an immediate value.
                    break;
                case 5: // Stack offset. Not coded.
                    throw new Exception("Unimplemented stack offset ALU/LOD operation.");
                case 6: // Indirect PostInc.
                    if (eightBitMode)
                    {
                        value = ReadMemInt8(R[(int)source]);
                        R[(int)source] += (ushort)1; // post increment by data size in bytes (8-bit = 1 byte).
                    }
                    else
                    {
                        value = ReadMemInt16(R[(int)source]);
                        R[(int)source] += (ushort)2; // post increment by data size in bytes (16-bit = 2 bytes).
                    }
                    break;
                case 7: // Indirect PreDec.
                    if (eightBitMode)
                    {
                        R[(int)source] -= (ushort)1; // pre decrement by data size in bytes (8-bit = 1 bytes).
                        value = ReadMemInt8(R[(int)source]);
                    }
                    else
                    {
                        R[(int)source] -= (ushort)2; // pre decrement by data size in bytes (16-bit = 2 bytes).
                        value = ReadMemInt16(R[(int)source]);
                    }
                    break;
                default: // $8-$F are Indirect Indexed operations.
                    int index_bits = ((operand & 0x0700) >> 8);
                    value = ReadMemInt16((ushort)(R[(int)source] + R[index_bits]));
                    break;
            }

            if (eightBitMode)
                value &= 0x00FF;
        }

        private void BitPatternSTO(ushort operand, out ushort destAddress, out RegGPIndex source)
        {
            // FEDC BA98 7654 3210                             
            // rrrE AAAA OOOO ORRR

            // R = source register
            source = (RegGPIndex)(operand & 0x0007);
            // r = r2 register
            RegGPIndex r2 = (RegGPIndex)((operand & 0xE000) >> 13);
            // E = 8-bit mode
            bool eightBitMode = (operand & 0x1000) != 0;
            // A = addressing mode
            int addressingMode = (operand & 0x0F00) >> 8;

            ushort nextword;
            switch (addressingMode)
            {
                case 0: // Immediate - no such addressing mode for STO.
                    source = RegGPIndex.Error;
                    destAddress = 0xffff;
                    break;
                case 1: // Absolute
                    nextword = ReadMemInt16(PC, true);
                    destAddress = nextword;
                    PC += 2; // advance PC two bytes because we're reading an immediate value.
                    break;
                case 2: // Register - no such addressing mode for STO.
                    source = RegGPIndex.Error;
                    destAddress = 0xffff;
                    break;
                case 3: // Indirect
                    destAddress = R[(int)r2];
                    break;
                case 4: // Absolute Offset OR Indirect Offset
                    nextword = ReadMemInt16(PC, true);
                    destAddress = (ushort)(R[(int)r2] + nextword);
                    PC += 2; // advance PC two bytes because we're reading an immediate value.
                    break;
                case 5: // Stack offset. Not coded.
                    throw new Exception("Unimplemented stack offset STO operation.");
                case 6: // Indirect PostInc.
                    destAddress = R[(int)r2];
                    R[(int)r2] += (ushort)(eightBitMode ? 1 : 2); // post increment by data size in bytes (16-bit = 2 bytes).
                    break;
                case 7: // Indirect PreDec.
                    R[(int)r2] -= (ushort)(eightBitMode ? 1 : 2); // pre decrement by data size in bytes (16-bit = 2 bytes).
                    destAddress = R[(int)r2];
                    break;
                default: // $8-$F are Indirect Indexed operations.
                    int index_bits = ((operand & 0x0700) >> 8);
                    destAddress = ((ushort)(R[(int)source] + R[index_bits]));
                    break;
            }
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
            mmuIndex = (ushort)((operand & 0x1C00) >> 10);
        }

        private void BitPatternPSH(ushort operand, out ushort value, out RegGPIndex destination)
        {
            destination = RegGPIndex.R0; // unused
            value = (ushort)(operand & 0xFF01);
        }

        private void BitPatternSET(ushort operand, out ushort value, out RegGPIndex destination)
        {
            destination = (RegGPIndex)((operand & 0xE000) >> 13);
            value = (ushort)((operand & 0x1F00) >> 8);
            if ((operand & 0x0001) == 1)
            {
                if (value <= 0x0A)
                    value = (ushort)(0x0001 << (value + 0x05));
                else
                    value = (ushort)(0xFFE0 + value);
            }
        }

        private void BitPatternSHF(ushort operand, out ushort value, out RegGPIndex destination)
        {
            destination = (RegGPIndex)((operand & 0xE000) >> 13);
            if ((operand & 0x1000) == 0)
            {
                value = (ushort)(((operand & 0x0F00) >> 8) + 1);
            }
            else
            {
                value = R[(operand & 0x0700) >> 8];
            }
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
