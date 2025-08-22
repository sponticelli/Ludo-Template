namespace Ludo.Core.Boot
{
    /// <summary>
    /// Represents a discrete step in the application's boot sequence.
    /// Implementations should be stateless and easily removable.
    /// </summary>
    public interface IBootStep
    {
        /// <summary>
        /// Execution order for this step. Lower values run first.
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Execute the boot logic.
        /// </summary>
        void Boot();
    }
}
