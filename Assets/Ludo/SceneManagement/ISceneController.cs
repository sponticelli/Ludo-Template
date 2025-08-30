namespace Ludo.SceneManagement
{
    public interface ISceneController
    {
        bool IsLoaded(string scenePath);
        void Execute(SceneTransitionPlan plan);

        /// <summary>
        /// Marks a scene as unloadable (protected from unloading) by scene name.
        /// </summary>
        /// <param name="sceneName">The name of the scene to mark as unloadable</param>
        void MarkSceneAsUnloadable(string sceneName);

        /// <summary>
        /// Removes the unloadable status from a scene by scene name.
        /// </summary>
        /// <param name="sceneName">The name of the scene to unmark as unloadable</param>
        void UnmarkSceneAsUnloadable(string sceneName);

        /// <summary>
        /// Checks if a scene is marked as unloadable by scene name.
        /// </summary>
        /// <param name="sceneName">The name of the scene to check</param>
        /// <returns>True if the scene is marked as unloadable, false otherwise</returns>
        bool IsSceneUnloadable(string sceneName);
    }
}