using System;
using UnityEngine;
using Ludo.Core;

namespace Ludo.Audio
{
    public class AudioVolumeManager : AModule
    {
        public static AudioVolumeManager Instance { get; private set; }
        
        private AudioConfig m_Config;
        private AudioManager m_AudioManager;

        // Current runtime volumes
        private float m_MasterVolume = 1f;
        private float m_SFXVolume = 1f;
        private float m_UIVolume = 1f;
        private float m_MusicVolume = 1f;
        private float m_AmbienceVolume = 1f;

        // Events for volume changes
        public event Action<float> OnMasterVolumeChanged;
        public event Action<AudioChannel, float> OnChannelVolumeChanged;

        public float MasterVolume 
        { 
            get => m_MasterVolume; 
            set 
            { 
                float clampedValue = Mathf.Clamp01(value);
                if (Math.Abs(m_MasterVolume - clampedValue) > 0.001f)
                {
                    m_MasterVolume = clampedValue;
                    ApplyMasterVolume();
                    OnMasterVolumeChanged?.Invoke(m_MasterVolume);
                }
            }
        }

        public float SFXVolume 
        { 
            get => m_SFXVolume; 
            set => SetChannelVolume(AudioChannel.SFX, value);
        }

        public float UIVolume 
        { 
            get => m_UIVolume; 
            set => SetChannelVolume(AudioChannel.UI, value);
        }

        public float MusicVolume 
        { 
            get => m_MusicVolume; 
            set => SetChannelVolume(AudioChannel.Music, value);
        }

        public float AmbienceVolume 
        { 
            get => m_AmbienceVolume; 
            set => SetChannelVolume(AudioChannel.Ambience, value);
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Debug.LogWarning("AudioVolumeManager: Multiple instances detected. Destroying duplicate.");
                Destroy(gameObject);
            }
        }

        public void Initialize(AudioConfig config, AudioManager audioManager)
        {
            m_Config = config;
            m_AudioManager = audioManager;
            
            if (!IsInitialized)
            {
                base.Initialize();
            }
        }

        protected override void HandleInitialization()
        {
            if (m_Config == null)
            {
                Debug.LogError("AudioVolumeManager: No AudioConfig provided. Using default values.");
                return;
            }

            LoadDefaultVolumes();
            ApplyAllVolumes();
        }

        protected override void HandleUninitialization()
        {
            OnMasterVolumeChanged = null;
            OnChannelVolumeChanged = null;
        }

        private void LoadDefaultVolumes()
        {
            m_MasterVolume = m_Config.MasterVolume;
            m_SFXVolume = m_Config.SFXVolume;
            m_UIVolume = m_Config.UIVolume;
            m_MusicVolume = m_Config.MusicVolume;
            m_AmbienceVolume = m_Config.AmbienceVolume;
        }

        public void SetChannelVolume(AudioChannel channel, float volume)
        {
            float clampedValue = Mathf.Clamp01(volume);
            float currentVolume = GetChannelVolume(channel);
            
            if (Math.Abs(currentVolume - clampedValue) <= 0.001f) return;

            switch (channel)
            {
                case AudioChannel.SFX:
                    m_SFXVolume = clampedValue;
                    break;
                case AudioChannel.UI:
                    m_UIVolume = clampedValue;
                    break;
                case AudioChannel.Music:
                    m_MusicVolume = clampedValue;
                    break;
                case AudioChannel.Ambience:
                    m_AmbienceVolume = clampedValue;
                    break;
            }

            ApplyChannelVolume(channel);
            OnChannelVolumeChanged?.Invoke(channel, clampedValue);
        }

        public float GetChannelVolume(AudioChannel channel)
        {
            return channel switch
            {
                AudioChannel.SFX => m_SFXVolume,
                AudioChannel.UI => m_UIVolume,
                AudioChannel.Music => m_MusicVolume,
                AudioChannel.Ambience => m_AmbienceVolume,
                _ => 1f
            };
        }

        public void ResetToDefaults()
        {
            if (m_Config == null) return;

            MasterVolume = m_Config.MasterVolume;
            SetChannelVolume(AudioChannel.SFX, m_Config.SFXVolume);
            SetChannelVolume(AudioChannel.UI, m_Config.UIVolume);
            SetChannelVolume(AudioChannel.Music, m_Config.MusicVolume);
            SetChannelVolume(AudioChannel.Ambience, m_Config.AmbienceVolume);
        }

        public void MuteChannel(AudioChannel channel)
        {
            SetChannelVolume(channel, 0f);
        }

        public void UnmuteChannel(AudioChannel channel)
        {
            float defaultVolume = m_Config?.GetChannelDefaultVolume(channel) ?? 1f;
            SetChannelVolume(channel, defaultVolume);
        }

        public bool IsChannelMuted(AudioChannel channel)
        {
            return GetChannelVolume(channel) <= 0.001f;
        }

        public void MuteMaster()
        {
            MasterVolume = 0f;
        }

        public void UnmuteMaster()
        {
            MasterVolume = m_Config?.MasterVolume ?? 1f;
        }

        public bool IsMasterMuted()
        {
            return MasterVolume <= 0.001f;
        }

        private void ApplyMasterVolume()
        {
            if (m_AudioManager != null)
            {
                m_AudioManager.MasterVolume = m_MasterVolume;
            }
        }

        private void ApplyChannelVolume(AudioChannel channel)
        {
            if (m_AudioManager != null)
            {
                m_AudioManager.SetChannelVolume(channel, GetChannelVolume(channel));
            }
        }

        private void ApplyAllVolumes()
        {
            ApplyMasterVolume();
            ApplyChannelVolume(AudioChannel.SFX);
            ApplyChannelVolume(AudioChannel.UI);
            ApplyChannelVolume(AudioChannel.Music);
            ApplyChannelVolume(AudioChannel.Ambience);
        }

        // Convenience methods for common pitch variance patterns
        public void PlayOneShotWithVariance(AudioClip clip, AudioChannel channel = AudioChannel.SFX, 
            float volume = 1f, float pitchMin = 0.9f, float pitchMax = 1.1f)
        {
            float randomPitch = UnityEngine.Random.Range(pitchMin, pitchMax);
            m_AudioManager?.PlayOneShot(clip, channel, volume, randomPitch);
        }

        public AudioHandle PlayWithVariance(AudioClip clip, AudioChannel channel = AudioChannel.SFX, 
            float volume = 1f, float pitchMin = 0.9f, float pitchMax = 1.1f, bool loop = false)
        {
            float randomPitch = UnityEngine.Random.Range(pitchMin, pitchMax);
            return m_AudioManager?.Play(clip, channel, volume, randomPitch, loop);
        }

        // Save/Load functionality (can be extended to integrate with save system)
        public AudioVolumeSettings GetCurrentSettings()
        {
            return new AudioVolumeSettings
            {
                masterVolume = m_MasterVolume,
                sfxVolume = m_SFXVolume,
                uiVolume = m_UIVolume,
                musicVolume = m_MusicVolume,
                ambienceVolume = m_AmbienceVolume
            };
        }

        public void ApplySettings(AudioVolumeSettings settings)
        {
            MasterVolume = settings.masterVolume;
            SetChannelVolume(AudioChannel.SFX, settings.sfxVolume);
            SetChannelVolume(AudioChannel.UI, settings.uiVolume);
            SetChannelVolume(AudioChannel.Music, settings.musicVolume);
            SetChannelVolume(AudioChannel.Ambience, settings.ambienceVolume);
        }
    }

    [System.Serializable]
    public struct AudioVolumeSettings
    {
        public float masterVolume;
        public float sfxVolume;
        public float uiVolume;
        public float musicVolume;
        public float ambienceVolume;
    }
}