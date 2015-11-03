using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YCPUXNA.ServiceProviders;
using Ypsilon;
using Ypsilon.Hardware;
using YCPUXNA.ServiceProviders.Input;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace YCPUXNA
{
    class Emu : Game
    {
        private GraphicsDeviceManager m_Graphics;
        private SpriteBatch m_SpriteBatch;
        private List<ITexture> m_DeviceTextures;

        private InputService m_InputProvider;
        private DeviceRenderService m_DeviceRenderer;

        private Emulator m_Emulator;
        private Display.Curses m_Curses;

        private double m_LastConsoleUpdate = 0;
        private const int c_ConsoleWidth = 128;
        private const int c_ConsoleHeight = 64;
        private const int c_ConsoleUpdateMS = 50; // don't go lower than 50, max update rate is 16-33 ms.

        public Emu()
        {
            m_Graphics = new GraphicsDeviceManager(this);
            m_Graphics.IsFullScreen = false;
            m_Graphics.PreferredBackBufferWidth = c_ConsoleWidth * 8;
            m_Graphics.PreferredBackBufferHeight = c_ConsoleHeight * 8;

            this.IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            m_SpriteBatch = new SpriteBatch(GraphicsDevice);

            m_InputProvider = (InputService)ServiceRegistry.Register<IInputProvider>(new InputService(this));
            m_DeviceRenderer = (DeviceRenderService)ServiceRegistry.Register<IDeviceRenderer>(new DeviceRenderService(m_SpriteBatch));

            m_Emulator = new Emulator();
            m_Curses = new Display.Curses(GraphicsDevice, c_ConsoleWidth, c_ConsoleHeight);
        }

        protected override void UnloadContent()
        {
            m_SpriteBatch.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            m_InputProvider.Update();

            base.Update(gameTime);

            UpdateEmulator(gameTime.ElapsedGameTime.TotalMilliseconds);
        }

        protected override void Draw(GameTime gameTime)
        {
            // render the devices
            if (m_DeviceTextures == null)
                m_DeviceTextures = new List<ITexture>();
            m_DeviceTextures.Clear();
            m_Emulator.Draw(m_DeviceTextures);

            // draw the emulator.
            base.Draw(gameTime);

            GraphicsDevice.Clear(Color.Black);
            m_SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            m_Curses.Render(m_SpriteBatch);

            for (int i = 0; i < m_DeviceTextures.Count; i++)
            {
                m_SpriteBatch.Draw((m_DeviceTextures[i] as YTexture).Texture,
                    new Rectangle(80 * 8, 1 * 8, m_DeviceTextures[i].Width * 2, m_DeviceTextures[i].Height * 2), Color.White);
            }

            m_SpriteBatch.End();
            GraphicsDevice.Textures[0] = null;
        }

        private void UpdateEmulator(double frameMS)
        {
            m_LastConsoleUpdate += frameMS;
            if (m_LastConsoleUpdate > c_ConsoleUpdateMS)
            {
                m_LastConsoleUpdate -= c_ConsoleUpdateMS;
                UpdateConsole();
            }

            if (m_InputProvider.HandleKeyboardEvent(KeyboardEventType.Press, Keys.Escape, false, false, true))
            {
                this.Exit();
            }
            else if (m_InputProvider.HandleKeyboardEvent(KeyboardEventType.Press, Keys.R, false, false, true))
            {
                m_Emulator.StartCPU();
            }
            else if (m_InputProvider.HandleKeyboardEvent(KeyboardEventType.Press, Keys.B, false, false, true))
            {
                m_Emulator.StopCPU();
            }
            else if (m_InputProvider.HandleKeyboardEvent(KeyboardEventType.Press, Keys.N, false, false, true))
            {
                m_Emulator.RunOneInstruction();
            }
            else if (m_InputProvider.HandleKeyboardEvent(KeyboardEventType.Press, Keys.M, false, false, true))
            {
                m_Emulator.RunCycles(100);
            }
            else if (m_InputProvider.HandleKeyboardEvent(KeyboardEventType.Press, Keys.L, false, false, true))
            {
#if DEBUG
                m_Emulator.LoadBinaryToCPU("../../Tests/rain.asm.bin", 0x0000);
#else
                m_Emulator.LoadBinaryToCPU("../Tests/bld/AsmTstGn-0.asm.bin", 0x0000);
#endif
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
            ConsoleWrite(70, r_y + 1, string.Format("R0: ${0:X4}", cpu.R0));
            ConsoleWrite(70, r_y + 2, string.Format("R1: ${0:X4}", cpu.R1));
            ConsoleWrite(70, r_y + 3, string.Format("R2: ${0:X4}", cpu.R2));
            ConsoleWrite(70, r_y + 4, string.Format("R3: ${0:X4}", cpu.R3));
            ConsoleWrite(70, r_y + 5, string.Format("R4: ${0:X4}", cpu.R4));
            ConsoleWrite(70, r_y + 6, string.Format("R5: ${0:X4}", cpu.R5));
            ConsoleWrite(70, r_y + 7, string.Format("R6: ${0:X4}", cpu.R6));
            ConsoleWrite(70, r_y + 8, string.Format("R7: ${0:X4}", cpu.R7));

            ConsoleWrite(70, r_y + 10, string.Format("FL: ${0:X4}", cpu.FL));
            ConsoleWrite(70, r_y + 11, string.Format("IA: ${0:X4}", cpu.IA));
            ConsoleWrite(70, r_y + 12, string.Format("II: ${0:X4}", cpu.II));
            ConsoleWrite(70, r_y + 13, string.Format("PC: ${0:X4}", cpu.PC));
            ConsoleWrite(70, r_y + 14, string.Format("PS: ${0:X4}", cpu.PS));
            ConsoleWrite(70, r_y + 15, string.Format("P2: ${0:X4}", cpu.P2));
            ConsoleWrite(69, r_y + 16, string.Format("USP: ${0:X4}", cpu.USP));
            ConsoleWrite(69, r_y + 17, string.Format("SSP:*${0:X4}", cpu.SSP));

            ConsoleWrite(70, r_y + 19, "PS Bits:");
            ConsoleWrite(70, r_y + 20, string.Format("{0}{1}{2}{3} {4}{5}{6}{7}",
                cpu.PS_S ? "S" : ".", cpu.PS_M ? "M" : ".", cpu.PS_I ? "I" : ".", ".",
                ".", ".", ".", "."));

            ConsoleWrite(70, r_y + 22, "FL Bits:");
            ConsoleWrite(70, r_y + 23, string.Format("{0}{1}{2}{3} ....",
                cpu.FL_N ? "N" : ".", cpu.FL_Z ? "Z" : ".", cpu.FL_C ? "C" : ".", cpu.FL_V ? "V" : "."));

            // disassembly
            ConsoleWrite(2, r_y - 1, "Disassembly");
            string[] disasm = cpu.Disassemble(cpu.PC, 21, true);
            for (int i = 0; i < 21; i += 1)
                ConsoleWrite(2, r_y + i + 1, disasm[i] + new string(' ', 50 - disasm[i].Length));
            ConsoleWrite(1, 3, ">");
            ConsoleWrite(2, 23, string.Format("{0} Cycles total", cpu.Cycles));

            ConsoleWrite(2, 25, "Ctrl-L: Load debug console program.");
            ConsoleWrite(2, 26, "Ctrl-R: Run at 10 khz.");
            ConsoleWrite(2, 27, "Ctrl-B: Break.");
            ConsoleWrite(2, 28, "Ctrl-N: Run one instruction.");
            ConsoleWrite(2, 29, "Ctrl-M: Run approximately 100 cycles.");
        }

        private void ConsoleWrite(int x, int y, string s)
        {
            m_Curses.WriteString(x, y, s);
            // Console.SetCursorPosition(x, y);
            // Console.Write(s);
        }
        #endregion
    }
}
