using System;

namespace Ypsilon.Emulation.Hardware
{
    partial class YCPU
    {
        /// <summary>
        /// Executes an ALU operation.
        /// </summary>
        /// <param name="operand">Input: machine code word</param>
        /// <param name="value">Output: result value of operation</param>
        /// <param name="destination">Output: index of general register result should be written to.</param>
        void BitPatternALU(ushort operand, out ushort value, out RegGPIndex destination)
        {
            int addressingMode;
            RegGPIndex source;
            bool eightBitMode;
            ushort address;

            // Decode the operand word's constituent bits.             FEDC BA98 7654 3210                             
            //                                                         AAAA rrrE OOOO ORRR
            addressingMode = (operand & 0xF000) >> 12;              // A = addressing mode
            source = (RegGPIndex)((operand & 0x0E00) >> 9);         // r = source register
            eightBitMode = (operand & 0x0100) != 0;                 // E = 8-bit mode
            destination = (RegGPIndex)(operand & 0x0007);           // R = destination register
            
            switch (addressingMode) // will always be between 0x0 and 0xf
            {
                case 0: // Immediate (r == 0) or Absolute (r == 1)
                    if ((int)source == 0)
                    {
                        value = eightBitMode ? ReadMemInt8(PC, true) : ReadMemInt16(PC, true);
                    }
                    else
                    {
                        address = ReadMemInt16(PC, true);
                        value = eightBitMode ? ReadMemInt8(address) : ReadMemInt16(address);
                    }
                    PC += 2; // advance PC two bytes because we're reading an immediate value.
                    break;

                case 1: // Processor Register
                    value = ReadStatusRegister((RegSPIndex)source);
                    break;

                case 2: // Register
                    value = R[(int)source];
                    break;

                case 3: // Indirect
                    value = eightBitMode ? ReadMemInt8(R[(int)source]) : ReadMemInt16(R[(int)source]);
                    break;

                case 4: // Absolute Offset AKA Indirect Offset
                    address = (ushort)(R[(int)source] + ReadMemInt16(PC, true));
                    value = eightBitMode ? ReadMemInt8(address) : ReadMemInt16(address);
                    PC += 2; // advance PC two bytes because we're reading an immediate value.
                    break;

                case 5: // Stack offset.
                    address = (ushort)(SP + (int)source * 2);
                    value = eightBitMode ? ReadMemInt8(address) : ReadMemInt16(address);
                    break;

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

                default: // addressing of 0x8 ~ 0xF is an Indirect Indexed operation.
                    int indexRegister = ((operand & 0x7000) >> 12);
                    address = (ushort)(R[(int)source] + R[indexRegister]);
                    value = eightBitMode ? ReadMemInt8(address) : ReadMemInt16(address);
                    break;
            }
        }

        /// <summary>
        /// Executes a STOre operation (same bit pattern as ALU, but writes a value from r0 to destination).
        /// </summary>
        /// <param name="operand">Input: machine code word</param>
        /// <param name="destAddress">Output: memory address that is the destination of the operation</param>
        /// <param name="source">Output: general register that is the source of the operation</param>
        void BitPatternSTO(ushort operand, out ushort destAddress, out RegGPIndex source)
        {
            int addressingMode;
            bool eightBitMode;
            RegGPIndex addrRegister;

            // Decode the operand word's constituent bits.             FEDC BA98 7654 3210                             
            //                                                         AAAA rrrE OOOO ORRR
            addressingMode = (operand & 0xF000) >> 12;              // A = addressing mode
            addrRegister = (RegGPIndex)((operand & 0x0E00) >> 9);   // r = address of destination register
            eightBitMode = (operand & 0x0100) != 0;                 // E = 8-bit mode
            source = (RegGPIndex)(operand & 0x0007);                // R = source register

            switch (addressingMode) // will always be between 0x0 and 0xf
            {
                case 0: // Immediate (r == 0) or Absolute (r == 1)
                    if ((int)addrRegister == 0)
                    {
                        // Immediate - no such addressing mode for STO.
                        source = RegGPIndex.None;
                        destAddress = 0;
                        Interupt_UndefOpcode();
                    }
                    else
                    {
                        destAddress = ReadMemInt16(PC, true);
                    }
                    PC += 2; // advance PC two bytes because we're reading an immediate value.
                    break;

                case 1: // Processor Register
                    WriteStatusRegister((RegSPIndex)addrRegister, R[(int)source]);
                    // set source = none so calling function doesn't attempt to interpret this as well.
                    source = RegGPIndex.None;
                    destAddress = 0;
                    break;

                case 2: // Register - no such addressing mode for STO.
                    source = RegGPIndex.None;
                    destAddress = 0;
                    Interupt_UndefOpcode();
                    break;

                case 3: // Indirect
                    destAddress = R[(int)addrRegister];
                    break;

                case 4: // Absolute Offset AKA Indirect Offset
                    destAddress = (ushort)(R[(int)addrRegister] + ReadMemInt16(PC, true));
                    PC += 2; // advance PC two bytes because we're reading an immediate value.
                    break;

                case 5: // Stack offset
                    destAddress = (ushort)(SP + (int)addrRegister * 2);
                    break;

                case 6: // Indirect PostInc.
                    destAddress = R[(int)addrRegister];
                    R[(int)addrRegister] += (ushort)(eightBitMode ? 1 : 2); // post increment by data size in bytes (16-bit = 2 bytes).
                    break;

                case 7: // Indirect PreDec.
                    R[(int)addrRegister] -= (ushort)(eightBitMode ? 1 : 2); // pre decrement by data size in bytes (16-bit = 2 bytes).
                    destAddress = R[(int)addrRegister];
                    break;

                default: // $8-$F are Indirect Indexed operations.
                    int indexRegister = ((operand & 0x7000) >> 12);
                    destAddress = (ushort)(R[(int)source] + R[indexRegister]);
                    break;
            }
        }

        void BitPatternBRA(ushort operand, out ushort value, out RegGPIndex destination)
        {
            destination = RegGPIndex.None; // not used
            value = (ushort)((operand & 0xFF00) >> 7); // (shift 8 - 1) to multiply result by two, per spec.
        }

        void BitPatternBTT(ushort operand, out ushort value, out RegGPIndex destination)
        {
            destination = (RegGPIndex)((operand & 0xE000) >> 13);
            RegGPIndex source = (RegGPIndex)((operand & 0x1C00) >> 10);
            bool as_register = ((operand & 0x0100) != 0);
            value = (as_register) ?
                (ushort)(R[(int)source] & 0x000F) :
                (ushort)((operand & 0x1E00) >> 9);
        }

        void BitPatternFLG(ushort operand, out ushort value, out RegGPIndex destination)
        {
            destination = RegGPIndex.None; // unused
            value = (ushort)((operand & 0xF000)); // flags to set
        }

        void BitPatternHWQ(ushort operand, out ushort value, out RegGPIndex destination)
        {
            destination = RegGPIndex.None; // Unused.
            value = (ushort)((operand & 0xFF00) >> 8);
        }

        void BitPatternIMM(ushort operand, out ushort value, out RegGPIndex destination)
        {
            destination = (RegGPIndex)((operand & 0xE000) >> 13);
            value = (ushort)(((operand & 0x1F00) >> 8) + 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operand">Input: 16-bit machine code word</param>
        /// <param name="value">Output: Value that PC should be set to</param>
        /// <param name="unused">Output: Unused</param>
        void BitPatternJMI(ushort operand, out ushort address, out ushort addressFar, out bool isFarJump)
        {
            int addressingMode;
            RegGPIndex source;
            ushort nextword;

            // Decode the operand word's constituent bits.             FEDC BA98 7654 3210                             
            //                                                         AAAA rrrF OOOO ORRR
            addressingMode = (operand & 0xF000) >> 12;              // A = addressing mode
            source = (RegGPIndex)((operand & 0x0E00) >> 9);         // r = source register
            isFarJump = (operand & 0x0100) != 0;                    // F = far jump mode
            addressFar = 0;

            switch (addressingMode) // will always be between 0x0 and 0xf
            {
                case 0: // Immediate (r == 0) or Absolute (r == 1)
                    if ((int)source == 0)
                    {
                        address = ReadMemInt16(PC, true);
                        if (isFarJump)
                        {
                            PC += 2;
                            addressFar = ReadMemInt16(PC, true);
                        }
                    }
                    else
                    {
                        nextword = ReadMemInt16(PC, true);
                        address = ReadMemInt16(nextword);
                        if (isFarJump)
                            addressFar = ReadMemInt16((ushort)(nextword + 2));
                    }
                    PC += 2; // advance PC two bytes because we're reading an immediate value.
                    break;

                case 1: // DOES NOT EXIST
                    address = PC;
                    Interupt_UndefOpcode();
                    break;

                case 2: // Register
                    address = R[(int)source];
                    if (isFarJump)
                        addressFar = R[(((int)source) + 1) & 0x0007];
                    break;

                case 3: // Indirect
                    address = ReadMemInt16(R[(int)source]);
                    if (isFarJump)
                        addressFar = ReadMemInt16((ushort)(R[(int)source] + 2));
                    break;

                case 4: // Indirect Offset AKA Absolute Offset
                    nextword = ReadMemInt16(PC, true);
                    PC += 2; // advance PC two bytes because we're reading an immediate value.
                    address = ReadMemInt16((ushort)(R[(int)source] + nextword));
                    if (isFarJump)
                        addressFar = ReadMemInt16((ushort)(R[(int)source] + nextword + 2));
                    break;

                case 5: // Stack offset
                    int sp = SP + (int)source * 2;
                    address = ReadMemInt16((ushort)sp);
                    if (isFarJump)
                        addressFar = ReadMemInt16((ushort)(sp + 2));
                    break;

                case 6: // Indirect PostInc
                    address = ReadMemInt16(R[(int)source]);
                    R[(int)source] += 2; // post increment by data size in bytes (16-bit = 2 bytes).
                    if (isFarJump)
                    {
                        addressFar = ReadMemInt16(R[(int)source]);
                        R[(int)source] += 2; // post increment by data size in bytes (16-bit = 2 bytes).
                    }
                    break;

                case 7: // Indirect PreDec
                    R[(int)source] -= 2; // pre decrement by data size in bytes (16-bit = 2 bytes).
                    address = ReadMemInt16(R[(int)source]);
                    if (isFarJump)
                    {
                        R[(int)source] -= 2; // pre decrement by data size in bytes (16-bit = 2 bytes).
                        addressFar = ReadMemInt16(R[(int)source]);
                    }
                    break;

                default: // Indirect Indexed
                    int indexRegister = (operand & 0x7000) >> 12;
                    address = ReadMemInt16((ushort)(R[(int)source] + R[indexRegister]));
                    if (isFarJump)
                        addressFar = ReadMemInt16((ushort)(R[(int)source] + R[indexRegister] + 2));
                    break;
            }
        }

        /// <summary>
        /// Retrieves Ra and Rv from an MMU operand. These are BOTH register indexes.
        /// </summary>
        /// <param name="operand"></param>
        void BitPatternMMU(ushort operand, out RegGPIndex regMMUIndex, out RegGPIndex regValue)
        {
            regValue = (RegGPIndex)((operand & 0xE000) >> 13);
            regMMUIndex = (RegGPIndex)((operand & 0x1C00) >> 10);
        }

        void BitPatternSET(ushort operand, out ushort value, out RegGPIndex destination)
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

        void BitPatternSHF(ushort operand, out ushort value, out RegGPIndex destination)
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

        void BitPatternSTK(ushort operand, out ushort value, out RegGPIndex destination)
        {
            destination = RegGPIndex.None; // unused
            value = (ushort)(operand & 0xFF01);
        }
    }
}
