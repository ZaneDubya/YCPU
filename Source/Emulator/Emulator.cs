using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Threading;

namespace Ypsilon
{
    class Emulator : Platform.Host
    {
        private Hardware.YCPU m_CPU;
        private System.Diagnostics.Stopwatch m_Stopwatch;
        private int m_LastRunMS;

        public Emulator() : base()
        {
            m_Stopwatch = new System.Diagnostics.Stopwatch();
        }

        protected override void Initialize()
        {
            base.Initialize();
            Settings.Resolution = new Point(256, 192);
            Program.ShowConsoleWindow();
        }

        private double m_LastConsoleUpdate = 0;
        private bool m_Running = false;
        private bool m_Threaded = false;

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (m_CPU == null)
            {
                m_CPU = new Hardware.YCPU();
                m_CPU.Interrupt_Reset();
            }

            if (gameTime.TotalGameTime.TotalMilliseconds > (m_LastConsoleUpdate + 200))
            {
                m_LastConsoleUpdate += 150;
                UpdateConsole();
            }

            if (m_Running && !m_Threaded)
            {
                Stopwatch_Start();
                m_CPU.Run(100000 / 60);
                Stopwatch_Stop();
            }

            List<Platform.Input.InputEventKeyboard> kb = InputState.GetKeyboardEvents();
            foreach (Platform.Input.InputEventKeyboard e in kb)
            {
                switch (e.KeyChar)
                {
                    case 'b':
                        StopCPU();
                        break;
                    case 'r':
                        StartCPU(false);
                        break;
                    case 't':
                        StartCPU(true);
                        break;
                    case 'y':
                        Stopwatch_Start();
                        m_CPU.Run(100000 / 60);
                        Stopwatch_Stop();
                        break;
                    case 'n':
                        StopCPU();
                        m_CPU.RunOneInstruction();
                        break;
                    case 'l':
                        StopCPU();
                        m_CPU.PS_R = true;
#if DEBUG
                        m_CPU.LoadBinaryToMemory("../../../../Tests/rain.s.bin", 0x0000);
#else
                        m_CPU.LoadBinaryToMemory("../Tests/rain.s.bin", 0x0000);
#endif
                        m_CPU.BUS.SetupDebugDevices();
                        m_CPU.Interrupt_Reset();
                        m_CPU.MMU_SwitchInHardwareBank(0x08, 0x00, 0x00);
                        m_CPU.PS_M = true;
                        break;
#if BENCHMARK
                    case 'v':
                        m_CPU.Benchmark(true, 0x100000);
                        break;
#endif
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            m_CPU.Display(SpriteBatch);
            base.Draw(gameTime);
            
        }

        private void StartCPU(bool threaded)
        {
            StopCPU();
            m_Running = true;
            m_Threaded = threaded;
            if (m_Threaded)
            {
                ThreadPool.QueueUserWorkItem(Task_StartCPU);
                Stopwatch_Start();
            }
        }

        private void StopCPU()
        {
            if (m_CPU.Running)
            {
                Stopwatch_Stop();
                m_CPU.Pause();
            }
            m_Running = false;
        }

        private void Task_StartCPU(Object threadContext)
        {
            m_CPU.Run();
        }

        private void Stopwatch_Start()
        {
            m_Stopwatch.Reset();
            m_Stopwatch.Start();
        }

        private void Stopwatch_Stop()
        {
            m_Stopwatch.Stop();
            m_LastRunMS = (int)m_Stopwatch.ElapsedMilliseconds;
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
            ConsoleWrite(70, r_y + 8, string.Format("R7: ${0:X4}", m_CPU.R7));

            ConsoleWrite(70, r_y + 9, string.Format("FL: ${0:X4}", m_CPU.FL));
            ConsoleWrite(70, r_y + 10, string.Format("IA: ${0:X4}", m_CPU.IA));
            ConsoleWrite(70, r_y + 11, string.Format("II: ${0:X4}", m_CPU.II));
            ConsoleWrite(70, r_y + 12, string.Format("PC: ${0:X4}", m_CPU.PC));
            ConsoleWrite(70, r_y + 13, string.Format("PS: ${0:X4}", m_CPU.PS));
            ConsoleWrite(70, r_y + 14, string.Format("P2: ${0:X4}", m_CPU.P2));
            ConsoleWrite(69, r_y + 15, string.Format("USP: ${0:X4}", m_CPU.USP));
            ConsoleWrite(69, r_y + 16, string.Format("SSP:*${0:X4}", m_CPU.SSP));

            ConsoleWrite(70, r_y + 18, "PS Bits:");
            ConsoleWrite(70, r_y + 19, string.Format("{0}{1}{2}{3} {4}{5}{6}{7}",
                m_CPU.PS_S ? "S" : ".", m_CPU.PS_M ? "M" : ".", m_CPU.PS_I ? "I" : ".", m_CPU.PS_R ? "R" : ".",
                ".", ".", ".", "."));

            ConsoleWrite(70, r_y + 21, "FL Bits:");
            ConsoleWrite(70, r_y + 22, string.Format("{0}{1}{2}{3} ....", 
                m_CPU.FL_N ? "N" : ".", m_CPU.FL_Z ? "Z" : ".", m_CPU.FL_C ? "C" : ".", m_CPU.FL_V ? "V" : "."));

            ConsoleWrite(2, r_y, "Disassembly");
            string[] disasm = m_CPU.Disassemble(m_CPU.PC, -9, 21);
            for (int i = 0; i < 21; i += 1)
                ConsoleWrite(2, r_y + i + 1, disasm[i] + new string(' ', 50 - disasm[i].Length));
            ConsoleWrite(0, 11, ">");
            if (m_CPU.LastRunCycles != 0)
            {
                ConsoleWrite(2, 23, string.Format("{0} Cycles in {1} ms. {2:0.00} mhz                  ", m_CPU.LastRunCycles, m_LastRunMS, ((float)m_CPU.LastRunCycles / m_LastRunMS) / 1000));
            }
        }

        private void ConsoleWrite(int x, int y, string s)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(s);
        }
        #endregion
    }
}
