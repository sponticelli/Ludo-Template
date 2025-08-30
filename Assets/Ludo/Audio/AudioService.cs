using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ludo.Audio
{
    public class AudioService : MonoBehaviour, IAudioService
    {
        [Header("Audio Pool Settings")]
        [SerializeField] private int initialPoolSize = 10;
        [SerializeField] private int maxPoolSize = 50;
        
        private readonly Queue<AudioSource> _availableAudioSources = new();
        private readonly List<AudioSource> _activeAudioSources = new();
        private readonly Dictionary<AudioHandle, Coroutine> _fadeCoroutines = new();
        
        private void Awake()
        {
            InitializeAudioPool();
        }
        
        private void InitializeAudioPool()
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                CreateAudioSource();
            }
        }
        
        private AudioSource CreateAudioSource()
        {
            var audioSourceGO = new GameObject($"AudioSource_{_availableAudioSources.Count + _activeAudioSources.Count}");
            audioSourceGO.transform.SetParent(transform);
            
            var audioSource = audioSourceGO.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            
            _availableAudioSources.Enqueue(audioSource);
            return audioSource;
        }
        
        private AudioSource GetAudioSource()
        {
            if (_availableAudioSources.Count == 0)
            {
                if (_activeAudioSources.Count + _availableAudioSources.Count < maxPoolSize)
                {
                    return CreateAudioSource();
                }
                else
                {
                    // Pool is at max capacity, reuse the oldest active source
                    var oldestSource = _activeAudioSources[0];
                    ReturnAudioSource(oldestSource);
                    return GetAudioSource();
                }
            }
            
            var audioSource = _availableAudioSources.Dequeue();
            _activeAudioSources.Add(audioSource);
            return audioSource;
        }
        
        private void ReturnAudioSource(AudioSource audioSource)
        {
            if (audioSource == null) return;
            
            audioSource.Stop();
            audioSource.clip = null;
            audioSource.outputAudioMixerGroup = null;
            audioSource.volume = 1f;
            audioSource.pitch = 1f;
            audioSource.loop = false;
            
            _activeAudioSources.Remove(audioSource);
            _availableAudioSources.Enqueue(audioSource);
        }
        
        public void PlayOneShot(AudioClip audioClip, IChannel channel, float volume = 1f, float pitch = 1f)
        {
            if (audioClip == null || channel == null) return;

            var audioSource = GetAudioSource();
            ConfigureAudioSource(audioSource, audioClip, channel, volume, pitch, false);
            audioSource.Play();

            // Start coroutine to return the audio source when finished
            StartCoroutine(ReturnAudioSourceWhenFinished(audioSource));
        }

        public void PlayOneShotAtPosition(AudioClip audioClip, IChannel channel, Vector3 position, float volume = 1f, float pitch = 1f)
        {
            if (audioClip == null || channel == null) return;

            var audioSource = GetAudioSource();
            ConfigureAudioSourceForSpatialAudio(audioSource, audioClip, channel, position, volume, pitch, false);
            audioSource.Play();

            // Start coroutine to return the audio source when finished
            StartCoroutine(ReturnAudioSourceWhenFinished(audioSource));
        }
        
        public AudioHandle PlayLooping(AudioClip audioClip, IChannel channel, float volume = 1f, float pitch = 1f)
        {
            if (audioClip == null || channel == null) return null;

            var audioSource = GetAudioSource();
            ConfigureAudioSource(audioSource, audioClip, channel, volume, pitch, true);
            audioSource.Play();

            return new AudioHandle(audioSource);
        }

        public AudioHandle PlayLoopingAtPosition(AudioClip audioClip, IChannel channel, Vector3 position, float volume = 1f, float pitch = 1f)
        {
            if (audioClip == null || channel == null) return null;

            var audioSource = GetAudioSource();
            ConfigureAudioSourceForSpatialAudio(audioSource, audioClip, channel, position, volume, pitch, true);
            audioSource.Play();

            return new AudioHandle(audioSource);
        }
        
        public void Stop(AudioHandle handle)
        {
            if (handle?.IsValid != true) return;
            
            // Stop any fade coroutine
            if (_fadeCoroutines.TryGetValue(handle, out var fadeCoroutine))
            {
                StopCoroutine(fadeCoroutine);
                _fadeCoroutines.Remove(handle);
            }
            
            ReturnAudioSource(handle.AudioSource);
            handle.AudioSource = null;
        }
        
        public AudioHandle PlayLoopingWithFadeIn(AudioClip audioClip, IChannel channel, float fadeDuration, float targetVolume = 1f, float pitch = 1f)
        {
            if (audioClip == null || channel == null) return null;

            var audioSource = GetAudioSource();
            ConfigureAudioSource(audioSource, audioClip, channel, 0f, pitch, true);
            audioSource.Play();

            var handle = new AudioHandle(audioSource);
            var fadeCoroutine = StartCoroutine(FadeIn(handle, fadeDuration, targetVolume));
            _fadeCoroutines[handle] = fadeCoroutine;

            return handle;
        }

        public AudioHandle PlayLoopingWithFadeInAtPosition(AudioClip audioClip, IChannel channel, Vector3 position, float fadeDuration, float targetVolume = 1f, float pitch = 1f)
        {
            if (audioClip == null || channel == null) return null;

            var audioSource = GetAudioSource();
            ConfigureAudioSourceForSpatialAudio(audioSource, audioClip, channel, position, 0f, pitch, true);
            audioSource.Play();

            var handle = new AudioHandle(audioSource);
            var fadeCoroutine = StartCoroutine(FadeIn(handle, fadeDuration, targetVolume));
            _fadeCoroutines[handle] = fadeCoroutine;

            return handle;
        }
        
        public void FadeOutAndStop(AudioHandle handle, float fadeDuration)
        {
            if (handle?.IsValid != true) return;
            
            // Stop any existing fade coroutine
            if (_fadeCoroutines.TryGetValue(handle, out var existingCoroutine))
            {
                StopCoroutine(existingCoroutine);
            }
            
            var fadeCoroutine = StartCoroutine(FadeOut(handle, fadeDuration));
            _fadeCoroutines[handle] = fadeCoroutine;
        }
        
        public void PauseAll()
        {
            foreach (var audioSource in _activeAudioSources)
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Pause();
                }
            }
        }
        
        public void UnpauseAll()
        {
            foreach (var audioSource in _activeAudioSources)
            {
                audioSource.UnPause();
            }
        }
        
        private void ConfigureAudioSource(AudioSource audioSource, AudioClip audioClip, IChannel channel, float volume, float pitch, bool loop)
        {
            audioSource.clip = audioClip;
            audioSource.outputAudioMixerGroup = channel.MixerGroup;
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.loop = loop;

            // Reset spatial settings for 2D audio
            audioSource.spatialBlend = 0f;
            audioSource.transform.position = Vector3.zero;
        }

        private void ConfigureAudioSourceForSpatialAudio(AudioSource audioSource, AudioClip audioClip, IChannel channel, Vector3 position, float volume, float pitch, bool loop)
        {
            audioSource.clip = audioClip;
            audioSource.outputAudioMixerGroup = channel.MixerGroup;
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.loop = loop;

            // Configure for 3D spatial audio
            audioSource.spatialBlend = 1f; // Full 3D
            audioSource.transform.position = position;
            audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
            audioSource.minDistance = 1f;
            audioSource.maxDistance = 50f;
        }
        
        private IEnumerator ReturnAudioSourceWhenFinished(AudioSource audioSource)
        {
            yield return new WaitWhile(() => audioSource.isPlaying);
            ReturnAudioSource(audioSource);
        }
        
        private IEnumerator FadeIn(AudioHandle handle, float duration, float targetVolume)
        {
            if (!handle.IsValid) yield break;
            
            var audioSource = handle.AudioSource;
            var startVolume = 0f;
            var elapsed = 0f;
            
            while (elapsed < duration && handle.IsValid)
            {
                elapsed += Time.deltaTime;
                var progress = elapsed / duration;
                audioSource.volume = Mathf.Lerp(startVolume, targetVolume, progress);
                yield return null;
            }
            
            if (handle.IsValid)
            {
                audioSource.volume = targetVolume;
            }
            
            _fadeCoroutines.Remove(handle);
        }
        
        private IEnumerator FadeOut(AudioHandle handle, float duration)
        {
            if (!handle.IsValid) yield break;
            
            var audioSource = handle.AudioSource;
            var startVolume = audioSource.volume;
            var elapsed = 0f;
            
            while (elapsed < duration && handle.IsValid)
            {
                elapsed += Time.deltaTime;
                var progress = elapsed / duration;
                audioSource.volume = Mathf.Lerp(startVolume, 0f, progress);
                yield return null;
            }
            
            _fadeCoroutines.Remove(handle);
            Stop(handle);
        }
        
        private void OnDestroy()
        {
            // Stop all fade coroutines
            foreach (var coroutine in _fadeCoroutines.Values)
            {
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }
            }
            _fadeCoroutines.Clear();
        }
    }
}
