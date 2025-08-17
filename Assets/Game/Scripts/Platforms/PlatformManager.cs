using Game.Platforms.Achievements;
using Ludo.Core;
using Ludo.Core.Events;
using UnityEngine;

namespace Game.Platforms
{
    
    public class TrailerModeChangedEvent : AEvent<bool> { }
    
    
    public class PlatformManager : AModule
    {
        public readonly TrailerModeChangedEvent OnTrailerModeChanged = new TrailerModeChangedEvent();

        public AAchievementManager AchievementManager = new NoDrmAchievementManager();

        public bool IsDesktop => !IsMobile;
        
        
        #if UNITY_IOS || UNITY_ANDROID
        public bool IsMobile => true;
        #else
        public bool IsMobile => false;
        #endif

        public bool IsTablet
        {
            get
            {
                if (IsMobile)
                {
                    return DeviceDiagonalSizeInInches() >= 7f;
                }

                return false;
            }
        }

        public bool IsEditorOrNotMobile
        {
            get
            {
                if (IsMobile)
                {
                    return Application.isEditor;
                }

                return true;
            }
        }

        public bool IsDevelopmentBuildOrEditor
        {
            get
            {
                if (!Debug.isDebugBuild)
                {
                    return Application.isEditor;
                }

                return true;
            }
        }

        public bool IsTrailerMode { get; private set; }

        public bool IsSteamDeck => false;

        protected override void HandleInitialization()
        {
            AchievementManager.Initialize(this);
            if (!Application.isEditor)
            {
                InitializeSteam();
                InitializeIOS();
                InitializeAndroid();
            }
        }

        protected override void HandleUninitialization()
        {
            if (!Application.isEditor)
            {
                UninitializeSteam();
                UninitializeIOS();
                UninitializeAndroid();
            }
        }

        private void Update()
        {
            if (!Application.isEditor)
            {
                UpdateSteam();
                UpdateIOS();
                UpdateAndroid();
            }
        }

        private void ToggleTrailerMode()
        {
            IsTrailerMode = !IsTrailerMode;
            OnTrailerModeChanged.Invoke(IsTrailerMode);
        }

        private static float DeviceDiagonalSizeInInches()
        {
            float num = Screen.dpi;
            if (num == 0f)
            {
                num = 72f;
            }

            float f = (float)Screen.width / num;
            float f2 = (float)Screen.height / num;
            return Mathf.Sqrt(Mathf.Pow(f, 2f) + Mathf.Pow(f2, 2f));
        }

        private void InitializeSteam()
        {
        }

        private void UpdateSteam()
        {
        }

        private void UninitializeSteam()
        {
        }

        public bool IsSteamInitialized()
        {
            return false;
        }

        private void InitializeIOS()
        {
        }

        private void UpdateIOS()
        {
        }

        private void UninitializeIOS()
        {
        }

        private void InitializeAndroid()
        {
        }

        private void UpdateAndroid()
        {
        }

        private void UninitializeAndroid()
        {
        }

        private void OnApplicationQuit()
        {
        }
    }
}