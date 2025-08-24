#nullable enable
using Game.MainMenu.Credits;
using Game.UI;
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
        private readonly UICreditsPanel _credits;

        public HomeState(MainMenuFlowController controller, GameObject main, UICreditsPanel credits)
            : base(controller)
        {
            Debug.Log("HomeState created");
            
            if (main == null)
            {
                Debug.LogError("Main page is not set");
            }
            
            _main = main;
            _credits = credits;
        }

        public override Awaitable Enter()
        {
            Debug.Log("HomeState entered");
            _main?.SetActive(true);
            _credits?.ImmediateHide();
            return Awaitable.EndOfFrameAsync();
        }

        public override FlowState<MainMenuEvent>? Handle(MainMenuEvent evt)
        {
            Debug.Log($"HomeState handling event {evt}");
            switch (evt)
            {
                case MainMenuEvent.StartGame:
                {
                    var sceneService = ServiceLocator.Get<ISceneService>();
                    sceneService.Load("Game");
                    return this;
                }
                case MainMenuEvent.ShowCredits:
                    return new CreditsState((MainMenuFlowController)Controller, _main, _credits);
                case MainMenuEvent.ShowSettings:
                    return new SettingsState((MainMenuFlowController)Controller, _main, _credits);
            }

            return this;
        }
    }
}