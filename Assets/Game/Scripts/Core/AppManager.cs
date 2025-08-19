using System;
using System.Collections.Generic;
using Game.Platforms;
using Ludo.Core;
using Ludo.Localization;
using Ludo.Save;
using Ludo.Timers;
using UnityEngine;

namespace Game.Core
{
    public class AppManager : ASISingleton<AppManager>
    {
        private Dictionary<Type, ASceneManager> _sceneManagers = new Dictionary<Type, ASceneManager>();
        private Dictionary<Type, AModule> _modules = new Dictionary<Type, AModule>();


        public PlatformManager Platform { get; private set; }
        public TimeManager Time { get; private set; }
        public ASaveManager Save { get; private set; }
        
        
        #region Modules
        public bool HasModule<T>() where T : AModule
        {
            var type = typeof(T);
            return _modules.ContainsKey(type);
        }

        public T GetModule<T>() where T : AModule
        {
            var type = typeof(T);
            if (_modules.TryGetValue(type, out var module))
            {
                return (T)module;
            }
            return null;
        }

        private void AddModule<T>(T module) where T : AModule
        {
            if (module == null) return;
            var type = typeof(T);
            RemoveModule<T>();
            _modules.Add(type, module);
        }

        private void RemoveModule<T>() where T : AModule
        {
            var type = typeof(T);
            if (!_modules.ContainsKey(type)) return;
            _modules.Remove(type);
        }

        private void RemoveAllModules()
        {
            _modules.Clear();
        }
        

        #endregion
        
        #region Scene Managers
        public bool HasSceneManager<T>() where T : ASceneManager
        {
            var type = typeof(T);
            return _sceneManagers.ContainsKey(type);
        }

        public T GetSceneManager<T>() where T : ASceneManager
        {
            var type = typeof(T);
            if (_sceneManagers.TryGetValue(type, out var sceneManager))
            {
                return (T)sceneManager;
            }

            return null;
        }
        
        public void RegisterSceneManager<T>(T sceneManager) where T : ASceneManager
        {
            if (sceneManager == null) return;   
            TryInitialize();
            Type type = typeof(T);
            UnregisterSceneManager<T>();
            _sceneManagers.Add(type, sceneManager);
            sceneManager.Initialize();
        }
        
        public void UnregisterSceneManager<T>() where T : ASceneManager
        {
            Type type = typeof(T);
            if (!_sceneManagers.TryGetValue(type, out var sceneManager)) return;
            sceneManager.Uninitialize();
            _sceneManagers.Remove(type);
        }
        
        #endregion

        protected override void HandleInitialization()
        {
            base.HandleInitialization();
            InitializePlatform();
            InitializeTime();
            InitializeSave();
            InitializeLocalization();
        }
        
        
        private void InitializePlatform()
        {
            if (HasModule<PlatformManager>()) return;
            
            Platform = CreateChildModule<PlatformManager>();
            AddModule(Platform);
        }

        private void InitializeTime()
        {
            if (HasModule<TimeManager>()) return;

            Time = CreateChildModule<TimeManager>();
            AddModule(Time);
        }
        
        private void InitializeSave()
        {
            if (HasModule<ASaveManager>()) return;

            Save = CreateChildModule<FileSystemSaveManager>();
            AddModule(Save);
        }
        
        private void InitializeLocalization()
        {
            LocalizationManager.Read();
        }

    }
}