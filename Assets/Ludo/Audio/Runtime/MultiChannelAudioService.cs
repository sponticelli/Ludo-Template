using System;
using System.Collections.Generic;
using Ludo.Core.Services;
using UnityEngine;

namespace Ludo.Audio
{
    /// <summary>
    /// Multi-channel audio service that manages separate PooledAudioService instances
    /// for different audio categories with independent volume control and fade capabilities.
    /// </summary>
    public sealed class MultiChannelAudioService : IMultiChannelAudioService
    {
        private readonly Dictionary<AudioChannel, IPooledAudioService> _channels = new();
        private readonly Dictionary<AudioChannel, float> _channelVolumes = new();
        private readonly MultiChannelAudioServiceConfig _config;
        private readonly List<FadeOperation> _activeFades = new();
        private readonly GameObject _fadeUpdaterObject;
        private readonly AudioFadeUpdater _fadeUpdater;

        // Default channel for backward compatibility
        private const AudioChannel DefaultChannel = AudioChannel.SFX;

        public MultiChannelAudioService(MultiChannelAudioServiceConfig config)
        {
            _config = config ?? new MultiChannelAudioServiceConfig();

            // Create fade updater
            _fadeUpdaterObject = new GameObject("Audio Fade Updater");
            if (_config.hideInHierarchy) _fadeUpdaterObject.hideFlags = HideFlags.HideInHierarchy;
            UnityEngine.Object.DontDestroyOnLoad(_fadeUpdaterObject);
            _fadeUpdater = _fadeUpdaterObject.AddComponent<AudioFadeUpdater>();
            _fadeUpdater.Initialize(this);

            InitializeChannels();
        }

        /// <summary>
        /// Initializes all audio channels with their respective PooledAudioService instances.
        /// </summary>
        private void InitializeChannels()
        {
            foreach (AudioChannel channel in Enum.GetValues(typeof(AudioChannel)))
            {
                var channelConfig = _config.GetChannelConfig(channel);
                
                // Create PooledAudioServiceConfig for this channel
                var poolConfig = new PooledAudioServiceConfig
                {
                    hideInHierarchy = _config.hideInHierarchy,
                    mixerGroup = channelConfig.mixerGroup,
                    initialPoolSize = channelConfig.initialPoolSize,
                    maxPoolSize = channelConfig.maxPoolSize,
                    preWarmPool = _config.preWarmPools
                };

                _channels[channel] = new PooledAudioService(poolConfig);
                _channelVolumes[channel] = channelConfig.defaultVolume;
            }
        }

        #region IAudioService Implementation (Backward Compatibility)

        /// <inheritdoc />
        public void PlayOneShot(AudioClip clip, float vol = 1)
        {
            PlayOneShot(clip, DefaultChannel, vol);
        }

        /// <inheritdoc />
        public IAudioHandle PlayLoop(AudioClip clip, float vol = 1)
        {
            return PlayLoop(clip, DefaultChannel, vol);
        }

        #endregion

        #region Multi-Channel Audio Methods

        /// <summary>
        /// Plays a one-shot audio clip on the specified channel.
        /// </summary>
        public void PlayOneShot(AudioClip clip, AudioChannel channel, float vol = 1)
        {
            if (clip == null) return;
            
            var channelVolume = _channelVolumes[channel];
            _channels[channel].PlayOneShot(clip, vol * channelVolume);
        }

        /// <summary>
        /// Plays a looping audio clip on the specified channel.
        /// </summary>
        public IAudioHandle PlayLoop(AudioClip clip, AudioChannel channel, float vol = 1)
        {
            if (clip == null) return DummyHandle.Instance;
            
            var channelVolume = _channelVolumes[channel];
            var handle = _channels[channel].PlayLoop(clip, vol * channelVolume);
            return new ChannelAudioHandle(handle, channel, vol, this);
        }

        /// <summary>
        /// Sets the volume for a specific audio channel.
        /// </summary>
        public void SetChannelVolume(AudioChannel channel, float volume)
        {
            _channelVolumes[channel] = Mathf.Clamp01(volume);
            // Note: Individual loop volumes are managed by handles
        }

        /// <summary>
        /// Gets the current volume for a specific audio channel.
        /// </summary>
        public float GetChannelVolume(AudioChannel channel)
        {
            return _channelVolumes[channel];
        }

        #endregion

        #region Fade Operations

        /// <summary>
        /// Plays a looping audio clip with fade-in effect.
        /// </summary>
        public IAudioHandle PlayLoopWithFadeIn(AudioClip clip, AudioChannel channel, float targetVolume, float fadeDuration)
        {
            if (clip == null) return DummyHandle.Instance;

            // Start with zero volume
            var handle = PlayLoop(clip, channel, 0f);
            
            // Create fade-in operation
            if (fadeDuration > 0f)
            {
                var fadeOp = new FadeOperation
                {
                    Handle = handle,
                    StartVolume = 0f,
                    TargetVolume = targetVolume,
                    Duration = fadeDuration,
                    StartTime = Time.time,
                    StopAfterFade = false
                };
                _activeFades.Add(fadeOp);
            }
            else
            {
                // Instant volume set
                if (handle is ChannelAudioHandle channelHandle)
                {
                    channelHandle.SetVolume(targetVolume);
                }
            }

            return handle;
        }

        /// <summary>
        /// Fades out a looping audio handle.
        /// </summary>
        public void FadeOutLoop(IAudioHandle handle, float fadeDuration, bool stopAfterFade = true)
        {
            if (handle == null || !handle.IsPlaying) return;

            if (fadeDuration <= 0f)
            {
                if (stopAfterFade) handle.Stop();
                return;
            }

            var currentVolume = 0f;
            if (handle is ChannelAudioHandle channelHandle)
            {
                currentVolume = channelHandle.GetOriginalVolume();
            }

            var fadeOp = new FadeOperation
            {
                Handle = handle,
                StartVolume = currentVolume,
                TargetVolume = 0f,
                Duration = fadeDuration,
                StartTime = Time.time,
                StopAfterFade = stopAfterFade
            };
            _activeFades.Add(fadeOp);
        }

        /// <summary>
        /// Cross-fades between two looping audio clips.
        /// </summary>
        public IAudioHandle CrossFadeLoops(IAudioHandle currentLoop, AudioClip newClip, AudioChannel channel, float crossFadeDuration)
        {
            if (newClip == null) return DummyHandle.Instance;

            var targetVolume = 1f;
            if (currentLoop is ChannelAudioHandle currentChannelHandle)
            {
                targetVolume = currentChannelHandle.GetOriginalVolume();
            }

            // Fade out current loop
            if (currentLoop != null && currentLoop.IsPlaying)
            {
                FadeOutLoop(currentLoop, crossFadeDuration, true);
            }

            // Fade in new loop
            return PlayLoopWithFadeIn(newClip, channel, targetVolume, crossFadeDuration);
        }

        #endregion

        /// <summary>
        /// Updates fade operations. Should be called regularly (e.g., from Update).
        /// </summary>
        public void UpdateFades()
        {
            var currentTime = Time.time;
            
            for (int i = _activeFades.Count - 1; i >= 0; i--)
            {
                var fade = _activeFades[i];
                
                if (fade.Handle == null || !fade.Handle.IsPlaying)
                {
                    _activeFades.RemoveAt(i);
                    continue;
                }

                var elapsed = currentTime - fade.StartTime;
                var progress = Mathf.Clamp01(elapsed / fade.Duration);
                var currentVolume = Mathf.Lerp(fade.StartVolume, fade.TargetVolume, progress);

                if (fade.Handle is ChannelAudioHandle channelHandle)
                {
                    channelHandle.SetVolume(currentVolume);
                }

                if (progress >= 1f)
                {
                    if (fade.StopAfterFade)
                    {
                        fade.Handle.Stop();
                    }
                    _activeFades.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Gets pool statistics for all channels.
        /// </summary>
        public Dictionary<AudioChannel, (int available, int total, int activeLoops, int activeOneShots)> GetAllChannelStats()
        {
            var stats = new Dictionary<AudioChannel, (int, int, int, int)>();
            foreach (var kvp in _channels)
            {
                stats[kvp.Key] = kvp.Value.GetPoolStats();
            }
            return stats;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _activeFades.Clear();

            foreach (var channel in _channels.Values)
            {
                try
                {
                    channel?.Dispose();
                }
                catch (MissingReferenceException)
                {
                    // Channel objects already destroyed, ignore
                }
            }
            _channels.Clear();

            // Clean up fade updater
            if (_fadeUpdaterObject != null)
            {
                try
                {
                    UnityEngine.Object.Destroy(_fadeUpdaterObject);
                }
                catch (MissingReferenceException)
                {
                    // Object already destroyed, ignore
                }
            }
        }

        #region Helper Classes

        private class FadeOperation
        {
            public IAudioHandle Handle;
            public float StartVolume;
            public float TargetVolume;
            public float Duration;
            public float StartTime;
            public bool StopAfterFade;
        }

        private sealed class ChannelAudioHandle : IAudioHandle
        {
            private readonly IVolumeControlledAudioHandle _innerHandle;
            private readonly AudioChannel _channel;
            private readonly MultiChannelAudioService _service;
            private float _originalVolume;

            public ChannelAudioHandle(IAudioHandle innerHandle, AudioChannel channel, float originalVolume, MultiChannelAudioService service)
            {
                _innerHandle = innerHandle as IVolumeControlledAudioHandle;
                _channel = channel;
                _originalVolume = originalVolume;
                _service = service;
            }

            public bool IsPlaying => _innerHandle?.IsPlaying ?? false;

            public void Stop() => _innerHandle?.Stop();

            public float GetOriginalVolume() => _originalVolume;

            public void SetVolume(float volume)
            {
                _originalVolume = volume;
                var channelVolume = _service.GetChannelVolume(_channel);
                _innerHandle?.SetVolume(volume * channelVolume);
            }
        }

        private sealed class DummyHandle : IAudioHandle
        {
            public static readonly DummyHandle Instance = new DummyHandle();
            public bool IsPlaying => false;
            public void Stop() { }
        }

        #endregion
    }
}
