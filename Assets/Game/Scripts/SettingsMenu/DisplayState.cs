namespace Game.SettingsMenu
{
    public class DisplayState : BaseSettingsState
    {
        public override SettingsMenuEvent StateEvent => SettingsMenuEvent.Display;
        public DisplayState(SettingsMenuFlowController controller) : base(controller)
        {
            
        }
    }
}