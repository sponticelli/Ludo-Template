using UnityEngine;

namespace Ludo.Core.Boot
{
    /// <summary>
    /// Simple boot step that logs a message for debugging purposes.
    /// </summary>
    [CreateAssetMenu(menuName = "Ludo/Boot/DebugBootStep")]
    public class DebugBootStep : BootStep
    {
        /// <summary>
        /// Message written to the console during boot.
        /// </summary>
        [SerializeField]
        private string message = "Boot step";

        /// <inheritdoc />
        public override void Boot()
        {
            Debug.Log(message);
        }
    }
}
