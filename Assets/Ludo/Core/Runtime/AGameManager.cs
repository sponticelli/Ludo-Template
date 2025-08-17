using System.Collections.Generic;
using Ludo.Core.Events;

namespace Ludo.Core
{
    public abstract class AGameManager : ASISingleton<AGameManager>
    {
        protected readonly List<AModule> InitializedModules = new List<AModule>();

        protected readonly EventBinder Binder = new EventBinder();

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

        protected void AddModule(AModule module)
        {
            if (module == null) return;
            module.Initialize();
            InitializedModules.Add(module);
        }

        private void RemoveModule(AModule module, bool isClearing = false)
        {
            if (module == null) return;
            module.Uninitialize();
            if (!isClearing)
            {
                InitializedModules.Remove(module);
            }
        }

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