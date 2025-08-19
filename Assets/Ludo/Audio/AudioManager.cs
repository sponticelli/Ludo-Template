using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Ludo.Core;

namespace Ludo.Audio
{
    public class AudioManager : AModule
    {
        private AudioConfig m_Config;
        private AudioVolumeManager m_VolumeManager;

        private Dictionary<AudioChannel, Queue<AudioSource>> m_AudioSourcePools;
        private Dictionary<AudioChannel, List<AudioSource>> m_ActiveAudioSources;
        private Dictionary<AudioChannel, AudioMixerGroup> m_ChannelGroups;
        private List<AudioHandle> m_ActiveHandles;
        
        // Music system
        private AudioSource m_MusicSource1;
        private AudioSource m_MusicSource2;
        private bool m_UsingMusicSource1 = true;
        private AudioPlaylist m_CurrentPlaylist;
        private Coroutine m_CrossfadeCoroutine;
        private bool m_IsMusicPaused;

        // Runtime volume values
        private float m_MasterVolume = 1f;
        private float m_SFXVolume = 1f;
        private float m_UIVolume = 1f;
        private float m_MusicVolume = 1f;
        private float m_AmbienceVolume = 1f;

        public float MasterVolume 
        { 
            get => m_VolumeManager?.MasterVolume ?? m_MasterVolume; 
            set 
            { 
                if (m_VolumeManager != null)
                {
                    m_VolumeManager.MasterVolume = value;
                }
                else
                {
                    m_MasterVolume = Mathf.Clamp01(value);
                    UpdateMixerVolume(m_Config?.MasterVolumeParameter ?? "MasterVolume", m_MasterVolume);
                }
            }
        }

        public float SFXVolume 
        { 
            get => m_VolumeManager?.SFXVolume ?? m_SFXVolume; 
            set 
            { 
                if (m_VolumeManager != null)
                {
                    m_VolumeManager.SFXVolume = value;
                }
                else
                {
                    m_SFXVolume = Mathf.Clamp01(value);
                    UpdateMixerVolume(m_Config?.SFXVolumeParameter ?? "SFXVolume", m_SFXVolume);
                }
            }
        }

        public float UIVolume 
        { 
            get => m_VolumeManager?.UIVolume ?? m_UIVolume; 
            set 
            { 
                if (m_VolumeManager != null)
                {
                    m_VolumeManager.UIVolume = value;
                }
                else
                {
                    m_UIVolume = Mathf.Clamp01(value);
                    UpdateMixerVolume(m_Config?.UIVolumeParameter ?? "UIVolume", m_UIVolume);
                }
            }
        }

        public float MusicVolume 
        { 
            get => m_VolumeManager?.MusicVolume ?? m_MusicVolume; 
            set 
            { 
                if (m_VolumeManager != null)
                {
                    m_VolumeManager.MusicVolume = value;
                }
                else
                {
                    m_MusicVolume = Mathf.Clamp01(value);
                    UpdateMixerVolume(m_Config?.MusicVolumeParameter ?? "MusicVolume", m_MusicVolume);
                }
            }
        }

        public float AmbienceVolume 
        { 
            get => m_VolumeManager?.AmbienceVolume ?? m_AmbienceVolume; 
            set 
            { 
                if (m_VolumeManager != null)
                {
                    m_VolumeManager.AmbienceVolume = value;
                }
                else
                {
                    m_AmbienceVolume = Mathf.Clamp01(value);
                    UpdateMixerVolume(m_Config?.AmbienceVolumeParameter ?? "AmbienceVolume", m_AmbienceVolume);
                }
            }
        }

        public bool IsMusicPlaying => GetCurrentMusicSource().isPlaying;
        public AudioClip CurrentMusicTrack => GetCurrentMusicSource().clip;
        public AudioPlaylist CurrentPlaylist => m_CurrentPlaylist;
        
        public void Initialize(AudioConfig config, AudioVolumeManager volumeManager = null)
        {
            m_Config = config;
            m_VolumeManager = volumeManager;
            if (!IsInitialized)
            {
                base.Initialize();
            }
        }
        
        protected override void HandleInitialization()
        {
            if (m_Config == null)
            {
                Debug.LogError("AudioManager: No AudioConfig provided. Initialization failed.");
                return;
            }
            
            InitializeChannelGroups();
            InitializeAudioSourcePools();
            InitializeMusicSources();
            
            // Initialize volumes through VolumeManager if available
            if (m_VolumeManager != null)
            {
                m_VolumeManager.Initialize(m_Config, this);
            }
            else
            {
                InitializeVolumes();
            }
            
            m_ActiveHandles = new List<AudioHandle>();
            m_CurrentPlaylist = m_Config.DefaultPlaylist;
        }

        protected override void HandleUninitialization()
        {
            StopAllAudio();
            
            if (m_CrossfadeCoroutine != null)
            {
                StopCoroutine(m_CrossfadeCoroutine);
                m_CrossfadeCoroutine = null;
            }

            foreach (var handle in m_ActiveHandles)
            {
                handle?.Invalidate();
            }
            m_ActiveHandles?.Clear();
        }

        private void InitializeChannelGroups()
        {
            m_ChannelGroups = new Dictionary<AudioChannel, AudioMixerGroup>
            {
                { AudioChannel.SFX, m_Config.SFXGroup },
                { AudioChannel.UI, m_Config.UIGroup },
                { AudioChannel.Music, m_Config.MusicGroup },
                { AudioChannel.Ambience, m_Config.AmbienceGroup }
            };
        }

        private void InitializeAudioSourcePools()
        {
            m_AudioSourcePools = new Dictionary<AudioChannel, Queue<AudioSource>>();
            m_ActiveAudioSources = new Dictionary<AudioChannel, List<AudioSource>>();

            foreach (AudioChannel channel in System.Enum.GetValues(typeof(AudioChannel)))
            {
                m_AudioSourcePools[channel] = new Queue<AudioSource>();
                m_ActiveAudioSources[channel] = new List<AudioSource>();

                // Create initial pool
                for (int i = 0; i < m_Config.InitialPoolSize; i++)
                {
                    CreateAudioSource(channel);
                }
            }
        }

        private void InitializeMusicSources()
        {
            m_MusicSource1 = CreateDedicatedAudioSource("MusicSource1", AudioChannel.Music);
            m_MusicSource2 = CreateDedicatedAudioSource("MusicSource2", AudioChannel.Music);
        }

        private void InitializeVolumes()
        {
            // Set runtime volumes from config defaults
            m_MasterVolume = m_Config.MasterVolume;
            m_SFXVolume = m_Config.SFXVolume;
            m_UIVolume = m_Config.UIVolume;
            m_MusicVolume = m_Config.MusicVolume;
            m_AmbienceVolume = m_Config.AmbienceVolume;
            
            // Apply to mixer
            UpdateMixerVolume(m_Config.MasterVolumeParameter, m_MasterVolume);
            UpdateMixerVolume(m_Config.SFXVolumeParameter, m_SFXVolume);
            UpdateMixerVolume(m_Config.UIVolumeParameter, m_UIVolume);
            UpdateMixerVolume(m_Config.MusicVolumeParameter, m_MusicVolume);
            UpdateMixerVolume(m_Config.AmbienceVolumeParameter, m_AmbienceVolume);
        }

        private AudioSource CreateAudioSource(AudioChannel channel)
        {
            GameObject audioObject = new GameObject($"AudioSource_{channel}");
            audioObject.transform.SetParent(transform);
            
            AudioSource source = audioObject.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = m_ChannelGroups[channel];
            source.playOnAwake = false;
            
            m_AudioSourcePools[channel].Enqueue(source);
            return source;
        }

        private AudioSource CreateDedicatedAudioSource(string name, AudioChannel channel)
        {
            GameObject audioObject = new GameObject(name);
            audioObject.transform.SetParent(transform);
            
            AudioSource source = audioObject.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = m_ChannelGroups[channel];
            source.playOnAwake = false;
            source.loop = false;
            
            return source;
        }

        private AudioSource GetAudioSource(AudioChannel channel)
        {
            if (m_AudioSourcePools[channel].Count > 0)
            {
                AudioSource source = m_AudioSourcePools[channel].Dequeue();
                m_ActiveAudioSources[channel].Add(source);
                return source;
            }

            // Pool is empty, create new source if under limit
            if (GetTotalActiveSourcesCount() < m_Config.MaxPoolSize)
            {
                AudioSource newSource = CreateAudioSource(channel);
                m_AudioSourcePools[channel].Dequeue(); // Remove from pool
                m_ActiveAudioSources[channel].Add(newSource);
                return newSource;
            }

            Debug.LogWarning($"AudioManager: Maximum audio sources reached ({m_Config.MaxPoolSize}). Cannot play more audio.");
            return null;
        }

        private void ReturnAudioSource(AudioSource source, AudioChannel channel)
        {
            if (source == null) return;

            source.Stop();
            source.clip = null;
            source.volume = 1f;
            source.pitch = 1f;
            source.loop = false;

            m_ActiveAudioSources[channel].Remove(source);
            m_AudioSourcePools[channel].Enqueue(source);
        }

        private int GetTotalActiveSourcesCount()
        {
            int count = 0;
            foreach (var list in m_ActiveAudioSources.Values)
            {
                count += list.Count;
            }
            return count;
        }

        public void PlayOneShot(AudioClip clip, AudioChannel channel = AudioChannel.SFX, float volume = 1f, float pitch = 1f)
        {
            if (clip == null) return;

            AudioSource source = GetAudioSource(channel);
            if (source == null) return;

            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.loop = false;
            source.Play();

            StartCoroutine(ReturnSourceWhenFinished(source, channel, clip.length));
        }

        public AudioHandle PlayLooping(AudioClip clip, AudioChannel channel = AudioChannel.SFX, float volume = 1f, float pitch = 1f)
        {
            if (clip == null) return null;

            AudioSource source = GetAudioSource(channel);
            if (source == null) return null;

            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.loop = true;
            source.Play();

            AudioHandle handle = new AudioHandle(source, this);
            m_ActiveHandles.Add(handle);
            
            StartCoroutine(MonitorAudioHandle(handle, source, channel));
            return handle;
        }

        public AudioHandle Play(AudioClip clip, AudioChannel channel = AudioChannel.SFX, float volume = 1f, float pitch = 1f, bool loop = false)
        {
            if (loop)
                return PlayLooping(clip, channel, volume, pitch);
            
            if (clip == null) return null;

            AudioSource source = GetAudioSource(channel);
            if (source == null) return null;

            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.loop = false;
            source.Play();

            AudioHandle handle = new AudioHandle(source, this);
            m_ActiveHandles.Add(handle);
            
            StartCoroutine(MonitorAudioHandle(handle, source, channel));
            return handle;
        }

        private IEnumerator ReturnSourceWhenFinished(AudioSource source, AudioChannel channel, float duration)
        {
            yield return new WaitForSeconds(duration);
            ReturnAudioSource(source, channel);
        }

        private IEnumerator MonitorAudioHandle(AudioHandle handle, AudioSource source, AudioChannel channel)
        {
            while (handle.IsValid && source.isPlaying)
            {
                yield return null;
            }

            m_ActiveHandles.Remove(handle);
            handle.Invalidate();
            ReturnAudioSource(source, channel);
        }

        public void PlayMusic(AudioClip clip, bool loop = true, float crossfadeDuration = -1f)
        {
            if (clip == null) return;

            float fadeTime = crossfadeDuration >= 0 ? crossfadeDuration : m_Config.DefaultCrossfadeDuration;
            
            if (m_CrossfadeCoroutine != null)
            {
                StopCoroutine(m_CrossfadeCoroutine);
            }

            m_CrossfadeCoroutine = StartCoroutine(CrossfadeMusic(clip, loop, fadeTime));
        }

        public void PlayMusicFromPlaylist(AudioPlaylist playlist = null)
        {
            if (playlist != null)
                m_CurrentPlaylist = playlist;
            
            if (m_CurrentPlaylist == null || !m_CurrentPlaylist.HasTracks)
            {
                Debug.LogWarning("AudioManager: No valid playlist to play from.");
                return;
            }

            AudioClip nextTrack = m_CurrentPlaylist.CurrentTrack;
            if (nextTrack != null)
            {
                PlayMusic(nextTrack, true);
            }
        }

        public void PlayNextTrack()
        {
            if (m_CurrentPlaylist == null || !m_CurrentPlaylist.HasTracks) return;

            AudioClip nextTrack = m_CurrentPlaylist.GetNextTrack();
            if (nextTrack != null)
            {
                PlayMusic(nextTrack, true);
            }
        }

        public void PlayPreviousTrack()
        {
            if (m_CurrentPlaylist == null || !m_CurrentPlaylist.HasTracks) return;

            AudioClip previousTrack = m_CurrentPlaylist.GetPreviousTrack();
            if (previousTrack != null)
            {
                PlayMusic(previousTrack, true);
            }
        }

        private IEnumerator CrossfadeMusic(AudioClip newClip, bool loop, float duration)
        {
            AudioSource currentSource = GetCurrentMusicSource();
            AudioSource newSource = GetInactiveMusicSource();

            // Setup new source
            newSource.clip = newClip;
            newSource.loop = loop;
            newSource.volume = 0f;
            newSource.Play();

            float elapsed = 0f;
            float startVolume = currentSource.volume;

            // Crossfade
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                currentSource.volume = Mathf.Lerp(startVolume, 0f, t);
                newSource.volume = Mathf.Lerp(0f, 1f, t);
                
                yield return null;
            }

            // Finalize
            currentSource.Stop();
            currentSource.volume = 1f;
            newSource.volume = 1f;
            
            m_UsingMusicSource1 = !m_UsingMusicSource1;
            m_CrossfadeCoroutine = null;
        }

        public void PauseMusic()
        {
            AudioSource currentSource = GetCurrentMusicSource();
            if (currentSource.isPlaying)
            {
                currentSource.Pause();
                m_IsMusicPaused = true;
            }
        }

        public void ResumeMusic()
        {
            if (m_IsMusicPaused)
            {
                AudioSource currentSource = GetCurrentMusicSource();
                currentSource.UnPause();
                m_IsMusicPaused = false;
            }
        }

        public void StopMusic(float fadeOutDuration = -1f)
        {
            float fadeTime = fadeOutDuration >= 0 ? fadeOutDuration : m_Config.DefaultCrossfadeDuration;
            
            if (m_CrossfadeCoroutine != null)
            {
                StopCoroutine(m_CrossfadeCoroutine);
            }

            m_CrossfadeCoroutine = StartCoroutine(FadeOutMusic(fadeTime));
        }

        private IEnumerator FadeOutMusic(float duration)
        {
            AudioSource currentSource = GetCurrentMusicSource();
            float startVolume = currentSource.volume;
            float elapsed = 0f;

            while (elapsed < duration && currentSource.isPlaying)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                currentSource.volume = Mathf.Lerp(startVolume, 0f, t);
                yield return null;
            }

            currentSource.Stop();
            currentSource.volume = 1f;
            m_CrossfadeCoroutine = null;
        }

        public void StopAllAudio()
        {
            // Stop all active handles
            foreach (var handle in m_ActiveHandles)
            {
                handle?.Stop();
            }
            m_ActiveHandles.Clear();

            // Stop all active sources
            foreach (var channelSources in m_ActiveAudioSources.Values)
            {
                foreach (var source in channelSources)
                {
                    if (source != null)
                        source.Stop();
                }
            }

            // Stop music
            m_MusicSource1?.Stop();
            m_MusicSource2?.Stop();
            
            if (m_CrossfadeCoroutine != null)
            {
                StopCoroutine(m_CrossfadeCoroutine);
                m_CrossfadeCoroutine = null;
            }
        }

        public void StopChannel(AudioChannel channel)
        {
            if (m_ActiveAudioSources.ContainsKey(channel))
            {
                foreach (var source in m_ActiveAudioSources[channel])
                {
                    if (source != null)
                        source.Stop();
                }
            }

            // Stop handles for this channel
            for (int i = m_ActiveHandles.Count - 1; i >= 0; i--)
            {
                var handle = m_ActiveHandles[i];
                if (handle != null && !handle.IsValid)
                {
                    m_ActiveHandles.RemoveAt(i);
                }
            }
        }

        private AudioSource GetCurrentMusicSource()
        {
            return m_UsingMusicSource1 ? m_MusicSource1 : m_MusicSource2;
        }

        private AudioSource GetInactiveMusicSource()
        {
            return m_UsingMusicSource1 ? m_MusicSource2 : m_MusicSource1;
        }

        public void UpdateMixerVolume(string parameterName, float volume)
        {
            if (m_Config?.MainMixer != null)
            {
                float dbValue = volume > 0 ? Mathf.Log10(volume) * 20 : -80f;
                m_Config.MainMixer.SetFloat(parameterName, dbValue);
            }
        }

        public void UpdateMixerVolumeForChannel(AudioChannel channel, float volume)
        {
            if (m_Config == null) return;
            
            string parameter = m_Config.GetChannelVolumeParameter(channel);
            UpdateMixerVolume(parameter, volume);
        }

        public void SetChannelVolume(AudioChannel channel, float volume)
        {
            if (m_VolumeManager != null)
            {
                m_VolumeManager.SetChannelVolume(channel, volume);
            }
            else
            {
                switch (channel)
                {
                    case AudioChannel.SFX:
                        SFXVolume = volume;
                        break;
                    case AudioChannel.UI:
                        UIVolume = volume;
                        break;
                    case AudioChannel.Music:
                        MusicVolume = volume;
                        break;
                    case AudioChannel.Ambience:
                        AmbienceVolume = volume;
                        break;
                }
            }
        }

        public float GetChannelVolume(AudioChannel channel)
        {
            if (m_VolumeManager != null)
            {
                return m_VolumeManager.GetChannelVolume(channel);
            }
            
            return channel switch
            {
                AudioChannel.SFX => SFXVolume,
                AudioChannel.UI => UIVolume,
                AudioChannel.Music => MusicVolume,
                AudioChannel.Ambience => AmbienceVolume,
                _ => 1f
            };
        }

        // Resource loading helpers
        public void PlayOneShotFromResources(string resourcePath, AudioChannel channel = AudioChannel.SFX, float volume = 1f, float pitch = 1f)
        {
            AudioClip clip = Resources.Load<AudioClip>(resourcePath);
            PlayOneShot(clip, channel, volume, pitch);
        }

        public AudioHandle PlayFromResources(string resourcePath, AudioChannel channel = AudioChannel.SFX, float volume = 1f, float pitch = 1f, bool loop = false)
        {
            AudioClip clip = Resources.Load<AudioClip>(resourcePath);
            return Play(clip, channel, volume, pitch, loop);
        }

        public void PlayMusicFromResources(string resourcePath, bool loop = true, float crossfadeDuration = -1f)
        {
            AudioClip clip = Resources.Load<AudioClip>(resourcePath);
            PlayMusic(clip, loop, crossfadeDuration);
        }

        private void Update()
        {
            CleanupInvalidHandles();
        }

        private void CleanupInvalidHandles()
        {
            for (int i = m_ActiveHandles.Count - 1; i >= 0; i--)
            {
                if (i < m_ActiveHandles.Count && m_ActiveHandles[i] != null && !m_ActiveHandles[i].IsValid)
                {
                    m_ActiveHandles.RemoveAt(i);
                }
            }
        }
    }
}