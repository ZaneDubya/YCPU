namespace Ypsilon.Emulation.Processor {
    partial class YCPU {
        public void Interrupt_BusRefresh() {
            Interrupt(Interrupts.BusRefresh);
        }

        public void Interrupt_Clock() {
            Interrupt(Interrupts.Clock);
        }

        public void Interrupt_DebugQuery() {
            Interrupt(Interrupts.DebugQuery);
        }

        public void Interrupt_HWI() {
            PS_Q = true;
            ushort irq_index = BUS.FirstIRQ;
            BUS.AcknowledgeIRQ(irq_index);
            Interrupt(Interrupts.HWI, irq_index);
        }

        public void Interrupt_Reset() {
            m_RTC.DisableInterrupt();
            PS = 0x8000;
            Interrupt(Interrupts.Reset);
        }

        internal void Interrupt_SegFault(SegmentIndex segmentType, ushort opcode, ushort address) {
            if (segmentType == SegmentIndex.CS) {
                Interrupt(Interrupts.SegFault, opcode);
            }
            else if (segmentType == SegmentIndex.IS) {
                Interrupt_DoubleFault(opcode);
            }
            else if (segmentType == SegmentIndex.DS || segmentType == SegmentIndex.ES) {
                Interrupt(Interrupts.SegFault, opcode);
            }
            else if (segmentType == SegmentIndex.SS) {
                Interrupt(Interrupts.StackFault, opcode);
            }
        }

        public void Interrupt_SWI() {
            Interrupt(Interrupts.SWI);
        }

        private void Interrupt(Interrupts interrupt, ushort? errorCode = null) {
            // !!! Must handle stack_values
            // If this is not a reset interrupt, we should save PS and PC.
            if (interrupt != Interrupts.Reset) {
                ushort ps = PS;
                if (Interrupt_IsFault(interrupt))
                    PS_U = !PS_S;
                PS_S = true;
                PS_I = true;
                PS_Q = interrupt == Interrupts.HWI;
                StackPush(0xffff, ps);
                StackPush(0xffff, PC);
                if (errorCode != null)
                    StackPush(0xffff, errorCode.Value);
            }
            PC = ReadMemInt16((ushort)((ushort)interrupt * 2), SegmentIndex.IS);
            Cycles += 7;
        }

        private void Interrupt_DivZeroFault(ushort opcode) {
            Interrupt(Interrupts.DivZeroFault, opcode);
        }

        private void Interrupt_DoubleFault(ushort opcode) {
            Interrupt(Interrupts.DoubleFault, opcode);
        }

        private bool Interrupt_IsFault(Interrupts interrupt) {
            switch (interrupt) {
                case Interrupts.DivZeroFault:
                case Interrupts.DoubleFault:
                case Interrupts.StackFault:
                case Interrupts.SegFault:
                case Interrupts.UnPrivFault:
                case Interrupts.UndefFault:
                    return true;
                default:
                    return false;
            }
        }

        private void Interrupt_StackFault(ushort opcode, ushort address) {
            Interrupt(Interrupts.StackFault, opcode);
        }

        private void Interrupt_UndefFault(ushort opcode) {
            Interrupt(Interrupts.UndefFault, opcode);
        }

        private void Interrupt_UnPrivFault(ushort opcode) {
            Interrupt(Interrupts.UnPrivFault, opcode);
        }

        private void ReturnFromInterrupt() {
            PC = StackPop(0xffff);
            PS = StackPop(0xffff);
            m_PS &= 0xF0FF; // clear Q, U, W.
        }

        private void TripleFault() {
            Interrupt_Reset();
        }

        private enum Interrupts {
            Reset,
            Clock,
            DivZeroFault,
            DoubleFault,
            StackFault,
            SegFault,
            UnPrivFault,
            UndefFault,
            Reserved8,
            Reserved9,
            ReservedA,
            ReservedB,
            HWI,
            BusRefresh,
            DebugQuery,
            SWI
        }
    }
}