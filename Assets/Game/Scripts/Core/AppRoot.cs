using Game.Core.Data;
using Ludo.Core;
using Ludo.Core.Events;
using Ludo.Localization;
using Ludo.Pools.Runtime;
using Ludo.Scenes;
using UnityEngine;
using Ludo.Core.Boot;
using Ludo.Core.Services;
using System;

namespace Game.Core
{
    /// <summary>
    /// Central application entry point responsible for initializing services and running boot steps.
    /// </summary>
    [DefaultExecutionOrder(AppConst.AppRootExecutionOrder)]
    public class AppRoot : AAppRoot
    {
        [Header("Config")]

        /// <summary>
        /// Global settings for the application.
        /// </summary>
        [SerializeField] private GlobalConfig globalConfig;

        /// <summary>
        /// Global access to the application root.
        /// </summary>
        public static new AppRoot Instance => (AppRoot)instance;


        /// <summary>
        /// Called during Awake after singleton setup to configure application-specific settings.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            Application.targetFrameRate = globalConfig.TargetFPS;
        }
        

        /// <summary>
        /// Registers core services with the service locator.
        /// </summary>
        protected override void RegisterServices()
        {
            var eventHub = new EventHub();
            ServiceLocator.Register<IEventHub>(eventHub);

            var localizationService = new LocalizationService(eventHub, "en", globalConfig.LocalizationTables);
            ServiceLocator.Register<ILocalizationService>(localizationService);

            ServiceLocator.Register<ISceneService>(new SceneService());

            ServiceLocator.Register<IPoolService>(new PoolService());

            var audioService = new AudioService(globalConfig.SfxVolume, globalConfig.LoopVolume);
            ServiceLocator.Register<IAudioService>(audioService);
        }

        /// <summary>
        /// Initializes registered services.
        /// </summary>
        protected override void InitializeServices()
        {
            // TODO if needed, initialize services
        }

        /// <summary>
        /// Shuts down services in reverse order of initialization.
        /// </summary>
        protected override void TeardownServices()
        {
            if (ServiceLocator.TryGet<IAudioService>(out var audioService) && audioService is IDisposable disposable)
                disposable.Dispose();
            ServiceLocator.Unregister<IAudioService>();

            if (ServiceLocator.TryGet<IPoolService>(out var poolService))
            {
                poolService.Clear();
                ServiceLocator.Unregister<IPoolService>();
            }
        }

    }
}