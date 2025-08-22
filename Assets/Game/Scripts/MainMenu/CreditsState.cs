#nullable enable
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
        private readonly GameObject _credits;
        private readonly GameObject _settings;

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