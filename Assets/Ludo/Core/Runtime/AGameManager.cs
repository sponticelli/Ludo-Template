using System.Collections.Generic;
using Ludo.Core.Events;

namespace Ludo.Core
{
    /// <summary>
    ///     Base implementation for the main game manager. The manager acts as a
    ///     self-instantiating singleton and is responsible for initializing and
    ///     tracking game modules as well as providing a central <see cref="EventBinder" />.
    /// </summary>
    public abstract class AGameManager : ASISingleton<AGameManager>
    {
        /// <summary>
        ///     Collection of modules that have been initialized by the game manager.
        /// </summary>
        protected readonly List<AModule> InitializedModules = new List<AModule>();

        /// <summary>
        ///     Binder used for subscribing to and unbinding from events.
        /// </summary>
        protected readonly EventBinder Binder = new EventBinder();

        /// <summary>
        ///     Determines whether the manager should attempt to initialize during <see cref="Awake" />.
        /// </summary>
        protected virtual bool IsInitializeOnAwake => true;

        protected override void Awake()
        {
            base.Awake();
            if (IsInitializeOnAwake)
            {
                TryInitialize();
            }
        }

        protected override void HandleInitialization()
        {
            base.HandleInitialization();
        }

        protected override void HandleUninitialization()
        {
            base.HandleUninitialization();
        }

        /// <summary>
        ///     Initializes the provided module and tracks it for later cleanup.
        /// </summary>
        /// <param name="module">Module to add.</param>
        protected void AddModule(AModule module)
        {
            if (module == null) return;
            module.Initialize();
            InitializedModules.Add(module);
        }

        /// <summary>
        ///     Uninitializes the provided module and optionally removes it from the tracking list.
        /// </summary>
        private void RemoveModule(AModule module, bool isClearing = false)
        {
            if (module == null) return;
            module.Uninitialize();
            if (!isClearing)
            {
                InitializedModules.Remove(module);
            }
        }

        /// <summary>
        ///     Uninitializes and clears all currently tracked modules.
        /// </summary>
        private void ClearModules()
        {
            for (int num = InitializedModules.Count - 1; num >= 0; num--)
            {
                AModule aModule = InitializedModules[num];
                if (aModule != null)
                {
                    RemoveModule(aModule, isClearing: true);
                }
            }
            InitializedModules.Clear();
        }
    }
}