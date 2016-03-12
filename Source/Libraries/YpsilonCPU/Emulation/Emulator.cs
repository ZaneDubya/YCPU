using System;
using System.Threading;
using Ypsilon.Emulation.Hardware;
using System.Collections.Generic;

namespace Ypsilon.Emulation
{
    public class Emulator
    {
        public YCPU CPU
        {
            get;
            private set;
        }

        private bool m_Running = false;

        public Emulator(IDisplayProvider display, IInputProvider input)
        {
            CPU = new YCPU();
            CPU.BUS.SetProviders(display, input);
            SetupDebugDevices();
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

        private void SetupDebugDevices()
        {
            CPU.BUS.Reset();

            CPU.BUS.SetRAM(0x20000);
            CPU.BUS.AddDevice(new Devices.Graphics.GraphicsAdapter(CPU.BUS), 1);
            CPU.BUS.AddDevice(new Devices.Input.KeyboardDevice(CPU.BUS), 2);
            
        }

        public void LoadBinaryToCPU(string path, ushort address)
        {
            StopCPU();

            byte[] data = Common.GetBytesFromFile(path);
            if (data != null)
            {
                CPU.BUS.SetROM(0x10000, data);
            }

            CPU.Interrupt_Reset();
        }
    }
}
