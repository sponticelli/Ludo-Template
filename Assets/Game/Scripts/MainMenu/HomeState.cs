#nullable enable
using Ludo.Core;
using Ludo.Scenes;
using Ludo.Scenes.Flow;
using UnityEngine;

namespace Game.MainMenu
{
    /// <summary>
    /// Default state showing the primary menu options.
    /// </summary>
    internal sealed class HomeState : FlowState<MainMenuEvent>
    {
        private readonly GameObject _main;
        private readonly GameObject _credits;
        private readonly GameObject _settings;

        public HomeState(MainMenuFlowController controller, GameObject main, GameObject credits, GameObject settings)
            : base(controller)
        {
            Debug.Log("HomeState created");
            
            if (main == null)
            {
                Debug.LogError("Main page is not set");
            }
            
            _main = main;
            _credits = credits;
            _settings = settings;
        }

        public override Awaitable Enter()
        {
            Debug.Log("HomeState entered");
            _main?.SetActive(true);
            _credits?.SetActive(false);
            _settings?.SetActive(false);
            return Awaitable.EndOfFrameAsync();
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
}