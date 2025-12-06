using System.Collections.Generic;
using System.IO;
using Ypsilon.Emulation.Devices.Graphics;
using Ypsilon.Emulation.Devices.Input;
using Ypsilon.Emulation.Processor;

namespace Ypsilon.Emulation {
    public class Emulator : IEmulator {
        private bool m_Running;

        public YCPU CPU { get; }

        public Emulator() {
            CPU = new YCPU();
            CPU.BUS.Reset();
            CPU.BUS.AddDevice(new GraphicsAdapter(CPU.BUS), 1);
            CPU.BUS.AddDevice(new KeyboardDevice(CPU.BUS), 2);
            CPU.BUS.SetRAM(0x20000);
            CPU.BUS.SetROM(0x04000);
            CPU.Interrupt_Reset();
        }

        public void Draw(List<ITexture> textures) {
            CPU.BUS.Display(textures);
        }

        public void LoadBinaryToROM(string path) {
            Stop();
            byte[] data = getBytesFromFile(path);
            if (data != null) {
                CPU.BUS.FillROM(data);
            }
            CPU.Interrupt_Reset();
        }

        public void Start() {
            Stop();
            m_Running = true;
        }

        public void Stop() {
            if (CPU.Running)
                CPU.Pause();
            m_Running = false;
        }

        public void Update(float frameMS) {
            if (!m_Running)
                return;
            CPU.Run((int)(YCPU.ClockRateHz * (frameMS / 1000d)));
            CPU.BUS.Update();
        }

        public void Initialize(IDisplayProvider display, IInputProvider input) {
            CPU.BUS.SetProviders(display, input);
        }

        public void RunCycles(int cycleCount) {
            Stop();
            CPU.Run(cycleCount);
        }

        public void RunOneInstruction() {
            Stop();
            CPU.RunOneInstruction();
        }

        private byte[] getBytesFromFile(string path) {
            try {
                byte[] data = File.ReadAllBytes(path);
                return data;
            }
            catch {
                return null;
            }
        }
    }
}