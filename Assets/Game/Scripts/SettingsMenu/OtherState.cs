namespace Game.SettingsMenu
{
    public class OtherState : BaseSettingsState
    {
        public override SettingsMenuEvent StateEvent => SettingsMenuEvent.Other;
        public OtherState(SettingsMenuFlowController controller) : base(controller)
        {
            
        }
    }
}