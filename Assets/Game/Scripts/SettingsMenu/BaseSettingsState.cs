using Ludo.Scenes.Flow;
using UnityEngine;

namespace Game.SettingsMenu
{
    public abstract class BaseSettingsState : FlowState<SettingsMenuEvent>
    {
        public abstract SettingsMenuEvent StateEvent { get; }

        protected SettingsMenuFlowController SettingsController => (SettingsMenuFlowController)Controller;

        public BaseSettingsState(SettingsMenuFlowController controller) : base(controller)
        {

        }

        public override FlowState<SettingsMenuEvent>? Handle(SettingsMenuEvent evt)
        {
            // If the event matches our current state, stay in this state
            if (evt == StateEvent)
                return this;

            // Otherwise, transition to the appropriate state
            return evt switch
            {
                SettingsMenuEvent.Display => new DisplayState(SettingsController),
                SettingsMenuEvent.Sound => new SoundState(SettingsController),
                SettingsMenuEvent.Language => new LanguageState(SettingsController),
                _ => this
            };
        }

        public override Awaitable Enter()
        {
            // Notify the controller about the state change so it can update UI panels
            SettingsController.OnStateChanged(StateEvent);
            return Awaitable.EndOfFrameAsync();
        }
    }
}