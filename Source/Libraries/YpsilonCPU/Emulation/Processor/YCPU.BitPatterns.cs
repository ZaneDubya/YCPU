namespace Ypsilon.Emulation.Processor {
    partial class YCPU {
        /// <summary>
        /// Executes an ALU operation.
        /// </summary>
        /// <param name="operand">Input: machine code word</param>
        /// <param name="value">Output: result value of operation</param>
        /// <param name="destination">Output: index of general register result should be written to.</param>
        private void BitPatternALU(ushort operand, out ushort value, out RegGeneral destination) {
            ushort address;

            // Decode the operand word's constituent bits.             FEDC BA98 7654 3210                             
            //                                                         SAAA rrrE OOOO ORRR
            int addressingMode = (operand & 0x7000) >> 12;
            RegGeneral source = (RegGeneral)((operand & 0x0E00) >> 9);
            bool eightBitMode = (operand & 0x0100) != 0;
            destination = (RegGeneral)(operand & 0x0007); // R = destination register
            SegmentIndex dataSeg = (operand & 0x8000) != 0 ? SegmentIndex.ES : SegmentIndex.DS;
            switch (addressingMode) // will always be between 0x0 and 0x7
            {
                case 0: // Addressing mode: Immediate (r == 0), Absolute (r == 1), else Control Register.
                    if (source == 0) // Immediate
                    {
                        value = eightBitMode ? ReadMemInt8(PC, SegmentIndex.CS) : ReadMemInt16(PC, SegmentIndex.CS);
                        PC += 2; // advance PC two bytes because we're reading an immediate value.
                    }
                    else if ((int)source == 1) // Absolute
                    {
                        address = ReadMemInt16(PC, SegmentIndex.CS);
                        PC += 2; // advance PC two bytes because we're reading an immediate value.
                        value = eightBitMode ? ReadMemInt8(address, dataSeg) : ReadMemInt16(address, dataSeg);
                    }
                    else // Control Register
                    {
                        RegControl cr = (RegControl)((operand & 0x0700) >> 8);
                        value = ReadControlRegister(operand, cr);
                    }
                    break;
                case 1: // Addressing mode: Register
                    value = R[(int)source];
                    break;
                case 2: // Addressing mode: Indirect
                    value = eightBitMode ? ReadMemInt8(R[(int)source], dataSeg) : ReadMemInt16(R[(int)source], dataSeg);
                    break;
                case 3: // Addressing mode: Absolute Offset AKA Indirect Offset
                    address = (ushort)(R[(int)source] + ReadMemInt16(PC, SegmentIndex.CS));
                    PC += 2; // advance PC two bytes because we're reading an immediate value.
                    value = eightBitMode ? ReadMemInt8(address, dataSeg) : ReadMemInt16(address, dataSeg);
                    break;
                default: // addressing of 0x4 ~ 0x7 is an Indirect Indexed operation.
                    int indexRegister = addressingMode; // bit pattern is 1ii, where ii = r4 - r7.
                    address = (ushort)(R[(int)source] + R[indexRegister]);
                    value = eightBitMode ? ReadMemInt8(address, dataSeg) : ReadMemInt16(address, dataSeg);
                    break;
            }
        }

        private void BitPatternBRA(ushort operand, out ushort value, out RegGeneral destination) {
            destination = RegGeneral.None; // not used
            value = (ushort)((operand & 0xFF00) >> 7); // (shift 8 - 1) to multiply result by two, per spec.
        }

        private void BitPatternBTI(ushort operand, out ushort value, out RegGeneral destination) {
            destination = (RegGeneral)((operand & 0xE000) >> 13);
            RegGeneral source = (RegGeneral)((operand & 0x1C00) >> 10);
            bool asRegister = (operand & 0x0100) != 0;
            value = asRegister ?
                (ushort)(R[(int)source] & 0x000F) :
                (ushort)((operand & 0x1E00) >> 9);
        }

        private void BitPatternFLG(ushort operand, out ushort value, out RegGeneral destination) {
            destination = RegGeneral.None; // unused
            value = (ushort)(operand & 0xF000); // flags to set
        }

        private void BitPatternHWQ(ushort operand, out ushort value, out RegGeneral destination) {
            destination = RegGeneral.None; // Unused.
            value = (ushort)((operand & 0xFF00) >> 8);
        }

        private void BitPatternIMM(ushort operand, out ushort value, out RegGeneral destination) {
            destination = (RegGeneral)((operand & 0xE000) >> 13);
            value = (ushort)(((operand & 0x1F00) >> 8) + 1);
        }

        private void BitPatternJMI(ushort operand, out ushort address, out uint addressFar, out bool isFarJump) {
            ushort nextword;

            // Decode the operand word's constituent bits.             FEDC BA98 7654 3210                             
            //                                                         SAAA rrrF OOOO ORRR
            int addressingMode = (operand & 0x7000) >> 12;
            RegGeneral source = (RegGeneral)((operand & 0x0E00) >> 9);
            SegmentIndex dataSeg = (operand & 0x8000) != 0 ? SegmentIndex.ES : SegmentIndex.DS;
            addressFar = 0;
            isFarJump = (operand & 0x0100) != 0; // F = far jump mode
            switch (addressingMode) // will always be between 0x0 and 0xf
            {
                case 0: // Immediate (r == 0) or Absolute (r == 1)
                    if (source == 0) {
                        address = ReadMemInt16(PC, SegmentIndex.CS);
                        PC += 2; // advance PC two bytes because we're reading an immediate value.
                        if (isFarJump) {
                            addressFar = ReadMemInt16(PC, SegmentIndex.CS);
                            PC += 2; // advance PC two bytes because we're reading an immediate value.
                            addressFar |= (uint)ReadMemInt16(PC, SegmentIndex.CS) << 16;
                            PC += 2; // advance PC two bytes because we're reading an immediate value.
                        }
                    }
                    else {
                        nextword = ReadMemInt16(PC, SegmentIndex.CS);
                        PC += 2; // advance PC two bytes because we're reading an immediate value.
                        address = ReadMemInt16(nextword, dataSeg);
                        if (isFarJump) {
                            addressFar = ReadMemInt16((ushort)(nextword + 2), dataSeg);
                            addressFar |= (uint)ReadMemInt16((ushort)(nextword + 4), dataSeg) << 16;
                        }
                    }
                    break;
                case 1: // Register
                    address = R[(int)source];
                    break;
                case 2: // Indirect
                    address = ReadMemInt16(R[(int)source], dataSeg);
                    if (isFarJump) {
                        addressFar = ReadMemInt16((ushort)(R[(int)source] + 2), dataSeg);
                        addressFar |= (uint)ReadMemInt16((ushort)(R[(int)source] + 4), dataSeg) << 16;
                    }
                    break;
                case 3: // Indirect Offset AKA Absolute Offset
                    nextword = ReadMemInt16(PC, SegmentIndex.CS);
                    PC += 2; // advance PC two bytes because we're reading an immediate value.
                    address = ReadMemInt16((ushort)(R[(int)source] + nextword), dataSeg);
                    if (isFarJump) {
                        addressFar = ReadMemInt16((ushort)(R[(int)source] + nextword + 2), dataSeg);
                        addressFar |= (uint)ReadMemInt16((ushort)(R[(int)source] + nextword + 4), dataSeg) << 16;
                    }
                    break;
                default: // 0x04 - 0x07 are Indirect Indexed
                    int indexRegister = (operand & 0x7000) >> 12; // bit pattern is 1ii, indicating R4 - R7
                    address = ReadMemInt16((ushort)(R[(int)source] + R[indexRegister]), dataSeg);
                    if (isFarJump) {
                        addressFar = ReadMemInt16((ushort)(R[(int)source] + R[indexRegister] + 2), dataSeg);
                        addressFar |= (uint)ReadMemInt16((ushort)(R[(int)source] + R[indexRegister] + 4), dataSeg) << 16;
                    }
                    break;
            }
        }

        private void BitPatternSET(ushort operand, out ushort value, out RegGeneral destination) {
            destination = (RegGeneral)((operand & 0xE000) >> 13);
            value = (ushort)((operand & 0x1F00) >> 8);
            if ((operand & 0x0001) == 1) {
                if (value <= 0x0A)
                    value = (ushort)(0x0001 << (value + 0x05));
                else
                    value = (ushort)(0xFFE0 + value);
            }
        }

        private void BitPatternSHF(ushort operand, out ushort value, out RegGeneral destination) {
            destination = (RegGeneral)((operand & 0xE000) >> 13);
            if ((operand & 0x1000) == 0) {
                value = (ushort)(((operand & 0x0F00) >> 8) + 1);
            }
            else {
                value = R[(operand & 0x0700) >> 8];
            }
        }

        private void BitPatternSTK(ushort operand, out ushort value, out RegGeneral destination) {
            destination = RegGeneral.None; // unused
            value = (ushort)(operand & 0xFF01);
        }

        /// <summary>
        /// Executes a STOre operation (same bit pattern as ALU, but writes a value from r0 to destination).
        /// </summary>
        /// <param name="operand">Input: machine code word</param>
        /// <param name="destAddress">Output: memory address that is the destination of the operation</param>
        /// <param name="source">Output: general register that is the source of the operation</param>
        private void BitPatternSTO(ushort operand, out ushort destAddress, out RegGeneral source) {
            // Decode the operand word's constituent bits.             FEDC BA98 7654 3210                             
            //                                                         SAAA rrrE OOOO ORRR
            int addressingMode = (operand & 0x7000) >> 12;
            RegGeneral addrRegister = (RegGeneral)((operand & 0x0E00) >> 9);
            source = (RegGeneral)(operand & 0x0007); // R = source register
            switch (addressingMode) // will always be between 0x0 and 0x7
            {
                case 0: // Immediate (r == 0), Absolute (r == 1), else Control Register
                    if (addrRegister == 0) {
                        // Immediate - no such addressing mode for STO.
                        source = RegGeneral.None;
                        destAddress = 0;
                        Interrupt_UndefFault(operand);
                    }
                    else if ((int)addrRegister == 1) {
                        destAddress = ReadMemInt16(PC, SegmentIndex.CS);
                        PC += 2; // advance PC two bytes because we're reading an immediate value.
                    }
                    else {
                        RegControl cr = (RegControl)((operand & 0x0700) >> 8);
                        WriteControlRegister(operand, cr, R[(int)source]);
                        // set source = none so calling function doesn't attempt to interpret this as well.
                        source = RegGeneral.None;
                        destAddress = 0;
                    }
                    break;
                case 1: // Register - no such addressing mode for STO.
                    source = RegGeneral.None;
                    destAddress = 0;
                    Interrupt_UndefFault(operand);
                    break;
                case 2: // Indirect
                    destAddress = R[(int)addrRegister];
                    break;
                case 3: // Absolute Offset AKA Indirect Offset
                    destAddress = (ushort)(R[(int)addrRegister] + ReadMemInt16(PC, SegmentIndex.CS));
                    PC += 2; // advance PC two bytes because we're reading an immediate value.
                    break;
                default: // $8-$F are Indirect Indexed operations.
                    int indexRegister = addressingMode; // bit pattern is 01ii, indicating r4 - r7.
                    destAddress = (ushort)(R[(int)source] + R[indexRegister]);
                    break;
            }
        }
    }
}