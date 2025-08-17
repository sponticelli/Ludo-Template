using Ludo.Core.Events;

namespace Ludo.Core
{
    /// <summary>
    ///     Base class for game modules that can be initialized and
    ///     uninitialized. Modules may subscribe to events via the provided
    ///     <see cref="EventBinder" /> and are designed to be managed by a
    ///     <see cref="AGameManager" /> or <see cref="ASceneManager" />.
    /// </summary>
    public abstract class AModule : CachedMonoBehaviour
    {
        /// <summary>
        ///     Event binder used by the module for managing event subscriptions.
        /// </summary>
        protected readonly EventBinder m_Binder = new EventBinder();

        /// <summary>
        ///     Indicates whether the module has completed its initialization.
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        ///     Initializes the module if it has not already been initialized.
        /// </summary>
        public void Initialize()
        {
            if (!IsInitialized)
            {
                HandleInitialization();
                IsInitialized = true;
            }
        }

        /// <summary>
        ///     Uninitializes the module if it is currently initialized.
        /// </summary>
        public void Uninitialize()
        {
            if (IsInitialized)
            {
                HandleUninitialization();
                IsInitialized = false;
            }
            m_Binder.Unbind();
        }

        /// <summary>
        ///     Called when the module is initialized.
        /// </summary>
        protected abstract void HandleInitialization();

        /// <summary>
        ///     Called when the module is uninitialized.
        /// </summary>
        protected abstract void HandleUninitialization();
    }
}
