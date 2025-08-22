using Game.Core.Data;
using Ludo.Core;
using Ludo.Core.Events;
using Ludo.Localization;
using Ludo.Pools.Runtime;
using Ludo.Scenes;
using UnityEngine;
using Game.Core.Boot;

namespace Game.Core
{
    /// <summary>
    /// Central application entry point responsible for initializing services and running boot steps.
    /// </summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(AppConst.AppRootExecutionOrder)]
    public class AppRoot : MonoBehaviour
    {
        [Header("Config")]

        /// <summary>
        /// Global settings for the application.
        /// </summary>
        [SerializeField] private GlobalConfig globalConfig;

        /// <summary>
        /// Collection of boot steps executed during startup.
        /// </summary>
        [SerializeField] private BootStep[] bootSteps;

        private static AppRoot _instance;

        /// <summary>
        /// Global access to the application root.
        /// </summary>
        public static AppRoot Instance => _instance;


        /// <summary>
        /// Ensures a single persistent instance and registers services.
        /// </summary>
        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(this);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(this);
            Application.targetFrameRate = globalConfig.TargetFPS;
            RegisterServices();
        }

        /// <summary>
        /// Initializes services and executes boot steps after <see cref="Awake"/>.
        /// </summary>
        private void Start()
        {
            InitializeServices();
            RunBootSteps();
            // publish a boot-complete event here if useful
        }

        /// <summary>
        /// Cleans up services when the root is destroyed.
        /// </summary>
        private void OnDestroy()
        {
            if (_instance != this) return;
            TeardownServices();
            ServiceLocator.Clear();
            _instance = null;
        }

        /// <summary>
        /// Registers core services with the service locator.
        /// </summary>
        private void RegisterServices()
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
        private void InitializeServices()
        {
            // TODO if needed, initialize services
        }

        /// <summary>
        /// Shuts down services in reverse order of initialization.
        /// </summary>
        private void TeardownServices()
        {
            var poolService = ServiceLocator.Get<IPoolService>();
            poolService?.Clear();
            ServiceLocator.Unregister<IPoolService>();
        }

        /// <summary>
        /// Runs configured boot steps in ascending order.
        /// </summary>
        private void RunBootSteps()
        {
            if (bootSteps == null || bootSteps.Length == 0) return;

            System.Array.Sort(bootSteps, (a, b) => a.Order.CompareTo(b.Order));
            foreach (var step in bootSteps)
            {
                try
                {
                    step?.Boot();
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }
}