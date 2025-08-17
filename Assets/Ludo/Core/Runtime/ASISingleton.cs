using System.Collections.Generic;
using UnityEngine;

namespace Ludo.Core
{
    public abstract class ASISingleton<T> : CachedMonoBehaviour where T : ASISingleton<T>
    {
        protected static T _Instance;

        private readonly List<AModule> _initializedModules = new List<AModule>();

        public static T Instance
        {
            get
            {
                if (_Instance != null) return _Instance;
                T val = (_Instance = new GameObject($"[{typeof(T)}]").AddComponent<T>());
                _Instance.TryInitialize();
                val.hideFlags = HideFlags.DontSaveInEditor;
                return _Instance;
            }
        }

        protected virtual bool IsDontDestroyOnLoad => true;

        public bool IsInitialized { get; protected set; }
        

        protected virtual void Awake()
        {
            if (_Instance == null)
            {
                _Instance = this as T;
            }
            else
            {
                Destroy(gameObject);
            }
            if (IsDontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        private void OnApplicationQuit()
        {
            TryUninitialize();
        }

        private void OnDestroy()
        {
            TryUninitialize();
        }

        protected void TryInitialize()
        {
            if (!IsInitialized)
            {
                _initializedModules.Clear();
                HandleInitialization();
                IsInitialized = true;
            }
        }

        protected virtual void HandleInitialization()
        {
        }

        protected void TryUninitialize()
        {
            if (IsInitialized)
            {
                UninitializeModules();
                HandleUninitialization();
                IsInitialized = false;
            }
        }

        protected virtual void HandleUninitialization()
        {
        }

        protected T1 CreateChildModule<T1>() where T1 : AModule
        {
            T1 val = new GameObject($"[{typeof(T1).ToString()}]").AddComponent<T1>();
            val.transform.SetParent(transform);
            val.Initialize();
            _initializedModules.Add(val);
            return val;
        }

        private void UninitializeChildModule(AModule module)
        {
            if (module != null)
            {
                module.Uninitialize();
                _initializedModules.Remove(module);
                Destroy(module.gameObject);
            }
        }

        private void UninitializeModules()
        {
            List<AModule> list = new List<AModule>(_initializedModules);
            for (int i = 0; i < list.Count; i++)
            {
                UninitializeChildModule(list[i]);
            }
            _initializedModules.Clear();
        }
    }
}