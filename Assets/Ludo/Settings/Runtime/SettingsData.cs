using System;
using Ludo.Settings.Runtime;

namespace Game.SettingsMenu.Data
{
    [Serializable]
    public class SettingsData
    {
        public DisplaySettingsData displaySettings = new();
        public SoundSettingsData soundSettings = new();
        public LanguageSettingsData languageSettingsData = new();
    }


    [Serializable]
    public class LanguageSettingsData
    {
        public string language = "en";
    }

    [Serializable]
    public class SoundSettingsData
    {
        public int musicVolume = 5; // 0-10 scale, default 5 like reference
        public int sfxVolume = 5;   // 0-10 scale, default 5 like reference
        public bool vibrations = true;

        // Mute state tracking
        public bool isMuted = false;
        public int previousMusicVolume = 5;
        public int previousSfxVolume = 5;
        public bool forceMuted = false;
    }

    [Serializable]
    public class DisplaySettingsData
    {
        public int vSync = 1;
        public GraphicsQuality quality = GraphicsQuality.High;
        public string resolution = ""; // Will be set to biggest available resolution on init
        public int gameUIScale = 70; // Default for non-mobile (reference uses 70 for desktop, 100 for mobile)
        public bool batterySaver = false;
        public bool focusMode = false;
        public bool isColorAccessibilityOn = false;
        public bool isReduceMotionOn = false;
        public bool isHintsOn = false; // Reference implementation defaults to false
        public int windowMode = 0; // 0 = ExclusiveFullScreen

        // Debug settings
        public bool debugExpoMode = false;
        public bool debugUnlockAll = false;

        // Other settings
        public bool hasGameBeenReviewed = false;

        // Visual changes tracking
        public bool dirtyVisuals = false;
        public string pendingResolution = "";
        public int pendingWindowMode = -1;
    }
}