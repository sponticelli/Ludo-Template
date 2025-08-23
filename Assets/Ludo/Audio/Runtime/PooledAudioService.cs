using System;
using System.Collections.Generic;
using Ludo.Core.Services;
using UnityEngine;
using UnityEngine.Audio;
using Object = UnityEngine.Object;

namespace Ludo.Audio
{
    /// <summary>
    /// Improved implementation of <see cref="IAudioService"/> that uses AudioSource pooling
    /// to reduce GameObject creation/destruction overhead for looping audio.
    /// </summary>
    public sealed class PooledAudioService : IAudioService, IDisposable
    {
        private readonly Transform _root;
        private readonly PooledAudioServiceConfig _config;

        // AudioSource pooling for both one-shot and looping audio
        private readonly Queue<AudioSource> _availableAudioSources = new();
        private readonly HashSet<AudioSource> _allPooledSources = new();
        private readonly List<OneShotInstance> _oneShotSources = new(); // Track one-shot sources for cleanup
        private int _totalCreatedSources = 0;

        sealed class OneShotInstance
        {
            public AudioSource Source;
            public float EndTime; // Time when the clip finishes
        }

        sealed class LoopInstance
        {
            public AudioSource Source;
            public float Volume;
            public bool IsPooled; // Track if this source came from the pool
        }

        readonly List<LoopInstance> _activeLoops = new();

        public PooledAudioService(PooledAudioServiceConfig config)
        {
            _config = config ?? new PooledAudioServiceConfig();

            // Create root GameObject
            var go = new GameObject("Pooled Audio Service");
            if (_config.hideInHierarchy) go.hideFlags = HideFlags.HideInHierarchy;
            Object.DontDestroyOnLoad(go);
            _root = go.transform;

            // Initialize the AudioSource pool (used for both one-shot and looping)
            if (_config.preWarmPool)
            {
                PreWarmPool();
            }
        }

        /// <summary>
        /// Pre-creates AudioSources for the pool to avoid allocation during gameplay.
        /// </summary>
        private void PreWarmPool()
        {
            for (int i = 0; i < _config.initialPoolSize; i++)
            {
                CreatePooledAudioSource();
            }
        }

        /// <summary>
        /// Creates a new AudioSource for the pool.
        /// </summary>
        private AudioSource CreatePooledAudioSource()
        {
            var sourceGO = new GameObject($"PooledAudioSource_{_totalCreatedSources}");
            sourceGO.transform.SetParent(_root, false);

            var source = sourceGO.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false; // Will be configured per use
            source.outputAudioMixerGroup = _config.mixerGroup;

            // Initially disable the GameObject to save performance
            sourceGO.SetActive(false);

            _availableAudioSources.Enqueue(source);
            _allPooledSources.Add(source);
            _totalCreatedSources++;

            return source;
        }

        /// <summary>
        /// Gets an AudioSource from the pool, creating a new one if necessary and allowed.
        /// </summary>
        private AudioSource GetPooledAudioSource()
        {
            // Try to get from pool first
            if (_availableAudioSources.Count > 0)
            {
                var source = _availableAudioSources.Dequeue();
                source.gameObject.SetActive(true);
                return source;
            }
            
            // Create new one if under max limit (0 means unlimited)
            if (_config.maxPoolSize == 0 || _totalCreatedSources < _config.maxPoolSize)
            {
                var newSource = CreatePooledAudioSource();
                newSource.gameObject.SetActive(true);
                return _availableAudioSources.Dequeue(); // Get the one we just created
            }
            
            // Pool exhausted and at max limit - create temporary AudioSource
            Debug.LogWarning($"AudioSource pool exhausted (max: {_config.maxPoolSize}). Creating temporary AudioSource.");
            return CreateTemporaryAudioSource();
        }

        /// <summary>
        /// Creates a temporary AudioSource that won't be pooled.
        /// </summary>
        private AudioSource CreateTemporaryAudioSource()
        {
            var tempGO = new GameObject("TempAudioSource");
            tempGO.transform.SetParent(_root, false);

            var source = tempGO.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false; // Will be configured per use
            source.outputAudioMixerGroup = _config.mixerGroup;

            return source;
        }

        /// <summary>
        /// Returns an AudioSource to the pool for reuse.
        /// </summary>
        private void ReturnToPool(AudioSource source)
        {
            if (source == null) return;
            
            // Check if this is a pooled source
            if (_allPooledSources.Contains(source))
            {
                // Reset the AudioSource state
                source.Stop();
                source.clip = null;
                source.volume = 1f;
                source.gameObject.SetActive(false);
                
                // Return to pool
                _availableAudioSources.Enqueue(source);
            }
            else
            {
                // This was a temporary source, destroy it
                Object.Destroy(source.gameObject);
            }
        }

        /// <inheritdoc />
        public void PlayOneShot(AudioClip clip, float vol = 1)
        {
            if (clip == null) return;

            // Clean up finished one-shot sources before getting a new one
            CleanupFinishedOneShots();

            var source = GetPooledAudioSource();

            // Configure for one-shot playback
            source.clip = clip;
            source.volume = vol;
            source.loop = false;
            source.Play();

            // Track this source with its end time
            var oneShotInstance = new OneShotInstance
            {
                Source = source,
                EndTime = Time.time + clip.length
            };
            _oneShotSources.Add(oneShotInstance);
        }

        /// <summary>
        /// Cleans up one-shot AudioSources that have finished playing and returns them to the pool.
        /// </summary>
        private void CleanupFinishedOneShots()
        {
            var currentTime = Time.time;
            for (int i = _oneShotSources.Count - 1; i >= 0; i--)
            {
                var oneShot = _oneShotSources[i];
                if (currentTime >= oneShot.EndTime || !oneShot.Source.isPlaying)
                {
                    ReturnToPool(oneShot.Source);
                    _oneShotSources.RemoveAt(i);
                }
            }
        }

        /// <inheritdoc />
        public IAudioHandle PlayLoop(AudioClip clip, float vol = 1)
        {
            if (clip == null) return DummyHandle.Instance;

            var source = GetPooledAudioSource();
            var isPooled = _allPooledSources.Contains(source);
            
            // Configure the AudioSource
            source.clip = clip;
            source.volume = vol;
            source.loop = true;
            source.Play();

            var loop = new LoopInstance 
            { 
                Source = source, 
                Volume = Mathf.Clamp01(vol),
                IsPooled = isPooled
            };
            
            _activeLoops.Add(loop);
            return new PooledLoopHandle(this, loop);
        }

        /// <summary>
        /// Releases a loop instance and returns its AudioSource to the pool if applicable.
        /// </summary>
        void Release(LoopInstance inst)
        {
            if (inst == null) return;
            
            if (inst.Source != null)
            {
                if (inst.IsPooled)
                {
                    ReturnToPool(inst.Source);
                }
                else
                {
                    // Temporary source, destroy it
                    inst.Source.Stop();
                    Object.Destroy(inst.Source.gameObject);
                }
            }
            
            _activeLoops.Remove(inst);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // Stop and clean up all active loops
            foreach (var loop in _activeLoops)
            {
                if (loop.Source != null)
                {
                    loop.Source.Stop();
                    if (!loop.IsPooled)
                    {
                        Object.Destroy(loop.Source.gameObject);
                    }
                }
            }
            _activeLoops.Clear();

            // Stop and clean up all one-shot sources
            foreach (var oneShot in _oneShotSources)
            {
                if (oneShot.Source != null)
                {
                    oneShot.Source.Stop();
                    // One-shot sources are always pooled, so they'll be cleaned up with the pool
                }
            }
            _oneShotSources.Clear();

            // Clean up the pool
            foreach (var source in _allPooledSources)
            {
                if (source != null)
                    Object.Destroy(source.gameObject);
            }
            _availableAudioSources.Clear();
            _allPooledSources.Clear();

            // Destroy the root GameObject
            Object.Destroy(_root.gameObject);
        }

        /// <summary>
        /// Gets current pool statistics for debugging.
        /// </summary>
        public (int available, int total, int activeLoops, int activeOneShots) GetPoolStats()
        {
            return (_availableAudioSources.Count, _totalCreatedSources, _activeLoops.Count, _oneShotSources.Count);
        }

        sealed class PooledLoopHandle : IVolumeControlledAudioHandle
        {
            PooledAudioService _service;
            LoopInstance _instance;

            public PooledLoopHandle(PooledAudioService service, LoopInstance instance)
            {
                _service = service;
                _instance = instance;
            }

            public bool IsPlaying => _instance?.Source != null && _instance.Source.isPlaying;

            public void Stop()
            {
                if (_instance == null) return;
                _service?.Release(_instance);
                _service = null;
                _instance = null;
            }

            /// <summary>
            /// Sets the volume of this specific audio handle.
            /// </summary>
            public void SetVolume(float volume)
            {
                if (_instance?.Source != null)
                {
                    _instance.Volume = Mathf.Clamp01(volume);
                    _instance.Source.volume = _instance.Volume;
                }
            }

            /// <summary>
            /// Gets the current volume of this audio handle.
            /// </summary>
            public float GetVolume()
            {
                return _instance?.Volume ?? 0f;
            }
        }

        sealed class DummyHandle : IAudioHandle
        {
            public static readonly DummyHandle Instance = new DummyHandle();
            public bool IsPlaying => false;
            public void Stop() { }
        }
    }
}
