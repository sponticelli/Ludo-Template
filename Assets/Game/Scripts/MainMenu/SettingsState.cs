#nullable enable
using Game.MainMenu.Credits;
using Game.UI;
using Ludo.Scenes.Flow;
using UnityEngine;

namespace Game.MainMenu
{
    /// <summary>
    /// State representing the settings sub page.
    /// </summary>
    internal sealed class SettingsState : FlowState<MainMenuEvent>
    {
        private readonly GameObject _main;
        private readonly UICreditsPanel _credits;
        private readonly UIPopup _settings;

        public SettingsState(MainMenuFlowController controller, GameObject main, UICreditsPanel credits, UIPopup settings)
            : base(controller)
        {
            _main = main;
            _credits = credits;
            _settings = settings;
        }

        public override Awaitable Enter()
        {
            Debug.Log("SettingsState entered");
            _credits?.ImmediateHide();
            _settings?.Show();
            return Awaitable.EndOfFrameAsync();
        }
        
        public override Awaitable Exit()
        {
            Debug.Log("SettingsState exited");
            _settings?.Hide();
            return Awaitable.EndOfFrameAsync();
        }

        public override FlowState<MainMenuEvent>? Handle(MainMenuEvent evt)
        {
            if (evt == MainMenuEvent.Back)
                return new HomeState((MainMenuFlowController)Controller, _main, _credits, _settings);
            return this;
        }
    }
}