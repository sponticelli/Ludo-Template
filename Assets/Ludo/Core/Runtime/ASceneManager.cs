using System.Collections.Generic;
using Ludo.Core.Events;

namespace Ludo.Core
{
    /// <summary>
    ///     Base class responsible for controlling the lifecycle of modules within a scene.
    ///     The scene manager provides an initialization event and helps keep track of
    ///     modules that are created and destroyed during the scene's lifetime.
    /// </summary>
    public abstract class ASceneManager : CachedMonoBehaviour
    {
        /// <summary>
        ///     Event invoked after the scene manager has successfully initialized.
        /// </summary>
        public readonly AEvent OnInitialized = new AEvent();

        /// <summary>
        ///     List of modules that have been initialized for the scene.
        /// </summary>
        protected List<AModule> m_InitializedModules = new List<AModule>();

        /// <summary>
        ///     Indicates whether the scene manager has been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        ///     Initializes the scene manager and all registered modules.
        /// </summary>
        public void Initialize()
        {
            if (!IsInitialized)
            {
                HandleInitialization();
                IsInitialized = true;
                OnInitialized.Invoke();
            }
        }

        /// <summary>
        ///     Uninitializes the scene manager and all registered modules.
        /// </summary>
        public void Uninitialize()
        {
            if (IsInitialized)
            {
                ClearModules();
                HandleUninitialization();
                IsInitialized = false;
            }
        }

        /// <summary>
        ///     Called when the scene manager initializes.
        /// </summary>
        protected abstract void HandleInitialization();

        /// <summary>
        ///     Called when the scene manager uninitializes.
        /// </summary>
        protected abstract void HandleUninitialization();

        /// <summary>
        ///     Initializes a module and tracks it for cleanup.
        /// </summary>
        protected void AddModule(AModule module)
        {
            if (module != null)
            {
                module.Initialize();
                m_InitializedModules.Add(module);
            }
        }

        /// <summary>
        ///     Uninitializes a module and optionally removes it from tracking.
        /// </summary>
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

        /// <summary>
        ///     Uninitializes and clears all tracked modules.
        /// </summary>
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
