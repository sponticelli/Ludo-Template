using System.Collections.Generic;
using Ludo.Core.Events;

namespace Ludo.Core
{
    public abstract class ASceneManager : CachedMonoBehaviour
    {
        public readonly AEvent OnInitialized = new AEvent();

        protected List<AModule> m_InitializedModules = new List<AModule>();

        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            if (!IsInitialized)
            {
                HandleInitialization();
                IsInitialized = true;
                OnInitialized.Invoke();
            }
        }

        public void Uninitialize()
        {
            if (IsInitialized)
            {
                ClearModules();
                HandleUninitialization();
                IsInitialized = false;
            }
        }

        protected abstract void HandleInitialization();

        protected abstract void HandleUninitialization();

        protected void AddModule(AModule module)
        {
            if (module != null)
            {
                module.Initialize();
                m_InitializedModules.Add(module);
            }
        }

        protected void RemoveModule(AModule module, bool isClearing = false)
        {
            if (module != null)
            {
                module.Uninitialize();
                if (!isClearing)
                {
                    m_InitializedModules.Remove(module);
                }
            }
        }

        protected void ClearModules()
        {
            for (int num = m_InitializedModules.Count - 1; num >= 0; num--)
            {
                AModule module = m_InitializedModules[num];
                RemoveModule(module, isClearing: true);
            }
            m_InitializedModules.Clear();
        }

        private void OnDestroy()
        {
            Uninitialize();
        }
    }
}