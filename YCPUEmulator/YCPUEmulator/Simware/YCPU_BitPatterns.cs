using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCPU.Simware
{
    partial class YCPU
    {
        private void BitPatternALU(ushort operand, ushort nextword, out ushort value, out RegGPIndex destination)
        {
            int AddressMode = (operand & 0x0007);
            destination = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex source = (RegGPIndex)((operand & 0x1C00) >> 10);
            int index_bits = ((operand & 0x0300) >> 8);

            switch (AddressMode)
            {
                case 0: // Immediate
                    value = nextword;
                    PC++;
                    break;
                case 1: // Register
                    value = R[(int)source];
                    break;
                case 2: // Indirect
                    value = GetMemory(R[(int)source]);
                    break;
                case 3: // Indirect Offset (also Absolute Offset)
                    value = GetMemory((ushort)(R[(int)source] + nextword));
                    PC++;
                    break;
                case 4: // Indirect PostInc
                    value = GetMemory(R[(int)source]);
                    R[(int)source]++;
                    break;
                case 5: // Indirect PreDec
                    R[(int)source]--;
                    value = GetMemory(R[(int)source]);
                    break;
                case 6: // Indirect Indexed
                    value = GetMemory((ushort)(R[(int)source] + R[index_bits]));
                    break;
                case 7:
                    value = GetMemory((ushort)(R[(int)source] + R[index_bits + 4]));
                    break;
                default:
                    value = 0xDEAD;
                    break;
            }
        }

        private void BitPatternBIT(ushort operand, ushort nextword, out ushort value, out RegGPIndex destination)
        {
            destination = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex source = (RegGPIndex)((operand & 0x1C00) >> 10);
            bool as_register = ((operand & 0x0100) != 0);
            value = (as_register) ?
                (ushort)(R[(int)source] & 0x000F) :
                (ushort)((operand & 0x1E00) >> 9);
        }

        private void BitPatternBRA(ushort operand, ushort nextword, out ushort value, out RegGPIndex destination)
        {
            destination = RegGPIndex.R0; // not used
            value = (ushort)((operand & 0xFF00) >> 8);
        }

        private void BitPatternFLG(ushort operand, ushort nextword, out ushort value, out RegGPIndex destination)
        {
            destination = RegGPIndex.R0; // unused
            value = (ushort)((operand & 0xF000)); // flags to set
        }

        private void BitPatternFPU(ushort operand, ushort nextword, out ushort value, out RegGPIndex destination)
        {
            destination = (RegGPIndex)((operand & 0xE000) >> 13);
            value = (ushort)((operand & 0x1C00) >> 10);
        }

        private void BitPatternHWQ(ushort operand, ushort nextword, out ushort value, out RegGPIndex destination)
        {
            destination = RegGPIndex.R0; // Unused.
            value = (ushort)((operand & 0xFF00) >> 8);
        }

        private void BitPatternINC(ushort operand, ushort nextword, out ushort value, out RegGPIndex destination)
        {
            destination = (RegGPIndex)((operand & 0xE000) >> 13);
            value = (ushort)(((operand & 0x1F00) >> 8) + 1);
        }

        private void BitPatternJMP(ushort operand, ushort nextword, out ushort value, out RegGPIndex destination)
        {
            int AddressMode = ((operand & 0xE000) >> 13);
            RegGPIndex source = (RegGPIndex)((operand & 0x1C00) >> 10);
            int index_bits = ((operand & 0x0300) >> 8);
            destination = RegGPIndex.R0; // unused.

            switch (AddressMode)
            {
                case 0: // Immediate
                    value = nextword;
                    PC++;
                    break;
                case 1: // Register
                    value = R[(int)source];
                    break;
                case 2: // Indirect
                    value = GetMemory(R[(int)source]);
                    break;
                case 3: // Indirect Offset (also Absolute Offset)
                    value = GetMemory((ushort)(R[(int)source] + nextword));
                    PC++;
                    break;
                case 4: // Indirect PostInc
                    value = GetMemory(R[(int)source]);
                    R[(int)source]++;
                    break;
                case 5: // Indirect PreDec
                    R[(int)source]--;
                    value = GetMemory(R[(int)source]);
                    break;
                case 6: // Indirect Indexed
                    value = GetMemory((ushort)(R[(int)source] + R[index_bits]));
                    break;
                case 7:
                    value = GetMemory((ushort)(R[(int)source] + R[index_bits + 4]));
                    break;
                default:
                    // can't ever get here.
                    value = 0xDEAD;
                    break;
            }
        }

        private void BitPatternMMU(ushort operand, ushort nextword, out ushort value, out RegGPIndex destination)
        {
            destination = (RegGPIndex)((operand & 0xE000) >> 13);
            value = (ushort)((operand & 0x1C) >> 10);
        }

        private void BitPatternPSH(ushort operand, ushort nextword, out ushort value, out RegGPIndex destination)
        {
            destination = RegGPIndex.R0; // unused
            value = (ushort)(operand & 0xFF03);
        }

        private void BitPatternSHF(ushort operand, ushort nextword, out ushort value, out RegGPIndex destination)
        {
            destination = (RegGPIndex)((operand & 0xE000) >> 13);
            value = (ushort)(((operand & 0x0F00) >> 8));
        }

        private void BitPatternSWO(ushort operand, ushort nextword, out ushort value, out RegGPIndex destination)
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

        private void BitPatternTSR(ushort operand, ushort nextword, out ushort value, out RegGPIndex destination)
        {
            destination = (RegGPIndex)((operand & 0xE000) >> 13);
            value = (ushort)(((operand & 0x1F00) >> 8));
        }
    }
}
