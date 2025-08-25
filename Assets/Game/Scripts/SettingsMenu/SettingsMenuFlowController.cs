using Game.Core;
using Game.MainMenu;
using Ludo.Core;
using Ludo.Scenes.Flow;
using UnityEngine;

namespace Game.SettingsMenu
{
    [DefaultExecutionOrder(AppConst.SceneFlowControllerExecutionOrder)]
    public class SettingsMenuFlowController : SceneFlowController<SettingsMenuEvent>
    {
        protected override FlowState<SettingsMenuEvent> CreateInitialState()
        {
            // TODO 
            
            // Fade in Panel
            
            return new DisplayState(this);
        }
        
        
        public void Display() => Machine.Dispatch(SettingsMenuEvent.Display);
        public void Sound() => Machine.Dispatch(SettingsMenuEvent.Sound);
        public void Language() => Machine.Dispatch(SettingsMenuEvent.Language);
        public void Other() => Machine.Dispatch(SettingsMenuEvent.Other);
        
        public void Back() 
        {
            // Fade Panel and then go back to main menu
            var mainMenuFlowController = ServiceLocator.Get<MainMenuFlowController>();
            mainMenuFlowController?.Back();
        }
    }
}