using System.Collections.Generic;
using UnityEngine;

namespace Ludo.Core
{
    /// <summary>
    ///     Base class for self-instantiating singletons. When accessed for the first
    ///     time, an instance of <typeparamref name="T" /> is created if one does not
    ///     already exist. The singleton optionally persists across scene loads and
    ///     manages the lifecycle of child <see cref="AModule" /> instances.
    /// </summary>
    public abstract class ASISingleton<T> : CachedMonoBehaviour where T : ASISingleton<T>
    {
        /// <summary>
        ///     The backing field for the singleton instance.
        /// </summary>
        protected static T _Instance;

        /// <summary>
        ///     Modules created by the singleton that need to be cleaned up on
        ///     uninitialization.
        /// </summary>
        private readonly List<AModule> _initializedModules = new List<AModule>();

        /// <summary>
        ///     Gets the singleton instance, creating it if necessary and ensuring it is initialized.
        /// </summary>
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

        /// <summary>
        ///     Determines whether the singleton persists across scene loads.
        /// </summary>
        protected virtual bool IsDontDestroyOnLoad => true;

        /// <summary>
        ///     Indicates whether the singleton has been initialized.
        /// </summary>
        public bool IsInitialized { get; protected set; }


        /// <summary>
        ///     Unity Awake callback used to enforce the singleton pattern and optionally
        ///     mark the object to not be destroyed on load.
        /// </summary>
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

        /// <summary>
        ///     Initializes the singleton and clears any previous module registrations.
        /// </summary>
        protected void TryInitialize()
        {
            if (!IsInitialized)
            {
                _initializedModules.Clear();
                HandleInitialization();
                IsInitialized = true;
            }
        }

        /// <summary>
        ///     Called when the singleton initializes.
        /// </summary>
        protected virtual void HandleInitialization()
        {
        }

        /// <summary>
        ///     Uninitializes the singleton and all of its child modules.
        /// </summary>
        protected void TryUninitialize()
        {
            if (IsInitialized)
            {
                UninitializeModules();
                HandleUninitialization();
                IsInitialized = false;
            }
        }

        /// <summary>
        ///     Called when the singleton uninitializes.
        /// </summary>
        protected virtual void HandleUninitialization()
        {
        }

        /// <summary>
        ///     Creates, initializes and tracks a child module attached to the singleton's GameObject.
        /// </summary>
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
        

        /// <summary>
        ///     Uninitializes all tracked modules and clears the collection.
        /// </summary>
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
