using System.Collections;
using UnityEngine;

namespace Ludo.Audio
{
    public interface IAudioService
    {
        /// <summary>
        /// Play a one-shot audio clip with optional volume and pitch parameters
        /// </summary>
        void PlayOneShot(AudioClip audioClip, IChannel channel, float volume = 1f, float pitch = 1f);

        /// <summary>
        /// Play a one-shot audio clip at a specific 3D position
        /// </summary>
        void PlayOneShotAtPosition(AudioClip audioClip, IChannel channel, Vector3 position, float volume = 1f, float pitch = 1f);

        /// <summary>
        /// Start playing a looping audio clip and return a handle to control it
        /// </summary>
        AudioHandle PlayLooping(AudioClip audioClip, IChannel channel, float volume = 1f, float pitch = 1f);

        /// <summary>
        /// Start playing a looping audio clip at a specific 3D position
        /// </summary>
        AudioHandle PlayLoopingAtPosition(AudioClip audioClip, IChannel channel, Vector3 position, float volume = 1f, float pitch = 1f);

        /// <summary>
        /// Stop a specific looping audio by its handle
        /// </summary>
        void Stop(AudioHandle handle);

        /// <summary>
        /// Fade in a looping audio over the specified duration
        /// </summary>
        AudioHandle PlayLoopingWithFadeIn(AudioClip audioClip, IChannel channel, float fadeDuration, float targetVolume = 1f, float pitch = 1f);

        /// <summary>
        /// Fade in a looping audio at a specific 3D position over the specified duration
        /// </summary>
        AudioHandle PlayLoopingWithFadeInAtPosition(AudioClip audioClip, IChannel channel, Vector3 position, float fadeDuration, float targetVolume = 1f, float pitch = 1f);

        /// <summary>
        /// Fade out a looping audio over the specified duration and stop it
        /// </summary>
        void FadeOutAndStop(AudioHandle handle, float fadeDuration);

        /// <summary>
        /// Pause all currently playing audio
        /// </summary>
        void PauseAll();

        /// <summary>
        /// Unpause all currently paused audio
        /// </summary>
        void UnpauseAll();
    }

    /// <summary>
    /// Handle to control a playing audio source
    /// </summary>
    public class AudioHandle
    {
        public AudioSource AudioSource { get; internal set; }
        public bool IsValid => AudioSource != null;
        public bool IsPlaying => IsValid && AudioSource.isPlaying;

        internal AudioHandle(AudioSource audioSource)
        {
            AudioSource = audioSource;
        }
    }
}