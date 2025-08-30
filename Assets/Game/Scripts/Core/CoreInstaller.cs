using Game.Audio;
using Game.Configurations;
using Ludo.Audio;
using Ludo.Core;
using Ludo.Events.Hub;
using Ludo.Localization;
using Ludo.Pools;
using Ludo.SceneManagement;
using UnityEngine;
using AudioConfiguration = Game.Configurations.AudioConfiguration;

namespace Game.Core
{
    [DefaultExecutionOrder(ServiceExecutionOrder.Installer)]
    public class CoreInstaller : ServiceInstaller
    {
        [Header("Configuration")]
        [SerializeField] private LocalizationConfiguration localizationConfiguration;
        [SerializeField] private AudioConfiguration audioConfiguration;

        [Header("Mono Services")]
        [SerializeField] private SceneController sceneController;
        [SerializeField] private AudioService audioService;
        [SerializeField] private AudioManager audioManager;
        [SerializeField] private PoolService poolService;
        
        
        protected override void Install()
        {
            RegisterLocalizationService();
            ServiceLocator.Register<IEventHub>(new EventHub());
            ServiceLocator.Register<ISceneController>(sceneController);
            ServiceLocator.Register<IAudioService>(audioService);
            ServiceLocator.Register<IAudioManager>(audioManager);
            ServiceLocator.Register<IPoolService>(poolService);
        }

        public override void Initialize()
        {
            audioManager.Initialize(audioConfiguration);
        }
        
        protected override void Uninstall()
        {
            ServiceLocator.Unregister<IPoolService>();
            ServiceLocator.Unregister<IAudioManager>();
            ServiceLocator.Unregister<IAudioService>();
            ServiceLocator.Unregister<ILocalizationService>();
            ServiceLocator.Unregister<ISceneController>();
            ServiceLocator.Unregister<IEventHub>();
        }
        
        private void RegisterLocalizationService()
        {
            if (localizationConfiguration == null)
            {
                Debug.LogError("Localization configuration is null");
                return;
            }
            
            var service = new LocalizationService(localizationConfiguration.DefaultLanguage, 
                localizationConfiguration.Tables);
            ServiceLocator.Register<ILocalizationService>(service);
        }
    }
}