using System;
using System.Collections.Generic;
using Ludo.Core.Services;
using UnityEngine;

namespace Ludo.Audio
{
    /// <summary>
    /// Interface for multi-channel audio service that manages separate audio channels
    /// with independent volume control and fade capabilities.
    /// </summary>
    public interface IMultiChannelAudioService : IAudioService, IDisposable
    {
        #region Multi-Channel Audio Methods

        /// <summary>
        /// Plays a one-shot audio clip on the specified channel.
        /// </summary>
        /// <param name="clip">Audio clip to play</param>
        /// <param name="channel">Audio channel to play on</param>
        /// <param name="vol">Volume multiplier (0-1)</param>
        void PlayOneShot(AudioClip clip, AudioChannel channel, float vol = 1);

        /// <summary>
        /// Plays a looping audio clip on the specified channel.
        /// </summary>
        /// <param name="clip">Audio clip to loop</param>
        /// <param name="channel">Audio channel to play on</param>
        /// <param name="vol">Volume multiplier (0-1)</param>
        /// <returns>Handle to control the looping audio</returns>
        IAudioHandle PlayLoop(AudioClip clip, AudioChannel channel, float vol = 1);

        /// <summary>
        /// Sets the volume for a specific audio channel.
        /// </summary>
        /// <param name="channel">Audio channel to modify</param>
        /// <param name="volume">New volume level (0-1)</param>
        void SetChannelVolume(AudioChannel channel, float volume);

        /// <summary>
        /// Gets the current volume for a specific audio channel.
        /// </summary>
        /// <param name="channel">Audio channel to query</param>
        /// <returns>Current volume level (0-1)</returns>
        float GetChannelVolume(AudioChannel channel);

        #endregion

        #region Fade Operations

        /// <summary>
        /// Plays a looping audio clip with fade-in effect.
        /// </summary>
        /// <param name="clip">Audio clip to play</param>
        /// <param name="channel">Audio channel to play on</param>
        /// <param name="targetVolume">Target volume after fade-in</param>
        /// <param name="fadeDuration">Duration of fade-in in seconds</param>
        /// <returns>Handle to control the audio</returns>
        IAudioHandle PlayLoopWithFadeIn(AudioClip clip, AudioChannel channel, float targetVolume, float fadeDuration);

        /// <summary>
        /// Fades out a looping audio handle.
        /// </summary>
        /// <param name="handle">Audio handle to fade out</param>
        /// <param name="fadeDuration">Duration of fade-out in seconds</param>
        /// <param name="stopAfterFade">Whether to stop the audio after fade completes</param>
        void FadeOutLoop(IAudioHandle handle, float fadeDuration, bool stopAfterFade = true);

        /// <summary>
        /// Cross-fades between two looping audio clips.
        /// </summary>
        /// <param name="currentLoop">Current playing loop to fade out</param>
        /// <param name="newClip">New clip to fade in</param>
        /// <param name="channel">Audio channel for the new clip</param>
        /// <param name="crossFadeDuration">Duration of the cross-fade in seconds</param>
        /// <returns>Handle to the new audio</returns>
        IAudioHandle CrossFadeLoops(IAudioHandle currentLoop, AudioClip newClip, AudioChannel channel, float crossFadeDuration);

        #endregion

        #region Monitoring

        /// <summary>
        /// Gets pool statistics for all channels.
        /// </summary>
        /// <returns>Dictionary mapping channels to their pool statistics</returns>
        Dictionary<AudioChannel, (int available, int total, int activeLoops, int activeOneShots)> GetAllChannelStats();

        /// <summary>
        /// Updates fade operations. Called automatically by the service.
        /// </summary>
        void UpdateFades();

        #endregion
    }
}
