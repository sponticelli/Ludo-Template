using System.Collections.Generic;

namespace Ludo.SceneManagement
{
    public sealed class SceneTransitionPlan : ISceneTransitionPlan
    {
        public HashSet<string> ToLoad { get;  }  = new();
        public HashSet<string> ToUnload { get; } = new();
        public string ActiveScene { get; private set; } = null;
        public bool UseOverlay { get; private set; } = true;
        public bool CleanUnusedAssets { get; private set; } = true;

        public static SceneTransitionPlan Begin() => new();

        public SceneTransitionPlan Load(string scene)   { ToLoad.Add(scene); return this; }
        public SceneTransitionPlan Unload(string scene) { ToUnload.Add(scene); return this; }
        public SceneTransitionPlan SetActive(string scene) { ActiveScene = scene; return this; }
        public SceneTransitionPlan WithOverlay(bool on) { UseOverlay = on; return this; }
        public SceneTransitionPlan WithCleanup(bool on) { CleanUnusedAssets = on; return this; }
    }
}