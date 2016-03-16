using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Emulation.Processor
{
    partial class YCPU
    {
        private void Interrupt(ushort interrupt_number, params ushort[] stack_values)
        {
            // !!! Must handle stack_values
            // If this is not a reset interrupt, we should save PS and PC.
            if (interrupt_number != 0x00)
            {
                ushort ps = PS;
                PS_S = true;
                PS_I = true;
                PS_Q = (interrupt_number == 0x0C);
                StackPush(ps);
                // rewind the instruction if this is an error interrupt ($02 - $07)
                if (interrupt_number >= 0x02 && interrupt_number <= 0x07)
                    StackPush((ushort)(PC - SizeOfLastInstruction(PC)));
                else
                    StackPush(PC);
            }
            PC = ReadMemInt16((ushort)(interrupt_number * 2), SegmentIndex.IS);
            m_Cycles += 31;
        }

        private void ReturnFromInterrupt()
        {
            PC = StackPop();
            PS = StackPop();
            m_PS &= 0xF0FF; // clear Q, U, W.
        }

        public void Interrupt_Reset()
        {
            m_RTC.DisableInterrupt();
            PS = 0x8000;
            Interrupt(0x00);
        }

        public void Interrupt_Clock()
        {
            Interrupt(0x01);
        }

        private void Interrupt_DivZeroFault()
        {
            Interrupt(0x02);
        }

        private void Interrupt_DoubleFault()
        {
            Interrupt(0x03);
        }

        private void Interrupt_StackFault()
        {
            Interrupt(0x04);
        }

        internal void Interrupt_SegFault(SegmentIndex segmentType)
        {
            if (segmentType == SegmentIndex.CS || segmentType == SegmentIndex.IS)
            {
                m_ExecuteFail = true;   // the processor loop will skip the next instruction (which will be 0x0000).
                Interrupt(0x05);        // then, it will load the next instruction, with PC = InterruptTable[0x05];
            }
            else if (segmentType == SegmentIndex.DS || segmentType == SegmentIndex.ES)
            {
                m_ExecuteFail = false;
                Interrupt(0x05);
            }
            else if (segmentType == SegmentIndex.SS)
            {
                m_ExecuteFail = false;
                Interrupt(0x04);
            }
        }

        private void Interrupt_UnPrivFault()
        {
            Interrupt(0x06);
        }

        private void Interrupt_UndefFault()
        {
            Interrupt(0x07);
        }

        public void Interrupt_HWI()
        {
            PS_Q = true;
            ushort irq_index = m_Bus.FirstIRQ;
            m_Bus.AcknowledgeIRQ(irq_index);
            Interrupt(0x0C, irq_index);
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
    }
}
