namespace Game.SettingsMenu
{
    public class SoundState : BaseSettingsState
    {
        public override SettingsMenuEvent StateEvent => SettingsMenuEvent.Sound;
        public SoundState(SettingsMenuFlowController controller) : base(controller)
        {
            
        }
        
    }
}