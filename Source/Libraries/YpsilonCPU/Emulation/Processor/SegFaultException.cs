using System;

namespace Ypsilon.Emulation.Processor
{
    public class SegFaultException : Exception
    {
        public readonly SegmentIndex SegmentType;
        public readonly ushort Address;

        public SegFaultException(SegmentIndex si, ushort address)
            : base()
        {
            SegmentType = si;
            Address = address;
        }
    }
}
