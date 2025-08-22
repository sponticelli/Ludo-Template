using UnityEngine;

namespace Ludo.Core.Boot
{
    /// <summary>
    /// Base class for ScriptableObject boot steps.
    /// </summary>
    public abstract class BootStep : ScriptableObject, IBootStep
    {
        /// <summary>
        /// Execution order for this boot step. Lower values run earlier.
        /// </summary>
        [SerializeField]
        private int order;

        /// <inheritdoc />
        public int Order => order;

        /// <summary>
        /// Executes the boot logic for this step.
        /// </summary>
        public abstract void Boot();
    }
}
