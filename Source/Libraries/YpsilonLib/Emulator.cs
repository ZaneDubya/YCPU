using System;
using System.Threading;
using Ypsilon.Hardware;
using System.Collections.Generic;

namespace Ypsilon
{
    public class Emulator
    {
        public YCPU CPU
        {
            get;
            private set;
        }

        private bool m_Running = false;

        public Emulator()
        {

        }

        public void Update(double frameMS)
        {
            if (CPU == null)
            {
                CPU = new YCPU();
                SetupDebugDevices();
                CPU.Interrupt_Reset();
            }

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

            CPU.BUS.AddDevice(new Ypsilon.Devices.Graphics.GraphicsAdapter(CPU.BUS));
            CPU.BUS.AddDevice(new Ypsilon.Devices.Input.KeyboardDevice(CPU.BUS));
        }

        public void LoadBinaryToCPU(string path, ushort address)
        {
            StopCPU();

            byte[] data = Common.GetBytesFromFile(path);
            if (data != null)
            {
                for (int i = 0; i < data.Length; i += 1)
                {
                    CPU.WriteMemInt8((ushort)(address), data[i]);
                    address += 1;
                }
            }

            CPU.Interrupt_Reset();
        }
    }
}
