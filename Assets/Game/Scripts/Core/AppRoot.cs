using Game.Core.Data;
using Ludo.Core;
using Ludo.Core.Events;
using Ludo.Localization;
using Ludo.Pools.Runtime;
using Ludo.Scenes;
using UnityEngine;

namespace Game.Core
{
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(AppConst.AppRootExecutionOrder)]
    public class AppRoot : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private GlobalConfig globalConfig;

        private static AppRoot _instance;
        public static AppRoot Instance => _instance;
        

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

        private void Start()
        {
            InitializeServices();
            // publish a boot-complete event here if useful
        }

        void OnDestroy()
        {
            if (_instance != this) return;
            TeardownServices();
            ServiceLocator.Clear();
            _instance = null;
        }

        private void RegisterServices()
        {
            var eventHub = new EventHub();
            ServiceLocator.Register<IEventHub>(eventHub);
            
            var localizationService = new LocalizationService(eventHub, "en", globalConfig.LocalizationTables);
            ServiceLocator.Register<ILocalizationService>(localizationService);
            
            ServiceLocator.Register<ISceneService>(new SceneService());
            
            ServiceLocator.Register<IPoolService>(new PoolService());
        }
        
        private void InitializeServices()
        {
            // TODO if needed, initialize services
        }

        private void TeardownServices()
        {
            var poolService = ServiceLocator.Get<IPoolService>();
            poolService?.Clear();
            ServiceLocator.Unregister<IPoolService>();
        }
    }
}