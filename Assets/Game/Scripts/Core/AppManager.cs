using System;
using System.Collections.Generic;
using Ludo.Core;
using Ludo.Localization;
using UnityEngine;

namespace Game.Core
{
    public class AppManager : ASISingleton<AppManager>
    {
        private Dictionary<Type, ASceneManager> _sceneManagers = new Dictionary<Type, ASceneManager>();
        

        
        public void RegisterSceneManager<T>(T sceneManager) where T : ASceneManager
        {
            if (sceneManager == null) return;   
            TryInitialize();
            Type type = typeof(T);
            if (_sceneManagers.ContainsKey(type))
            {
                var previousManager = _sceneManagers[type];
                previousManager.Uninitialize();
            }
            _sceneManagers.Add(type, sceneManager);
            sceneManager.Initialize();
        }
        
        public void UnregisterSceneManager<T>() where T : ASceneManager
        {
            Type type = typeof(T);
            if (_sceneManagers.ContainsKey(type))
            {
                var sceneManager = _sceneManagers[type];
                sceneManager.Uninitialize();
                _sceneManagers.Remove(type);
            }
        }

        protected override void HandleInitialization()
        {
            base.HandleInitialization();
            InitializeLocalization();
        }
        
        
        private void InitializeLocalization()
        {
            LocalizationManager.Read();
        }

    }
}