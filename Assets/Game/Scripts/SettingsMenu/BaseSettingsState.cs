using Ludo.Scenes.Flow;

namespace Game.SettingsMenu
{
    public abstract class BaseSettingsState : FlowState<SettingsMenuEvent>
    {
        public abstract SettingsMenuEvent StateEvent { get; }
        public BaseSettingsState(SettingsMenuFlowController controller) : base(controller)
        {
            
        }
    }
}