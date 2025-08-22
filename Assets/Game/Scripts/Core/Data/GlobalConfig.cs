using System.Collections.Generic;
using Ludo.Localization;
using UnityEngine;

namespace Game.Core.Data
{
    [CreateAssetMenu(fileName = "GlobalConfig", menuName = "Game/GlobalConfig")]
    public class GlobalConfig : ScriptableObject
    {
        #region Application Config
        [Header("Application")]
        [SerializeField] private bool isDebug = false;
        [SerializeField] private int targetFPS = 60;
        
        public bool IsDebug => isDebug;
        public int TargetFPS => targetFPS;
        

        #endregion
        
        #region Localization Configs
        [SerializeField] private LocalizedTable[] localizationTables;
        public IEnumerable<LocalizedTable> LocalizationTables => localizationTables;
        #endregion

        #region Audio Config
        [Header("Audio")]
        [Range(0f, 1f)]
        [SerializeField] private float sfxVolume = 1f;

        [Range(0f, 1f)]
        [SerializeField] private float loopVolume = 1f;

        public float SfxVolume => sfxVolume;
        public float LoopVolume => loopVolume;
        #endregion
    }
}