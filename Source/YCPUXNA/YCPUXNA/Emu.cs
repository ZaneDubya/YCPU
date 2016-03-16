using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using YCPUXNA.Providers;
using Ypsilon.Core.Graphics;
using Ypsilon.Core.Input;
using Ypsilon.Core.Windows;
using Ypsilon.Emulation;
using Ypsilon.Emulation.Processor;

namespace YCPUXNA
{
    class Emu : Game
    {
        public static ServiceRegistry Registry
        {
            get;
            private set;
        }

        private const int c_ConsoleWidth = 120;
        private const int c_ConsoleHeight = 40;
        private const int c_ConsoleUpdateMS = 50; // don't go lower than 50, max update rate is 16-33 ms.

        private const string c_CursesFont = @"Content\BIOS8x14.png";

        private GraphicsDeviceManager m_Graphics;
        private SpriteBatchExtended m_SpriteBatch;
        private List<ITexture> m_DeviceTextures = new List<ITexture>();
        
        private InputManager m_InputManager;
        private InputProvider m_InputProvider;
        private DisplayProvider m_DisplayProvider;

        private Emulator m_Emulator;
        private Curses m_Curses;

        private double m_LastConsoleUpdate = 0;
        private bool m_DoScreenshot = false;

        public Emu()
        {
            m_Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Registry = new ServiceRegistry();

            Registry.Register<SpriteBatchExtended>(m_SpriteBatch = new SpriteBatchExtended(this));
            m_SpriteBatch.Initialize();

            Registry.Register<InputManager>(m_InputManager = new InputManager(Window.Handle));

            m_InputProvider = new InputProvider(m_InputManager);
            m_DisplayProvider = new DisplayProvider(m_SpriteBatch);

            m_Emulator = new Emulator(m_DisplayProvider, m_InputProvider);
            m_Curses = new Curses(GraphicsDevice, c_ConsoleWidth, c_ConsoleHeight, c_CursesFont, true);

            m_Graphics.PreferredBackBufferWidth = m_Curses.DisplayWidth;
            m_Graphics.PreferredBackBufferHeight = m_Curses.DisplayHeight;
            m_Graphics.IsFullScreen = false;
            m_Graphics.ApplyChanges();
            IsMouseVisible = true;

            base.Initialize();
        }

        protected override void UnloadContent()
        {
            m_SpriteBatch.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            float totalSeconds = (float)gameTime.TotalGameTime.TotalSeconds;
            float frameSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            m_InputManager.Update(totalSeconds, frameSeconds);
            m_InputProvider.Update(totalSeconds, frameSeconds);

            UpdateEmulator(gameTime.ElapsedGameTime.TotalMilliseconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (m_DoScreenshot)
            {
                GraphicsDevice.PrepareScreenShot();
            }

            GraphicsDevice.Clear(Color.Black);

            m_Curses.Render(m_SpriteBatch, Vector2.Zero);

            // render the devices
            m_DeviceTextures.Clear();
            m_Emulator.Draw(m_DeviceTextures);
            for (int i = 0; i < m_DeviceTextures.Count; i++)
            {
                m_SpriteBatch.DrawSprite(
                    (m_DeviceTextures[i] as YTexture).Texture,
                    new Vector3(82 * (m_Curses.CharWidth + 1), 2 * m_Curses.CharHeight, 0),
                    new Vector2(m_DeviceTextures[i].Width * 2, m_DeviceTextures[i].Height * 2), Color.White);
            }

            m_SpriteBatch.Draw(gameTime);

            if (m_DoScreenshot)
            {
                GraphicsDevice.SaveScreenshot();
                m_DoScreenshot = false;
            }
        }

        private void UpdateEmulator(double frameMS)
        {
            m_LastConsoleUpdate += frameMS;
            if (m_LastConsoleUpdate > c_ConsoleUpdateMS)
            {
                m_LastConsoleUpdate -= c_ConsoleUpdateMS;
                UpdateConsole();
            }

            if (m_InputProvider.HandleKeyboardEvent(KeyboardEvent.Press, WinKeys.Escape, false, false, true))
            {
                Exit();
            }
            else if (m_InputProvider.HandleKeyboardEvent(KeyboardEvent.Press, WinKeys.R, false, false, true))
            {
                m_Emulator.StartCPU();
            }
            else if (m_InputProvider.HandleKeyboardEvent(KeyboardEvent.Press, WinKeys.B, false, false, true))
            {
                m_Emulator.StopCPU();
            }
            else if (m_InputProvider.HandleKeyboardEvent(KeyboardEvent.Press, WinKeys.N, false, false, true))
            {
                m_Emulator.RunOneInstruction();
            }
            else if (m_InputProvider.HandleKeyboardEvent(KeyboardEvent.Press, WinKeys.M, false, false, true))
            {
                m_Emulator.RunCycles(100);
            }
            else if (m_InputProvider.HandleKeyboardEvent(KeyboardEvent.Press, WinKeys.L, false, false, true))
            {
#if DEBUG
                m_Emulator.LoadBinaryToCPU("../../Tests/rain.asm.bin", 0x0000);
#else
                m_Emulator.LoadBinaryToCPU("../Tests/bld/AsmTstGn-0.asm.bin", 0x0000);
#endif
            }
            else if (m_InputProvider.HandleKeyboardEvent(KeyboardEvent.Press, WinKeys.T, false, false, true))
            {
                m_Emulator.CPU.Interrupt_Reset();
            }
            else if (m_InputProvider.HandleKeyboardEvent(KeyboardEvent.Press, WinKeys.S, false, false, true))
            {
                m_DoScreenshot = true;
            }

            m_Emulator.Update(frameMS);
        }

        #region Console
        public void UpdateConsole()
        {
            m_Curses.Clear();
            YCPU cpu = m_Emulator.CPU;

            // registers - general purpose, then system.
            int r_y = 2;
            ConsoleWrite(70, r_y - 1, "Registers");
            ConsoleWrite(70, r_y + 1, string.Format("r0: ${0:X4}", cpu.R0));
            ConsoleWrite(70, r_y + 2, string.Format("r1: ${0:X4}", cpu.R1));
            ConsoleWrite(70, r_y + 3, string.Format("r2: ${0:X4}", cpu.R2));
            ConsoleWrite(70, r_y + 4, string.Format("r3: ${0:X4}", cpu.R3));
            ConsoleWrite(70, r_y + 5, string.Format("r4: ${0:X4}", cpu.R4));
            ConsoleWrite(70, r_y + 6, string.Format("r5: ${0:X4}", cpu.R5));
            ConsoleWrite(70, r_y + 7, string.Format("r6: ${0:X4}", cpu.R6));
            ConsoleWrite(70, r_y + 8, string.Format("r7: ${0:X4}", cpu.R7));

            ConsoleWrite(70, r_y + 10, string.Format("fl: ${0:X4}", cpu.FL));
            ConsoleWrite(70, r_y + 11, string.Format("pc: ${0:X4}", cpu.PC));
            ConsoleWrite(70, r_y + 12, string.Format("ps: ${0:X4}", cpu.PS));
            ConsoleWrite(69, r_y + 13, string.Format("usp: ${0:X4}", cpu.USP));
            ConsoleWrite(69, r_y + 14, string.Format("ssp:*${0:X4}", cpu.SSP));

            ConsoleWrite(70, r_y + 19, "ps bits:");
            ConsoleWrite(70, r_y + 20, string.Format("{0}{1}{2}{3} {4}{5}{6}{7}",
                cpu.PS_S ? "S" : ".", cpu.PS_M ? "M" : ".", cpu.PS_H ? "I" : ".", ".",
                cpu.PS_Q ? "Q" : ".", cpu.PS_V ? "V" : ".", cpu.PS_W ? "W" : ".", "."));

            ConsoleWrite(70, r_y + 22, "fl bits:");
            ConsoleWrite(70, r_y + 23, string.Format("{0}{1}{2}{3}",
                cpu.FL_N ? "N" : ".", cpu.FL_Z ? "Z" : ".", cpu.FL_C ? "C" : ".", cpu.FL_V ? "V" : "."));

            ConsoleWrite(70, r_y + 25, "Memory management:");
            ConsoleWrite(70, r_y + 26, "CS " + ConsoleSegmentRegisterString(cpu.CS));
            ConsoleWrite(70, r_y + 27, "DS " + ConsoleSegmentRegisterString(cpu.DS));
            ConsoleWrite(70, r_y + 28, "ES " + ConsoleSegmentRegisterString(cpu.ES));
            ConsoleWrite(70, r_y + 29, "SS " + ConsoleSegmentRegisterString(cpu.SS));
            ConsoleWrite(70, r_y + 30, "IS " + ConsoleSegmentRegisterString(cpu.IS));

            // disassembly
            ConsoleWrite(2, r_y - 1, "Disassembly");
            string[] disasm = cpu.Disassemble(cpu.PC, 21, true);
            for (int i = 0; i < 21; i += 1)
                ConsoleWrite(2, r_y + i + 1, disasm[i] + new string(' ', 50 - disasm[i].Length));
            ConsoleWrite(1, 3, ">");
            ConsoleWrite(2, 25, string.Format("{0} Cycles total", cpu.Cycles));

            ConsoleWrite(2, 27, "Ctrl-L: Load debug console program.");
            ConsoleWrite(2, 28, "Ctrl-R: Run at 10 khz.");
            ConsoleWrite(2, 29, "Ctrl-B: Break.");
            ConsoleWrite(2, 30, "Ctrl-N: Run one instruction.");
            ConsoleWrite(2, 31, "Ctrl-M: Run approximately 100 cycles.");
            ConsoleWrite(2, 32, "Ctrl-T: Reset interrupt.");
        }

        private void ConsoleWrite(int x, int y, string s)
        {
            m_Curses.WriteString(x, y, s);
            // Console.SetCursorPosition(x, y);
            // Console.Write(s);
        }

        private string ConsoleSegmentRegisterString(uint register)
        {
            return string.Format("${0:X8}", register);
            /*ushort bank = (ushort)(cache0 & 0x00ff);
            ushort device = (ushort)((cache0 >> 8) & 0x0f);
            return string.Format("${0:X1} ${1:X2} {2}{3}{4}{5}",
                    device, bank,
                    (cache0 & 0x8000) != 0 ? "S" : ".",
                    (cache0 & 0x4000) != 0 ? "W" : ".",
                    (cache0 & 0x2000) != 0 ? "P" : ".",
                    (cache0 & 0x1000) != 0 ? "A" : "."
                    );*/
        }
        #endregion
    }
}
