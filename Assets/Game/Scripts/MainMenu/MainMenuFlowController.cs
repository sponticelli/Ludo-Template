using Game.Core;
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
        [SerializeField] private GameObject creditsPage;
        [SerializeField] private GameObject settingsPage;
        

        private void Awake()
        {
            Debug.Log($"MainMenuFlowController Awake - mainPage: {mainPage?.name ?? "null"}, creditsPage: {creditsPage?.name ?? "null"}, settingsPage: {settingsPage?.name ?? "null"}");
        }

        protected override FlowState<MainMenuEvent> CreateInitialState()
        {
            Debug.Log($"CreateInitialState called - mainPage: {mainPage?.name ?? "null"}, creditsPage: {creditsPage?.name ?? "null"}, settingsPage: {settingsPage?.name ?? "null"}");
            
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
