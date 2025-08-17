using Ludo.Core.Events;

namespace Ludo.Core
{
    public abstract class AModule : CachedMonoBehaviour
    {
        protected readonly EventBinder m_Binder = new EventBinder();

        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            if (!IsInitialized)
            {
                HandleInitialization();
                IsInitialized = true;
            }
        }

        public void Uninitialize()
        {
            if (IsInitialized)
            {
                HandleUninitialization();
                IsInitialized = false;
            }
            m_Binder.Unbind();
        }

        protected abstract void HandleInitialization();

        protected abstract void HandleUninitialization();
    }
}