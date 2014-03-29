using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCPU.Hardware
{
    class GraphicsAdapter : BaseDevice
    {
        protected override ushort DeviceType
        {
            get { return 0x0000; }
        }
        protected override ushort ManufacturerID
        {
            get { return 0x0000; }
        }
        protected override ushort DeviceID
        {
            get { return 0x0000; }
        }
        protected override ushort DeviceRevision
        {
            get { return 0x0000; }
        }

        public GraphicsAdapter(Platform.YBUS bus)
            : base(bus)
        {

        }
    }
}
