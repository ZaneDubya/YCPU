using System;
using Microsoft.Xna.Framework;
using System.Threading;

namespace YCPU
{
    class Emulator : Platform.Host
    {
        private Hardware.YCPU m_CPU;
        private System.Diagnostics.Stopwatch m_Stopwatch;
        private int m_LastRunMS;

        public Emulator()
            : base()
        {
            m_Stopwatch = new System.Diagnostics.Stopwatch();
        }

        private double m_LastConsoleUpdate = 0;

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (m_CPU == null)
            {
                m_CPU = new Hardware.YCPU();
                m_CPU.Interrupt_Reset();
            }

            if (gameTime.TotalGameTime.TotalMilliseconds > (m_LastConsoleUpdate + 150))
            {
                m_LastConsoleUpdate += 150;
                UpdateConsole();
            }

            if (Console.KeyAvailable)
            {
                switch (Console.ReadKey().KeyChar)
                {
                    case 'b':
                        StopCPU();
                        break;
                    case 'w':
                        StopCPU();
                        m_CPU.Benchmark(false, 0x800);
                        break;
                    case 'e':
                        StopCPU();
                        m_CPU.Benchmark(true, 0x800);
                        break;
                    case 'r':
                        StartCPU();
                        break;
                    case 'n':
                        StopCPU();
                        m_CPU.RunOneInstruction();
                        break;
                    case 'q':
                        StopCPU();
                        m_CPU = null;
                        break;
                    case 'l':
                        StopCPU();
                        m_CPU.PS_R = true;
                        m_CPU.LoadBinaryToMemory("../../../../Tests/rain.yasm.bin", 0x0000);
                        m_CPU.PC = 0x0000;
                        break;
                }
            }
        }

        private void StartCPU()
        {
            Platform.ParallelTasks.Parallel.StartBackground(Task_StartCPU);
            m_Stopwatch.Reset();
            m_Stopwatch.Start();
        }

        private void StopCPU()
        {
            if (m_CPU.Running)
            {
                m_Stopwatch.Stop();
                m_LastRunMS = (int)m_Stopwatch.ElapsedMilliseconds;
                m_CPU.Pause();
            }
        }

        private void Task_StartCPU()
        {
            m_CPU.Run();
        }

        #region Console
        public void UpdateConsole()
        {
            Console.Title = "YCPU Emulator";

            int r_y = 1;
            ConsoleWrite(70, r_y, "Registers");
            ConsoleWrite(70, r_y + 1, string.Format("R0: ${0:X4}", m_CPU.R0));
            ConsoleWrite(70, r_y + 2, string.Format("R1: ${0:X4}", m_CPU.R1));
            ConsoleWrite(70, r_y + 3, string.Format("R2: ${0:X4}", m_CPU.R2));
            ConsoleWrite(70, r_y + 4, string.Format("R3: ${0:X4}", m_CPU.R3));
            ConsoleWrite(70, r_y + 5, string.Format("R4: ${0:X4}", m_CPU.R4));
            ConsoleWrite(70, r_y + 6, string.Format("R5: ${0:X4}", m_CPU.R5));
            ConsoleWrite(70, r_y + 7, string.Format("R6: ${0:X4}", m_CPU.R6));
            ConsoleWrite(70, r_y + 8, string.Format("SP: ${0:X4}", m_CPU.R7));

            ConsoleWrite(70, r_y + 9, string.Format("FL: ${0:X4}", m_CPU.FL));
            ConsoleWrite(70, r_y + 10, string.Format("IA: ${0:X4}", m_CPU.IA));
            ConsoleWrite(70, r_y + 11, string.Format("II: ${0:X4}", 0));
            ConsoleWrite(70, r_y + 12, string.Format("PC: ${0:X4}", m_CPU.PC));
            ConsoleWrite(70, r_y + 13, string.Format("PS: ${0:X4}", m_CPU.PS));
            ConsoleWrite(70, r_y + 14, string.Format("P2: ${0:X4}", 0));
            ConsoleWrite(69, r_y + 15, string.Format("USP: ${0:X4}", 0));
            ConsoleWrite(69, r_y + 16, string.Format("SSP:*${0:X4}", 0));

            ConsoleWrite(70, r_y + 18, "PS Bits:");
            ConsoleWrite(70, r_y + 19, string.Format("{0}{1}{2}{3} {4}{5}{6}{7}",
                m_CPU.PS_S ? "S" : ".", m_CPU.PS_M ? "M" : ".", m_CPU.PS_I ? "I" : ".", m_CPU.PS_R ? "R" : ".",
                ".", ".", ".", "."));

            ConsoleWrite(70, r_y + 21, "FL Bits:");
            ConsoleWrite(70, r_y + 22, string.Format("{0}{1}{2}{3} ....", 
                m_CPU.FL_N ? "N" : ".", m_CPU.FL_Z ? "Z" : ".", m_CPU.FL_C ? "C" : ".", m_CPU.FL_V ? "V" : "."));

            ConsoleWrite(2, r_y, "Disassembly");
            string[] disasm = m_CPU.Disassemble(m_CPU.PC, -9, 21);
            for (int i = 0; i < 21; i++)
                ConsoleWrite(2, r_y + i + 1, disasm[i] + new string(' ', 50 - disasm[i].Length));
            ConsoleWrite(0, 11, ">");

            ConsoleWrite(2, 23, string.Format("{0} Cycles in {1} ms.            ", m_CPU.LastRunCycles, m_LastRunMS));

            ConsoleWrite(0, 24, "Command: ");
        }

        private void ConsoleWrite(int x, int y, string s)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(s);
        }
        #endregion
    }
}
