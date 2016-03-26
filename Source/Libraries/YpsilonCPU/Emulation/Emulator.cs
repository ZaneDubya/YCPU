using System.Collections.Generic;
using System.IO;
using Ypsilon.Emulation.Devices.Graphics;
using Ypsilon.Emulation.Devices.Input;
using Ypsilon.Emulation.Processor;

namespace Ypsilon.Emulation
{
    public class Emulator
    {
        public YCPU CPU
        {
            get;
        }

        private bool m_Running;

        public Emulator(IDisplayProvider display, IInputProvider input)
        {
            CPU = new YCPU();
            CPU.BUS.Reset();
            CPU.BUS.AddDevice(new GraphicsAdapter(CPU.BUS), 1);
            CPU.BUS.AddDevice(new KeyboardDevice(CPU.BUS), 2);
            CPU.BUS.SetRAM(0x20000);
            CPU.BUS.SetROM(0x04000);
            CPU.BUS.SetProviders(display, input);
            CPU.Interrupt_Reset();
        }

        public void Update(double frameMS)
        {
            if (m_Running)
            {
                CPU.Run((int)(YCPU.ClockRateHz * (frameMS / 1000d)));
                CPU.BUS.Update();
            }
            
        }

        public void Draw(List<ITexture> textures)
        {
            CPU.BUS.Display(textures);
        }

        public void RunCPU(int cycles)
        {
            StopCPU();
            m_Running = true;
            CPU.Run(cycles);
            StopCPU();
        }

        public void RunOneInstruction()
        {
            StopCPU();
            CPU.RunOneInstruction();
        }

        public void RunCycles(int cycleCount)
        {
            StopCPU();
            CPU.Run(cycleCount);
        }

        public void StartCPU()
        {
            StopCPU();
            m_Running = true;
        }

        public void StopCPU()
        {
            if (CPU.Running)
            {
                CPU.Pause();
            }
            m_Running = false;
        }

        public void LoadBinaryToROM(string path)
        {
            StopCPU();

            byte[] data = getBytesFromFile(path);
            if (data != null)
            {
                CPU.BUS.FillROM(data);
            }

            CPU.Interrupt_Reset();
        }

        private byte[] getBytesFromFile(string path)
        {
            try
            {
                byte[] data = File.ReadAllBytes(path);
                return data;
            }
            catch
            {
                return null;
            }
        }
    }
}
