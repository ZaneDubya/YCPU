using System;

namespace Ypsilon.Emulation.Processor {
    public class SegFaultException : Exception {
        public readonly ushort Address;
        public readonly SegmentIndex SegmentType;

        public SegFaultException(SegmentIndex si, ushort address) {
            SegmentType = si;
            Address = address;
        }
    }
}