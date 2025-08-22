using Game.Core.Data;
using Ludo.Core;
using Ludo.Core.Events;
using Ludo.Localization;
using Ludo.Pools.Runtime;
using Ludo.Scenes;
using UnityEngine;
using Ludo.Core.Boot;

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
            var poolService = ServiceLocator.Get<IPoolService>();
            poolService?.Clear();
            ServiceLocator.Unregister<IPoolService>();
        }

    }
}