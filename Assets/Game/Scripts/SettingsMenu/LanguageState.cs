namespace Game.SettingsMenu
{
    public class LanguageState : BaseSettingsState
    {
        public override SettingsMenuEvent StateEvent => SettingsMenuEvent.Language;
        public LanguageState(SettingsMenuFlowController controller) : base(controller)
        {
            
        }
    }
}