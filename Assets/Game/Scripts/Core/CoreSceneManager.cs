using Ludo.Core;
using Ludo.SceneManagement;
using UnityEngine;

namespace Game.Core
{
    public class CoreSceneManager : MonoBehaviour
    {
        
        #region Editor Only Bootstrap
        #if UNITY_EDITOR
        private static string _currentScene;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureCoreLoaded()
        {
            // Get the current scene name
            _currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (_currentScene == SceneDatabase.Core) return;
            // If Core isn’t open (e.g., you hit Play from a content scene),
            // load it additively and let it bootstrap.
            var core = UnityEngine.SceneManagement.SceneManager.GetSceneByName(SceneDatabase.Core);
            if (!core.isLoaded)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(SceneDatabase.Core, UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
        }
        #endif
        #endregion
        
        public void OnServiceInstalled()
        {
            if (!ServiceLocator.Exist<ISceneController>())
            {
                Debug.LogError("[CoreSceneManager] SceneController not found!");
                return;
            }
            
            var sceneController = ServiceLocator.Get<ISceneController>();
            sceneController.MarkSceneAsUnloadable(SceneDatabase.Core);
            sceneController.Execute(SceneTransitionPlan.Begin()
                .Load(SceneDatabase.Session));

        }
    }
}