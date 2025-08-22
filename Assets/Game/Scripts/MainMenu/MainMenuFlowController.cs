using Ludo.Core;
using Ludo.Scenes;
using Ludo.Scenes.Flow;
using UnityEngine;

namespace Game.MainMenu
{
    public enum MainMenuEvent
    {
        StartGame,
        ShowCredits,
        ShowSettings,
        Back
    }

    /// <summary>
    /// Orchestrates the flow of the main menu using Ludo's scene flow framework.
    /// </summary>
    public class MainMenuFlowController : SceneFlowController<MainMenuEvent>
    {
        [Header("Pages")]
        [SerializeField] private GameObject mainPage;
        [SerializeField] private GameObject creditsPage;
        [SerializeField] private GameObject settingsPage;

        protected override FlowState<MainMenuEvent> CreateInitialState()
        {
            return new HomeState(this, mainPage, creditsPage, settingsPage);
        }

        // Methods exposed for UI buttons
        public void StartNewGame() => Machine.Dispatch(MainMenuEvent.StartGame);
        public void ShowCredits() => Machine.Dispatch(MainMenuEvent.ShowCredits);
        public void ShowSettings() => Machine.Dispatch(MainMenuEvent.ShowSettings);
        public void Back() => Machine.Dispatch(MainMenuEvent.Back);
    }

    /// <summary>
    /// Default state showing the primary menu options.
    /// </summary>
    sealed class HomeState : FlowState<MainMenuEvent>
    {
        readonly GameObject _main;
        readonly GameObject _credits;
        readonly GameObject _settings;

        public HomeState(MainMenuFlowController controller, GameObject main, GameObject credits, GameObject settings)
            : base(controller)
        {
            _main = main;
            _credits = credits;
            _settings = settings;
        }

        public override Awaitable Enter()
        {
            _main?.SetActive(true);
            _credits?.SetActive(false);
            _settings?.SetActive(false);
            return default;
        }

        public override FlowState<MainMenuEvent>? Handle(MainMenuEvent evt)
        {
            switch (evt)
            {
                case MainMenuEvent.StartGame:
                {
                    var sceneService = ServiceLocator.Get<ISceneService>();
                    sceneService.Load("Game");
                    return this;
                }
                case MainMenuEvent.ShowCredits:
                    return new CreditsState((MainMenuFlowController)Controller, _main, _credits, _settings);
                case MainMenuEvent.ShowSettings:
                    return new SettingsState((MainMenuFlowController)Controller, _main, _credits, _settings);
            }

            return this;
        }
    }

    /// <summary>
    /// State representing the credits sub page.
    /// </summary>
    sealed class CreditsState : FlowState<MainMenuEvent>
    {
        readonly GameObject _main;
        readonly GameObject _credits;
        readonly GameObject _settings;

        public CreditsState(MainMenuFlowController controller, GameObject main, GameObject credits, GameObject settings)
            : base(controller)
        {
            _main = main;
            _credits = credits;
            _settings = settings;
        }

        public override Awaitable Enter()
        {
            _main?.SetActive(false);
            _credits?.SetActive(true);
            _settings?.SetActive(false);
            return default;
        }

        public override FlowState<MainMenuEvent>? Handle(MainMenuEvent evt)
        {
            if (evt == MainMenuEvent.Back)
                return new HomeState((MainMenuFlowController)Controller, _main, _credits, _settings);
            return this;
        }
    }

    /// <summary>
    /// State representing the settings sub page.
    /// </summary>
    sealed class SettingsState : FlowState<MainMenuEvent>
    {
        readonly GameObject _main;
        readonly GameObject _credits;
        readonly GameObject _settings;

        public SettingsState(MainMenuFlowController controller, GameObject main, GameObject credits, GameObject settings)
            : base(controller)
        {
            _main = main;
            _credits = credits;
            _settings = settings;
        }

        public override Awaitable Enter()
        {
            _main?.SetActive(false);
            _credits?.SetActive(false);
            _settings?.SetActive(true);
            return default;
        }

        public override FlowState<MainMenuEvent>? Handle(MainMenuEvent evt)
        {
            if (evt == MainMenuEvent.Back)
                return new HomeState((MainMenuFlowController)Controller, _main, _credits, _settings);
            return this;
        }
    }
}
