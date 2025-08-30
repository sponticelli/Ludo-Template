using System.Collections;
using Game.Core;
using Ludo.Core;
using Ludo.SceneManagement;
using UnityEngine;

namespace Game.Sessions
{
    public class SessionSceneManager : MonoBehaviour
    {
        [SerializeField] private float waitTime = 1f;
        
        
        public void OnServiceInstalled()
        {
            StartCoroutine(WaitAndLoad());
        }

        private static void LoadMainMenu()
        {
            var sceneController = ServiceLocator.Get<ISceneController>();
            sceneController.MarkSceneAsUnloadable(SceneDatabase.Session);
            sceneController.Execute(SceneTransitionPlan.Begin()
                .WithOverlay(true)
                .Load(SceneDatabase.MainMenu)
                .SetActive(SceneDatabase.MainMenu));
        }

        private IEnumerator WaitAndLoad()
        {
            yield return new WaitForSeconds(waitTime);
            LoadMainMenu();
        }
    }
}