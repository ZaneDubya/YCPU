using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Emulation
{
    public enum MemoryReference
    {
        DeviceIndex = 0x00FF,
        ReferenceType = 0xFF00,
        None = 0x0000,
        RAM = 0x0100,
        ROM = 0x0200,
        Device = 0x0400,
    }
}
