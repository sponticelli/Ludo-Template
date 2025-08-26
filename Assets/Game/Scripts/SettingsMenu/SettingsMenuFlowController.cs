using Game.Core;
using Game.MainMenu;
using Ludo.Core;
using Ludo.Scenes.Flow;
using Ludo.Settings.Runtime;
using UnityEngine;

namespace Game.SettingsMenu
{
    [DefaultExecutionOrder(AppConst.SceneFlowControllerExecutionOrder)]
    public class SettingsMenuFlowController : SceneFlowController<SettingsMenuEvent>
    {
        [Header("UI Panels")]
        [SerializeField] private UISettingsPanel settingsPanel;
        [SerializeField] private UICategoryPanel categoryPanel;
        [SerializeField] private UIDisplayPanel displayPanel;
        [SerializeField] private UIAudioPanel audioPanel;
        [SerializeField] private UILanguagePanel languagePanel;

        private ISettingsService _settingsService;

        protected async void Start()
        {
            // Register this controller with ServiceLocator for access from other systems
            ServiceLocator.Register<SettingsMenuFlowController>(this);

            // Get the settings service
            _settingsService = ServiceLocator.Get<ISettingsService>();

            // Initialize the settings panel
            if (settingsPanel != null)
            {
                settingsPanel.ImmediateShow();
            }

            // Call base Start to initialize the state machine
            base.Start();
        }

        protected void OnDestroy()
        {
            ServiceLocator.Unregister<SettingsMenuFlowController>();
        }

        protected override FlowState<SettingsMenuEvent> CreateInitialState()
        {
            return new DisplayState(this);
        }

        /// <summary>
        /// Called by states when they enter to update the UI panels
        /// </summary>
        public void OnStateChanged(SettingsMenuEvent currentState)
        {
            // Hide all panels first
            if (displayPanel != null) displayPanel.gameObject.SetActive(false);
            if (audioPanel != null) audioPanel.gameObject.SetActive(false);
            if (languagePanel != null) languagePanel.gameObject.SetActive(false);

            // Show the appropriate panel based on current state
            switch (currentState)
            {
                case SettingsMenuEvent.Display:
                    if (displayPanel != null) displayPanel.gameObject.SetActive(true);
                    break;
                case SettingsMenuEvent.Sound:
                    if (audioPanel != null) audioPanel.gameObject.SetActive(true);
                    break;
                case SettingsMenuEvent.Language:
                    if (languagePanel != null) languagePanel.gameObject.SetActive(true);
                    break;
            }

            // Update category panel selection
            if (categoryPanel != null)
                categoryPanel.UpdateSelection(currentState);

            // Update main settings panel
            if (settingsPanel != null)
                settingsPanel.OnStateChanged(currentState);
        }

        // Public methods for UI button callbacks
        public void Display() => Machine.Dispatch(SettingsMenuEvent.Display);
        public void Sound() => Machine.Dispatch(SettingsMenuEvent.Sound);
        public void Language() => Machine.Dispatch(SettingsMenuEvent.Language);

        public void Back()
        {
            // Fade Panel and then go back to main menu
            if (!ServiceLocator.Exist<MainMenuFlowController>()) return;
            var mainMenuFlowController = ServiceLocator.Get<MainMenuFlowController>();
            mainMenuFlowController?.Back();
        }

        /// <summary>
        /// Provides access to the settings service for UI components
        /// </summary>
        public ISettingsService SettingsService => _settingsService;
    }
}