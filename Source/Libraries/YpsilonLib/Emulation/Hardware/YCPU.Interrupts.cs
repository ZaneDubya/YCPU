using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Emulation.Hardware
{
    partial class YCPU
    {
        private void Interrupt(ushort interrupt_number)
        {
            // If this is not a reset interrupt, we should save PS and PC.
            if (interrupt_number != 0x00)
            {
                ushort ps = PS;
                PS_S = true;
                PS_Q = (interrupt_number == 0x0C);
                StackPush(ps);
                // rewind the instruction if this is an error interrupt ($02 - $07)
                if (interrupt_number >= 0x02 && interrupt_number <= 0x07)
                    StackPush((ushort)(PC - SizeOfLastInstruction(PC)));
                else
                    StackPush(PC);
            }
            PC = ReadMemInt16((ushort)(IA + interrupt_number * 2));
            m_Cycles += 31;
        }

        private void ReturnFromInterrupt()
        {
            PC = StackPop();
            PS = StackPop();
            m_PS &= 0xF0FF; // clear Q, U, W, and E.
        }

        public void Interrupt_Reset()
        {
            m_RTC.DisableInterrupt();
            PS = 0x8000;
            IA = 0x0000;
            Interrupt(0x00);
        }

        public void Interrupt_Clock()
        {
            Interrupt(0x01);
        }

        private void Interrupt_DivideByZero()
        {
            Interrupt(0x02);
        }

        private void Interrupt_FPUError()
        {
            Interrupt(0x03);
        }

        private void Interrupt_StackFault()
        {
            Interrupt(0x04);
        }

        private void Interrupt_BankFault(bool execute)
        {
            if (execute)
                m_ExecuteFail = true;   // the processor loop will skip the next instruction (which will be 0x0000).
            // then, it will load the next instruction, with PC = InterruptTable[0x05];
            Interrupt(0x05);
        }

        private void Interrupt_UnPrivOpcode()
        {
            Interrupt(0x06);
        }

        private void Interupt_UndefOpcode()
        {
            Interrupt(0x07);
        }

        public void Interrupt_HWI()
        {
            PS_Q = true;
            II = m_Bus.II;
            m_Bus.AcknowledgeIRQ(II);
            Interrupt(0x0C);
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
