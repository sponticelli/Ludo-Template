#nullable enable
using Game.MainMenu.Credits;
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
        private readonly UICreditsPanel _credits;

        public CreditsState(MainMenuFlowController controller, GameObject main, UICreditsPanel credits)
            : base(controller)
        {
            _main = main;
            _credits = credits;
        }

        public override Awaitable Enter()
        {
            _credits?.Show();
            _credits?.Initialize();
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
                return new HomeState((MainMenuFlowController)Controller, _main, _credits);
            return this;
        }
    }
}