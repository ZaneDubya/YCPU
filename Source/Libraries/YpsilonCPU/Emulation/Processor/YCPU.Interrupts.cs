using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Emulation.Processor
{
    partial class YCPU
    {
        private enum Interrupts
        {
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

        private void Interrupt(Interrupts interrupt, params ushort[] stack_values)
        {
            // !!! Must handle stack_values
            // If this is not a reset interrupt, we should save PS and PC.
            if (interrupt != Interrupts.Reset)
            {
                ushort ps = PS;
                if (Interrupt_IsFault(interrupt))
                    PS_U = !PS_S;
                PS_S = true;
                PS_I = true;
                PS_Q = (interrupt == Interrupts.HWI);
                StackPush(ps);
                StackPush(PC);
            }
            PC = ReadMemInt16((ushort)((ushort)interrupt * 2), SegmentIndex.IS);
            m_Cycles += 31;
        }

        private void ReturnFromInterrupt()
        {
            PC = StackPop();
            PS = StackPop();
            m_PS &= 0xF0FF; // clear Q, U, W.
        }

        private bool Interrupt_IsFault(Interrupts interrupt)
        {
            switch (interrupt)
            {
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

        public void Interrupt_Reset()
        {
            m_RTC.DisableInterrupt();
            PS = 0x8000;
            Interrupt(Interrupts.Reset);
        }

        public void Interrupt_Clock()
        {
            Interrupt(Interrupts.Clock);
        }

        private void Interrupt_DivZeroFault()
        {
            Interrupt(Interrupts.DivZeroFault);
        }

        private void Interrupt_DoubleFault()
        {
            Interrupt(Interrupts.DoubleFault);
        }

        private void Interrupt_StackFault()
        {
            Interrupt(Interrupts.StackFault);
        }

        internal void Interrupt_SegFault(SegmentIndex segmentType)
        {
            if (segmentType == SegmentIndex.CS)
            {
                m_ExecuteFail = true;   // the processor loop will skip the next instruction (which will be 0x0000).
                Interrupt(Interrupts.SegFault);        // then, it will load the next instruction, with PC = InterruptTable[0x05];
            }
            else if (segmentType == SegmentIndex.IS)
            {
                m_ExecuteFail = true;
                Interrupt_DoubleFault();
            }
            else if (segmentType == SegmentIndex.DS || segmentType == SegmentIndex.ES)
            {
                m_ExecuteFail = false;
                Interrupt(Interrupts.SegFault);
            }
            else if (segmentType == SegmentIndex.SS)
            {
                m_ExecuteFail = false;
                Interrupt(Interrupts.StackFault);
            }
        }

        private void Interrupt_UnPrivFault()
        {
            Interrupt(Interrupts.UnPrivFault);
        }

        private void Interrupt_UndefFault()
        {
            Interrupt(Interrupts.UndefFault);
        }

        public void Interrupt_HWI()
        {
            PS_Q = true;
            ushort irq_index = m_Bus.FirstIRQ;
            m_Bus.AcknowledgeIRQ(irq_index);
            Interrupt(Interrupts.HWI, irq_index);
        }

        public void Interrupt_BusRefresh()
        {
            Interrupt(Interrupts.BusRefresh);
        }

        public void Interrupt_DebugQuery()
        {
            Interrupt(Interrupts.DebugQuery);
        }

        public void Interrupt_SWI()
        {
            Interrupt(Interrupts.SWI);
        }

        private void TripleFault()
        {
            Interrupt_Reset();
        }
    }
}
