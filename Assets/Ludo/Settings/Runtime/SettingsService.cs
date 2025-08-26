using System;
using System.Collections.Generic;
using System.Linq;
using Game.SettingsMenu.Data;
using UnityEngine;

namespace Ludo.Settings.Runtime
{
    /// <summary>
    /// In-memory settings service that implements ISettingsService interface.
    /// Provides read/write access to all settings without persistence.
    /// </summary>
    public class SettingsService : ISettingsService
    {
        private readonly SettingsData _data;
        private List<Resolution> _availableResolutions;

        // Quality localization keys mapping
        private readonly Dictionary<GraphicsQuality, string> _qualityKeys = new()
        {
            { GraphicsQuality.Low, "settings.quality.low" },
            { GraphicsQuality.Medium, "settings.quality.medium" },
            { GraphicsQuality.High, "settings.quality.high" },
            { GraphicsQuality.Ultra, "settings.quality.ultra" }
        };

        // Window mode localization keys mapping
        private readonly Dictionary<FullScreenMode, string> _windowModeKeys = new()
        {
            { FullScreenMode.ExclusiveFullScreen, "settings.window.fullscreen" },
            { FullScreenMode.FullScreenWindow, "settings.window.borderless" },
            { FullScreenMode.MaximizedWindow, "settings.window.maximized" },
            { FullScreenMode.Windowed, "settings.window.windowed" }
        };

        public SettingsService()
        {
            _data = new SettingsData();
            InitializeAvailableResolutions();
            InitializeDefaults();
        }

        public SettingsService(SettingsData initialData)
        {
            _data = initialData ?? new SettingsData();
            InitializeAvailableResolutions();
            InitializeDefaults();
        }

        #region Language Settings

        public string Language
        {
            get => _data.languageSettingsData.language;
            set => _data.languageSettingsData.language = value ?? "en";
        }

        #endregion

        #region Display Settings

        public int VSync
        {
            get => _data.displaySettings.vSync;
            set
            {
                _data.displaySettings.vSync = Mathf.Clamp(value, 0, 1);
                UpdateVSync();
            }
        }

        public GraphicsQuality Quality
        {
            get => _data.displaySettings.quality;
            set
            {
                _data.displaySettings.quality = value;
                UpdateQuality();
            }
        }

        public string Resolution
        {
            get
            {
                if (string.IsNullOrEmpty(_data.displaySettings.pendingResolution))
                {
                    return _data.displaySettings.resolution;
                }
                return _data.displaySettings.pendingResolution;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _data.displaySettings.pendingResolution = value;
                    _data.displaySettings.dirtyVisuals = true;
                }
            }
        }

        public int GameUIScale
        {
            get => _data.displaySettings.gameUIScale;
            set => _data.displaySettings.gameUIScale = Mathf.Clamp(value, 50, 120);
        }

        public string GameUIScaleText => $"{GameUIScale}%";

        public bool BatterySaver
        {
            get => _data.displaySettings.batterySaver;
            set
            {
                _data.displaySettings.batterySaver = value;
                UpdateBatterySaver();
            }
        }

        public bool FocusMode
        {
            get => _data.displaySettings.focusMode;
            set => _data.displaySettings.focusMode = value;
        }

        public bool IsColorAccessibilityOn
        {
            get => _data.displaySettings.isColorAccessibilityOn;
            set => _data.displaySettings.isColorAccessibilityOn = value;
        }

        public bool IsReduceMotionOn
        {
            get => _data.displaySettings.isReduceMotionOn;
            set => _data.displaySettings.isReduceMotionOn = value;
        }

        public bool IsHintsOn
        {
            get => _data.displaySettings.isHintsOn;
            set => _data.displaySettings.isHintsOn = value;
        }

        public List<Resolution> AvailableResolutions => _availableResolutions;

        public bool DirtyVisuals => _data.displaySettings.dirtyVisuals;

        public int WindowMode
        {
            get
            {
                if (_data.displaySettings.pendingWindowMode >= 0)
                {
                    return _data.displaySettings.pendingWindowMode;
                }
                return _data.displaySettings.windowMode;
            }
            set
            {
                _data.displaySettings.pendingWindowMode = value;
                _data.displaySettings.dirtyVisuals = true;
            }
        }

        public bool DEBUG_ExpoMode
        {
            get => _data.displaySettings.debugExpoMode;
            set => _data.displaySettings.debugExpoMode = value;
        }

        public bool DEBUG_UnlockAll
        {
            get => _data.displaySettings.debugUnlockAll;
            set => _data.displaySettings.debugUnlockAll = value;
        }

        public bool HasGameBeenReviewed
        {
            get => _data.displaySettings.hasGameBeenReviewed;
            set => _data.displaySettings.hasGameBeenReviewed = value;
        }

        #endregion

        #region Sound Settings

        public int MusicVolume
        {
            get => _data.soundSettings.musicVolume;
            set
            {
                _data.soundSettings.musicVolume = Mathf.Clamp(value, 0, 10);
                // Note: In a full implementation, this would call UpdateMusicVolume()
                // which would interface with an audio service
            }
        }

        public string MusicVolumeText => $"{MusicVolume * 10}%";

        public int SFXVolume
        {
            get => _data.soundSettings.sfxVolume;
            set
            {
                _data.soundSettings.sfxVolume = Mathf.Clamp(value, 0, 10);
                // Note: In a full implementation, this would call UpdateSFXVolume()
                // which would interface with an audio service
            }
        }

        public string SFXVolumeText => $"{SFXVolume * 10}%";

        public bool Vibrations
        {
            get => _data.soundSettings.vibrations;
            set
            {
                _data.soundSettings.vibrations = value;
                UpdateVibrations();
            }
        }

        #endregion

        #region Utility Methods

        public Vector2Int GetGameUIReferenceResolution()
        {
            // Base reference resolution (matching the reference implementation)
            var baseResolution = new Vector2Int(1600, 900);
            float scaleFactor = 100f / GameUIScale; // Inverse scaling like the reference

            return new Vector2Int(
                Mathf.RoundToInt(baseResolution.x * scaleFactor),
                Mathf.RoundToInt(baseResolution.y * scaleFactor)
            );
        }

        public bool IsMute()
        {
            return _data.soundSettings.isMuted || _data.soundSettings.forceMuted;
        }

        public void MuteSound(bool mute)
        {
            _data.soundSettings.isMuted = mute;

            if (mute)
            {
                // Save current volumes before muting (like reference implementation)
                _data.soundSettings.previousMusicVolume = _data.soundSettings.musicVolume;
                _data.soundSettings.previousSfxVolume = _data.soundSettings.sfxVolume;
                _data.soundSettings.musicVolume = 0;
                _data.soundSettings.sfxVolume = 0;
            }
            else
            {
                // Restore previous volumes
                _data.soundSettings.musicVolume = _data.soundSettings.previousMusicVolume;
                _data.soundSettings.sfxVolume = _data.soundSettings.previousSfxVolume;
            }

            // Note: In a full implementation, this would call UpdateMusicVolume() and UpdateSFXVolume()
        }

        public void ForceMute(bool mute)
        {
            _data.soundSettings.forceMuted = mute;
        }

        public void ForceColorAccessibility(bool value)
        {
            _data.displaySettings.isColorAccessibilityOn = value;
        }

        public string GetQualityKey()
        {
            _qualityKeys.TryGetValue(Quality, out var key);
            return key ?? "settings.quality.medium";
        }

        public void UpdateVibrations()
        {
            // Implementation would depend on platform-specific vibration systems
            // For now, this is a placeholder that could be extended
            Debug.Log($"Vibrations updated: {Vibrations}");
        }

        public string Res2Str(Resolution res)
        {
            return $"{res.width}x{res.height}";
        }

        public Resolution Str2Res(string str)
        {
            if (string.IsNullOrEmpty(str))
                return new Resolution { width = 1920, height = 1080, refreshRateRatio = Screen.currentResolution.refreshRateRatio };

            var parts = str.Split('x');
            if (parts.Length == 2 &&
                int.TryParse(parts[0], out int width) &&
                int.TryParse(parts[1], out int height))
            {
                return new Resolution
                {
                    width = width,
                    height = height,
                    refreshRateRatio = Screen.currentResolution.refreshRateRatio
                };
            }

            return new Resolution { width = 1920, height = 1080, refreshRateRatio = Screen.currentResolution.refreshRateRatio };
        }

        public void ApplyChanges()
        {
            if (!_data.displaySettings.dirtyVisuals) return;

            // Apply pending resolution changes
            if (!string.IsNullOrEmpty(_data.displaySettings.pendingResolution))
            {
                _data.displaySettings.resolution = _data.displaySettings.pendingResolution;
                _data.displaySettings.pendingResolution = "";
            }

            // Apply pending window mode changes
            if (_data.displaySettings.pendingWindowMode >= 0)
            {
                _data.displaySettings.windowMode = _data.displaySettings.pendingWindowMode;
                _data.displaySettings.pendingWindowMode = -1;
            }

            // Apply the resolution and window mode together
            UpdateResolution();
            UpdateWindowMode();

            _data.displaySettings.dirtyVisuals = false;
        }

        public void ResetVisuals()
        {
            _data.displaySettings.pendingResolution = "";
            _data.displaySettings.pendingWindowMode = -1;
            _data.displaySettings.dirtyVisuals = false;
        }

        public string GetWindowModeKey()
        {
            var windowMode = (FullScreenMode)_data.displaySettings.windowMode;
            _windowModeKeys.TryGetValue(windowMode, out var key);
            return key ?? "settings.window.fullscreen";
        }

        #endregion

        #region Private Methods

        private void InitializeDefaults()
        {
            // Set default resolution to the biggest available resolution
            if (string.IsNullOrEmpty(_data.displaySettings.resolution))
            {
                _data.displaySettings.resolution = Res2Str(BiggestResolution);
            }
        }

        private void InitializeAvailableResolutions()
        {
            _availableResolutions = new List<Resolution>();

            // First, try to get resolutions with the current refresh rate
            var currentRefreshRateRatio = Screen.currentResolution.refreshRateRatio;
            foreach (var resolution in Screen.resolutions)
            {
                if (resolution.refreshRateRatio.Equals(currentRefreshRateRatio))
                {
                    _availableResolutions.Add(resolution);
                }
            }

            // If we have resolutions with current refresh rate, use them
            if (_availableResolutions.Count > 0) return;

            // Otherwise, add unique resolutions (by width/height) regardless of refresh rate
            foreach (var resolution in Screen.resolutions)
            {
                bool exists = _availableResolutions.Any(r =>
                    r.width == resolution.width && r.height == resolution.height);
                if (!exists)
                {
                    _availableResolutions.Add(resolution);
                }
            }

            // Fallback: ensure we have at least the current resolution
            if (_availableResolutions.Count == 0)
            {
                _availableResolutions.Add(Screen.currentResolution);
            }
        }

        private Resolution BiggestResolution
        {
            get
            {
                if (_availableResolutions == null || _availableResolutions.Count == 0)
                {
                    return Screen.currentResolution;
                }
                return _availableResolutions[_availableResolutions.Count - 1];
            }
        }

        private void UpdateVSync()
        {
            Application.targetFrameRate = -1;
            QualitySettings.vSyncCount = VSync;
        }

        private void UpdateQuality()
        {
            QualitySettings.SetQualityLevel((int)Quality);
        }

        private void UpdateBatterySaver()
        {
            // Battery saver affects VSync and frame rate (like reference implementation)
            _data.displaySettings.vSync = BatterySaver ? 0 : 1;
            UpdateVSync(); // Apply the VSync change
            Application.targetFrameRate = BatterySaver ? 30 :
                Mathf.Min((int)Screen.currentResolution.refreshRateRatio.value, 120);
        }

        private void UpdateResolution()
        {
            var resolution = Str2Res(_data.displaySettings.resolution);
            Screen.SetResolution(resolution.width, resolution.height, (FullScreenMode)_data.displaySettings.windowMode);
        }

        private void UpdateWindowMode()
        {
            // Note: In a full implementation, you might want to check for specific platforms
            // like Steam Deck and force certain modes
            Screen.fullScreenMode = (FullScreenMode)_data.displaySettings.windowMode;
        }

        #endregion
    }
}
