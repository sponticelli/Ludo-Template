namespace Game.Core
{
    /// <summary>
    /// Application-wide constants.
    /// </summary>
    public static class AppConst
    {
        /// <summary>
        /// Execution order applied to <see cref="AppRoot"/>.
        /// </summary>
        public const int AppRootExecutionOrder = -1000;
        
        public const int SceneFlowControllerExecutionOrder = AppRootExecutionOrder + 1;
    }
}
