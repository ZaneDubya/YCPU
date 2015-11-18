
namespace Ypsilon.Emulation
{
    /// <summary>
    /// Standard interface to a 4 kilobyte memory bank.
    /// </summary>
    public interface IMemoryBank
    {
        /// <summary>
        /// If ReadOnly is true, writes to this bank should fail silently. This does not effect the MMU's 'write protection' bit.
        /// </summary>
        bool ReadOnly
        {
            get; set;
        }

        /// <summary>
        /// Returns the byte of memory at this address.
        /// </summary>
        /// <returns></returns>
        byte this[int address]
        {
            get; set;
        }
    }
}
