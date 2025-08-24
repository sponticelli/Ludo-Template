#nullable enable
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
        private readonly GameObject _credits;
        private readonly GameObject _settings;

        public SettingsState(MainMenuFlowController controller, GameObject main, GameObject credits, GameObject settings)
            : base(controller)
        {
            _main = main;
            _credits = credits;
            _settings = settings;
        }

        public override Awaitable Enter()
        {
            Debug.Log("SettingsState entered");
            _main?.SetActive(false);
            _credits?.SetActive(false);
            _settings?.SetActive(true);
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