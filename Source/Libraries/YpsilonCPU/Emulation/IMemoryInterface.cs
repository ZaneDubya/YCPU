namespace Ypsilon.Emulation {
    /// <summary>
    /// Standard interface to a block of memory
    /// </summary>
    public interface IMemoryInterface {
        /// <summary>
        /// Returns the byte of memory at this address.
        /// </summary>
        /// <returns></returns>
        byte this[uint address] { get; set; }
    }
}