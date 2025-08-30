using Ludo.Audio;
using UnityEngine;

namespace Game.Audio
{
    /// <summary>
    /// High-level audio manager interface that provides game-specific audio methods
    /// without exposing the underlying channel system to developers
    /// </summary>
    public interface IAudioManager
    {
        // UI Audio Methods
        /// <summary>
        /// Play a UI sound effect (button clicks, menu sounds, etc.)
        /// </summary>
        void PlayUI(AudioClip audioClip, float volume = 1f, float pitch = 1f);
        
        // SFX Audio Methods
        /// <summary>
        /// Play a sound effect once
        /// </summary>
        void PlaySFX(AudioClip audioClip, float volume = 1f, float pitch = 1f);
        
        /// <summary>
        /// Play a sound effect at a specific 3D position
        /// </summary>
        void PlaySFXAtPosition(AudioClip audioClip, Vector3 position, float volume = 1f, float pitch = 1f);
        
        /// <summary>
        /// Play a looping sound effect and return a handle to control it
        /// </summary>
        AudioHandle PlaySFXLooping(AudioClip audioClip, float volume = 1f, float pitch = 1f);
        
        /// <summary>
        /// Play a looping sound effect at a specific 3D position
        /// </summary>
        AudioHandle PlaySFXLoopingAtPosition(AudioClip audioClip, Vector3 position, float volume = 1f, float pitch = 1f);
        
        // Music Audio Methods
        /// <summary>
        /// Play background music with optional looping
        /// </summary>
        AudioHandle PlayMusic(AudioClip audioClip, bool loop = true, float volume = 1f, float pitch = 1f);
        
        /// <summary>
        /// Play background music with fade-in effect
        /// </summary>
        AudioHandle PlayMusicWithFadeIn(AudioClip audioClip, float fadeDuration, bool loop = true, float targetVolume = 1f, float pitch = 1f);
        
        /// <summary>
        /// Stop current music with fade-out effect
        /// </summary>
        void StopMusicWithFadeOut(float fadeDuration);
        
        /// <summary>
        /// Stop current music immediately
        /// </summary>
        void StopMusic();
        
        // Ambience Audio Methods
        /// <summary>
        /// Play ambient sound (environmental audio, atmosphere, etc.)
        /// </summary>
        AudioHandle PlayAmbience(AudioClip audioClip, bool loop = true, float volume = 1f, float pitch = 1f);
        
        /// <summary>
        /// Play ambient sound at a specific 3D position
        /// </summary>
        AudioHandle PlayAmbienceAtPosition(AudioClip audioClip, Vector3 position, bool loop = true, float volume = 1f, float pitch = 1f);
        
        /// <summary>
        /// Play ambient sound with fade-in effect
        /// </summary>
        AudioHandle PlayAmbienceWithFadeIn(AudioClip audioClip, float fadeDuration, bool loop = true, float targetVolume = 1f, float pitch = 1f);
        
        /// <summary>
        /// Play ambient sound at a specific 3D position with fade-in effect
        /// </summary>
        AudioHandle PlayAmbienceWithFadeInAtPosition(AudioClip audioClip, Vector3 position, float fadeDuration, bool loop = true, float targetVolume = 1f, float pitch = 1f);
        
        // General Control Methods
        /// <summary>
        /// Stop a specific audio handle
        /// </summary>
        void Stop(AudioHandle handle);
        
        /// <summary>
        /// Fade out and stop a specific audio handle
        /// </summary>
        void FadeOutAndStop(AudioHandle handle, float fadeDuration);
        
        /// <summary>
        /// Pause all audio
        /// </summary>
        void PauseAll();
        
        /// <summary>
        /// Unpause all audio
        /// </summary>
        void UnpauseAll();
        
        /// <summary>
        /// Check if the audio manager is properly initialized
        /// </summary>
        bool IsInitialized { get; }
    }
}
