
namespace YCPU.Library.ParallelTasks
{
    /// <summary>
    /// An interface for a piece of work which can be executed by YCPU.Library.ParallelTasks.
    /// </summary>
    public interface IWork
    {
        /// <summary>
        /// Executes the work.
        /// </summary>
        void DoWork();

        /// <summary>
        /// Gets options specifying how this work may be executed.
        /// </summary>
        WorkOptions Options { get; }
    }
}
