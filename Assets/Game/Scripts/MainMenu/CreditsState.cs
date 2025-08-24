#nullable enable
using Game.UI;
using Ludo.Scenes.Flow;
using UnityEngine;

namespace Game.MainMenu
{
    /// <summary>
    /// State representing the credits sub page.
    /// </summary>
    internal sealed class CreditsState : FlowState<MainMenuEvent>
    {
        private readonly GameObject _main;
        private readonly UIPopup _credits;
        private readonly UIPopup _settings;

        public CreditsState(MainMenuFlowController controller, GameObject main, UIPopup credits, UIPopup settings)
            : base(controller)
        {
            _main = main;
            _credits = credits;
            _settings = settings;
        }

        public override Awaitable Enter()
        {
            _credits?.Show();
            _settings?.ImmediateHide();
            return Awaitable.EndOfFrameAsync();
        }
        
        public override Awaitable Exit()
        {
            _credits?.Hide();
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