using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.Threading;
using Ypsilon.Hardware;
using Ypsilon.Platform;
using Ypsilon.Platform.Input.Windows;

namespace Ypsilon
{
    class Emulator : Host
    {
        private YCPU m_CPU;
        private Stopwatch m_Stopwatch;
        private IDeviceRenderer m_DeviceRenderer;

        public Emulator()
            : base()
        {
            m_Stopwatch = new Stopwatch();
        }

        protected override void Initialize()
        {
            base.Initialize();

            m_DeviceRenderer = (DeviceRenderer)Create<DeviceRenderer>();

            Settings.Resolution = new Point(256, 192);
            Program.ShowConsoleWindow();
        }

        private double m_LastConsoleUpdate = 0;
        private bool m_Running = false;
        private bool m_Threaded = false;
        private int m_LastRunMS;
        private long m_LastRunCycles;

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (m_CPU == null)
            {
                m_CPU = new YCPU();
                SetupDebugDevices();
                m_CPU.Interrupt_Reset();
            }

            if (gameTime.TotalGameTime.TotalMilliseconds > (m_LastConsoleUpdate + 200))
            {
                m_LastConsoleUpdate += 150;
                UpdateConsole();
            }

            if (InputState.HandleKeyboardEvent(KeyboardEventType.Press, WinKeys.Escape, false, false, true))
            {
                this.Exit();
            }
            else if (InputState.HandleKeyboardEvent(KeyboardEventType.Press, WinKeys.R, false, false, true))
            {
                StartCPU(false);
            }
            else if (InputState.HandleKeyboardEvent(KeyboardEventType.Press, WinKeys.T, false, false, true))
            {
                StartCPU(true);
            }
            else if (InputState.HandleKeyboardEvent(KeyboardEventType.Press, WinKeys.B, false, false, true))
            {
                StopCPU();
            }
            else if (InputState.HandleKeyboardEvent(KeyboardEventType.Press, WinKeys.N, false, false, true))
            {
                StopCPU();
                m_CPU.RunOneInstruction();
            }
            else if (InputState.HandleKeyboardEvent(KeyboardEventType.Press, WinKeys.M, false, false, true))
            {
                StopCPU();
                m_CPU.Run(100);
            }
            else if (InputState.HandleKeyboardEvent(KeyboardEventType.Press, WinKeys.L, false, false, true))
            {
                StopCPU();
#if DEBUG
                LoadBinaryToCPU("../../../../Tests/rain.asm.bin", 0x0000);
#else
                LoadBinaryToCPU("../Tests/rain.asm.bin", 0x0000);
#endif
                m_CPU.Interrupt_Reset();
            }

            if (m_Running)
            {
                if (!m_Threaded)
                {
                    m_CPU.Run(100000 / 60);
                }
                m_CPU.BUS.Update(InputState);
            }
            
        }

        protected override void OnDraw(GameTime gameTime)
        {
            m_CPU.BUS.Display(m_DeviceRenderer);
        }

        private void RunCPU(int cycles)
        {
            StopCPU();
            m_Running = true;
            m_Threaded = false;
            Stopwatch_Start();
            m_CPU.Run(cycles);
            StopCPU();
        }

        private void StartCPU(bool threaded)
        {
            StopCPU();
            m_Running = true;
            m_Threaded = threaded;
            Stopwatch_Start();
            if (m_Threaded)
            {
                ThreadPool.QueueUserWorkItem(Task_StartCPU);
            }
        }

        private void StopCPU()
        {
            Stopwatch_Stop();
            if (m_CPU.Running)
            {
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
            m_LastRunCycles = m_CPU.Cycles;
            m_LastRunMS = 0;
            m_Stopwatch.Reset();
            m_Stopwatch.Start();

        }

        private void Stopwatch_Stop()
        {
            if (m_Stopwatch.IsRunning)
            {
                m_Stopwatch.Stop();
                m_LastRunMS = (int)m_Stopwatch.ElapsedMilliseconds;
                m_LastRunCycles = m_CPU.Cycles - m_LastRunCycles;
            }
        }

        private void SetupDebugDevices()
        {
            m_CPU.BUS.Reset();

            m_CPU.BUS.AddDevice(new Ypsilon.Devices.Graphics.GraphicsAdapter(m_CPU.BUS));
            m_CPU.BUS.AddDevice(new Ypsilon.Devices.Input.Keyboard(m_CPU.BUS));
        }

        private void LoadBinaryToCPU(string path, ushort address)
        {
            byte[] data = Platform.Common.GetBytesFromFile(path);
            if (data != null)
            {
                for (int i = 0; i < data.Length; i += 1)
                {
                    m_CPU.WriteMemInt8((ushort)(address), data[i]);
                    address += 1;
                }
            }
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
                m_CPU.PS_S ? "S" : ".", m_CPU.PS_M ? "M" : ".", m_CPU.PS_I ? "I" : ".", ".",
                ".", ".", ".", "."));

            ConsoleWrite(70, r_y + 21, "FL Bits:");
            ConsoleWrite(70, r_y + 22, string.Format("{0}{1}{2}{3} ....", 
                m_CPU.FL_N ? "N" : ".", m_CPU.FL_Z ? "Z" : ".", m_CPU.FL_C ? "C" : ".", m_CPU.FL_V ? "V" : "."));

            ConsoleWrite(2, r_y, "Disassembly");
            string[] disasm = m_CPU.Disassemble(m_CPU.PC, 21, true);
            for (int i = 0; i < 21; i += 1)
                ConsoleWrite(2, r_y + i + 1, disasm[i] + new string(' ', 50 - disasm[i].Length));
            ConsoleWrite(0, 2, ">");
            ConsoleWrite(2, 23, string.Format("{0} Cycles total", m_CPU.Cycles));
            if (m_LastRunCycles != 0 && m_LastRunMS > 0)
            {
                ConsoleWrite(2, 24, string.Format("{0} Cycles in {1} ms. {2:0.0000} mhz                  ", m_LastRunCycles, m_LastRunMS, ((float)m_LastRunCycles / m_LastRunMS) / 1000));
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
