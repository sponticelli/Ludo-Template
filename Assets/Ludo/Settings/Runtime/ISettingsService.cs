using System.Collections.Generic;
using UnityEngine;

namespace Ludo.Settings.Runtime
{
    public interface ISettingsService
    {
        /// <summary>
        /// Gets or sets the current language code for localization.
        /// </summary>
        string Language { get; set; }

        /// <summary>
        /// Gets or sets the VSync setting (0 = off, 1 = on).
        /// </summary>
        int VSync { get; set; }

        /// <summary>
        /// Gets or sets the graphics quality level.
        /// </summary>
        GraphicsQuality Quality { get; set; }

        /// <summary>
        /// Gets or sets the screen resolution as a string (e.g., "1920x1080").
        /// </summary>
        string Resolution { get; set; }

        /// <summary>
        /// Gets or sets the music volume level (0-10 scale).
        /// </summary>
        int MusicVolume { get; set; }

        /// <summary>
        /// Gets the music volume as a formatted percentage string.
        /// </summary>
        string MusicVolumeText { get; }

        /// <summary>
        /// Gets or sets the sound effects volume level (0-10 scale).
        /// </summary>
        int SFXVolume { get; set; }

        /// <summary>
        /// Gets the SFX volume as a formatted percentage string.
        /// </summary>
        string SFXVolumeText { get; }

        /// <summary>
        /// Gets or sets the game UI scale percentage (50-120%).
        /// </summary>
        int GameUIScale { get; set; }

        /// <summary>
        /// Gets the game UI scale as a formatted percentage string.
        /// </summary>
        string GameUIScaleText { get; }

        /// <summary>
        /// Gets or sets whether battery saver mode is enabled (affects frame rate and VSync).
        /// </summary>
        bool BatterySaver { get; set; }

        /// <summary>
        /// Gets or sets whether focus mode is enabled (reduces distractions).
        /// </summary>
        bool FocusMode { get; set; }

        /// <summary>
        /// Gets or sets whether color accessibility features are enabled.
        /// </summary>
        bool IsColorAccessibilityOn { get; set; }

        /// <summary>
        /// Gets or sets whether motion reduction accessibility features are enabled.
        /// </summary>
        bool IsReduceMotionOn { get; set; }

        /// <summary>
        /// Gets or sets whether haptic feedback/vibrations are enabled.
        /// </summary>
        bool Vibrations { get; set; }

        /// <summary>
        /// Gets or sets whether gameplay hints are enabled.
        /// </summary>
        bool IsHintsOn { get; set; }

        /// <summary>
        /// Gets the list of available screen resolutions for the current display.
        /// </summary>
        List<Resolution> AvailableResolutions { get; }

        /// <summary>
        /// Gets whether there are unsaved visual settings changes that need to be applied.
        /// </summary>
        bool DirtyVisuals { get; }

        /// <summary>
        /// Gets or sets the window mode (fullscreen, windowed, etc.).
        /// </summary>
        int WindowMode { get; set; }

        /// <summary>
        /// Gets or sets debug expo mode setting (development/exhibition mode).
        /// </summary>
        bool DEBUG_ExpoMode { get; set; }

        /// <summary>
        /// Gets or sets debug setting to unlock all content.
        /// </summary>
        bool DEBUG_UnlockAll { get; set; }

        /// <summary>
        /// Gets or sets whether the game has been reviewed by the user.
        /// </summary>
        bool HasGameBeenReviewed { get; set; }

        /// <summary>
        /// Calculates the reference resolution for UI scaling based on current UI scale setting.
        /// </summary>
        /// <returns>The reference resolution as a Vector2Int</returns>
        Vector2Int GetGameUIReferenceResolution();

        /// <summary>
        /// Checks if the game audio is currently muted.
        /// </summary>
        /// <returns>True if audio is muted, false otherwise</returns>
        bool IsMute();

        /// <summary>
        /// Mutes or unmutes the game audio, saving previous volume levels when muting.
        /// </summary>
        /// <param name="mute">True to mute audio, false to restore previous volumes</param>
        void MuteSound(bool mute);

        /// <summary>
        /// Forces the mute state without changing volume levels (for temporary muting).
        /// </summary>
        /// <param name="mute">True to force mute, false to unmute</param>
        void ForceMute(bool mute);

        /// <summary>
        /// Forces the color accessibility setting without user interaction.
        /// </summary>
        /// <param name="value">True to enable color accessibility, false to disable</param>
        void ForceColorAccessibility(bool value);

        /// <summary>
        /// Gets the localization key for the current quality setting.
        /// </summary>
        /// <returns>The localization key string for the quality level</returns>
        string GetQualityKey();

        /// <summary>
        /// Updates the vibration/haptic feedback system based on current setting.
        /// </summary>
        void UpdateVibrations();

        /// <summary>
        /// Converts a Unity Resolution object to a string representation.
        /// </summary>
        /// <param name="res">The resolution to convert</param>
        /// <returns>String in format "widthxheight"</returns>
        string Res2Str(Resolution res);

        /// <summary>
        /// Converts a string representation back to a Unity Resolution object.
        /// </summary>
        /// <param name="str">String in format "widthxheight"</param>
        /// <returns>The corresponding Resolution object</returns>
        Resolution Str2Res(string str);

        /// <summary>
        /// Applies all pending visual settings changes (resolution, window mode, etc.).
        /// </summary>
        void ApplyChanges();

        /// <summary>
        /// Resets any unsaved visual settings changes back to their saved values.
        /// </summary>
        void ResetVisuals();

        /// <summary>
        /// Gets the localization key for the current window mode setting.
        /// </summary>
        /// <returns>The localization key string for the window mode</returns>
        string GetWindowModeKey();
    }

    public enum GraphicsQuality
    {
        Low,
        Medium,
        High,
        Ultra
    }
}