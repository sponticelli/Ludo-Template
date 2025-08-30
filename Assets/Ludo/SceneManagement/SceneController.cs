using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ludo.SceneManagement
{
    public class SceneController : MonoBehaviour, ISceneController
    {
        [SerializeField] private LoadingOverlay overlay;

        // Tracks loaded scenes by name (excluding Core which is the hosting scene).
        private readonly HashSet<string> _loaded = new();

        // Tracks scenes marked as unloadable by scene name
        private readonly HashSet<string> _unloadableScenes = new();

        private void Awake()
        {
            // Mark currently loaded additive scenes (usually just Core at boot)
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scn = SceneManager.GetSceneAt(i);
                if (scn.IsValid() && scn.isLoaded)
                    _loaded.Add(scn.path); // use path (matches SceneDatabase constants)
            }
        }

        public bool IsLoaded(string scenePath) => _loaded.Contains(scenePath);

        public void Execute(SceneTransitionPlan plan)
        {
            StartCoroutine(ExecutePlan(plan));
        }

        public void MarkSceneAsUnloadable(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogWarning("[SceneController] Cannot mark null or empty scene name as unloadable");
                return;
            }

            _unloadableScenes.Add(sceneName);
            Debug.Log($"[SceneController] Scene '{sceneName}' marked as unloadable");
        }

        public void UnmarkSceneAsUnloadable(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogWarning("[SceneController] Cannot unmark null or empty scene name");
                return;
            }

            if (_unloadableScenes.Remove(sceneName))
            {
                Debug.Log($"[SceneController] Scene '{sceneName}' unmarked as unloadable");
            }
            else
            {
                Debug.LogWarning($"[SceneController] Scene '{sceneName}' was not marked as unloadable");
            }
        }

        public bool IsSceneUnloadable(string sceneName)
        {
            return !string.IsNullOrEmpty(sceneName) && _unloadableScenes.Contains(sceneName);
        }

        private IEnumerator ExecutePlan(ISceneTransitionPlan plan)
        {
            if (plan.UseOverlay && overlay != null)
                yield return overlay.FadeIn();

            // Prevent duplicate loads/unloads
            plan.ToLoad.RemoveWhere(IsLoaded);
            plan.ToUnload.RemoveWhere(s => !IsLoaded(s));
            
            // Never unload scenes marked as unloadable (safety)
            plan.ToUnload.RemoveWhere(scenePath =>
            {
                // Extract scene name from path for checking unloadable status
                var sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                if (IsSceneUnloadable(sceneName))
                {
                    Debug.LogWarning($"[SceneController] Prevented unloading of protected scene: {sceneName} ({scenePath})");
                    return true;
                }
                return false;
            });

            // Unload phase
            foreach (var s in plan.ToUnload)
            {
                var op = SceneManager.UnloadSceneAsync(s);
                while (op is { isDone: false }) yield return null;
                _loaded.Remove(s);
            }

            // Load phase (additive)
            foreach (var s in plan.ToLoad)
            {
                var op = SceneManager.LoadSceneAsync(s, LoadSceneMode.Additive);
                while (op is { isDone: false }) yield return null;
                _loaded.Add(s);
            }

            // Set active scene (optional)
            if (!string.IsNullOrEmpty(plan.ActiveScene))
            {
                // Try to get scene by path first, then by name if path fails
                var scn = SceneManager.GetSceneByName(plan.ActiveScene);

                if (scn.IsValid() && scn.isLoaded)
                    SceneManager.SetActiveScene(scn);
                else
                    Debug.LogWarning($"[SceneController] Active target not loaded: {plan.ActiveScene}");
            }

            // Cleanup
            if (plan.CleanUnusedAssets)
            {
                var unload = Resources.UnloadUnusedAssets();
                while (!unload.isDone) yield return null;
                System.GC.Collect();
            }

            if (plan.UseOverlay && overlay != null)
                yield return overlay.FadeOut();
        }
    }
}