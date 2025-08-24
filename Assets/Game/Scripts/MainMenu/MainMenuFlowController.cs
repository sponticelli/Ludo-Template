using Game.Core;
using Game.MainMenu.Credits;
using Game.UI;
using Ludo.Scenes.Flow;
using UnityEngine;

namespace Game.MainMenu
{
    /// <summary>
    /// Orchestrates the flow of the main menu using Ludo's scene flow framework.
    /// </summary>
    [DefaultExecutionOrder(AppConst.SceneFlowControllerExecutionOrder)]
    public class MainMenuFlowController : SceneFlowController<MainMenuEvent>
    {
        [Header("Pages")]
        [SerializeField] private GameObject mainPage;
        [SerializeField] private UICreditsPanel creditsPage;
        [SerializeField] private UIPopup settingsPage;
        
        

        protected override FlowState<MainMenuEvent> CreateInitialState()
        {

            if (mainPage == null)
            {
                Debug.LogError("Main page is not set in MainMenuFlowController");
                return null!;
            }
            
            if (creditsPage == null)
            {
                Debug.LogWarning("Credits page is not set in MainMenuFlowController");
            }
            
            if (settingsPage == null)
            {
                Debug.LogWarning("Settings page is not set in MainMenuFlowController");
            }
            
            return new HomeState(this, mainPage, creditsPage, settingsPage);
        }

        // Methods exposed for UI buttons
        public void StartNewGame() => Machine.Dispatch(MainMenuEvent.StartGame);
        public void ShowCredits() => Machine.Dispatch(MainMenuEvent.ShowCredits);
        public void ShowSettings() => Machine.Dispatch(MainMenuEvent.ShowSettings);
        public void Back() => Machine.Dispatch(MainMenuEvent.Back);
    }
}
