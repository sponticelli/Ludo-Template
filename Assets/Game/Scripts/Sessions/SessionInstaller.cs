using Game.Services;
using Ludo.Core;

namespace Game.Sessions
{
    public class SessionInstaller : ServiceInstaller
    {
       
        private ISettingsService _settingsService;
        private IUserManager _userManager;
        private ICurrencyManager _currencyManager;

        /// <summary>
        /// Install Session Services
        /// </summary>
        protected override void Install()
        {
            _settingsService = new SettingsService();
            _userManager = new UserManager();
            _currencyManager = new CurrencyManager();
            
            ServiceLocator.Register<ISettingsService>(_settingsService);
            ServiceLocator.Register<IUserManager>(_userManager);
            ServiceLocator.Register<ICurrencyManager>(_currencyManager);
        }
        
        /// <summary>
        /// Initialize Session Services
        /// </summary>
        public override void Initialize()
        {
            _settingsService.Initialize();
            _userManager.Initialize();
            _currencyManager.Initialize();
        }

        /// <summary>
        /// Uninstall Session Services
        /// </summary>
        protected override void Uninstall()
        {
            ServiceLocator.Unregister<ISettingsService>();
            ServiceLocator.Unregister<IUserManager>();
            ServiceLocator.Unregister<ICurrencyManager>();
            
        }
    }
}