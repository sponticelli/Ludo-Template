using UnityEngine;
using UnityEngine.Audio;

namespace Ludo.Audio
{
    [CreateAssetMenu(fileName = "AudioConfig", menuName = "Ludo/Audio/Audio Config")]
    public class AudioConfig : ScriptableObject
    {
        [Header("Audio Mixer Configuration")]
        [SerializeField] private AudioMixer m_MainMixer;
        [SerializeField] private AudioMixerGroup m_SFXGroup;
        [SerializeField] private AudioMixerGroup m_UIGroup;
        [SerializeField] private AudioMixerGroup m_MusicGroup;
        [SerializeField] private AudioMixerGroup m_AmbienceGroup;

        [Header("Audio Source Pool Settings")]
        [SerializeField] private int m_InitialPoolSize = 10;
        [SerializeField] private int m_MaxPoolSize = 50;

        [Header("Music Settings")]
        [SerializeField] private float m_DefaultCrossfadeDuration = 1f;
        [SerializeField] private AudioPlaylist m_DefaultPlaylist;

        [Header("Default Volume Settings")]
        [SerializeField, Range(0f, 1f)] private float m_MasterVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float m_SFXVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float m_UIVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float m_MusicVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float m_AmbienceVolume = 1f;

        [Header("Volume Manager Settings")]
        [SerializeField] private bool m_EnableVolumeManager = true;
        [SerializeField] private bool m_SaveVolumeSettings = true;

        [Header("Audio Mixer Parameters")]
        [SerializeField] private string m_MasterVolumeParameter = "MasterVolume";
        [SerializeField] private string m_SFXVolumeParameter = "SFXVolume";
        [SerializeField] private string m_UIVolumeParameter = "UIVolume";
        [SerializeField] private string m_MusicVolumeParameter = "MusicVolume";
        [SerializeField] private string m_AmbienceVolumeParameter = "AmbienceVolume";

        public AudioMixer MainMixer => m_MainMixer;
        public AudioMixerGroup SFXGroup => m_SFXGroup;
        public AudioMixerGroup UIGroup => m_UIGroup;
        public AudioMixerGroup MusicGroup => m_MusicGroup;
        public AudioMixerGroup AmbienceGroup => m_AmbienceGroup;

        public int InitialPoolSize => m_InitialPoolSize;
        public int MaxPoolSize => m_MaxPoolSize;

        public float DefaultCrossfadeDuration => m_DefaultCrossfadeDuration;
        public AudioPlaylist DefaultPlaylist => m_DefaultPlaylist;

        public float MasterVolume => m_MasterVolume;
        public float SFXVolume => m_SFXVolume;
        public float UIVolume => m_UIVolume;
        public float MusicVolume => m_MusicVolume;
        public float AmbienceVolume => m_AmbienceVolume;

        public bool EnableVolumeManager => m_EnableVolumeManager;
        public bool SaveVolumeSettings => m_SaveVolumeSettings;

        public string MasterVolumeParameter => m_MasterVolumeParameter;
        public string SFXVolumeParameter => m_SFXVolumeParameter;
        public string UIVolumeParameter => m_UIVolumeParameter;
        public string MusicVolumeParameter => m_MusicVolumeParameter;
        public string AmbienceVolumeParameter => m_AmbienceVolumeParameter;

        public AudioMixerGroup GetChannelGroup(AudioChannel channel)
        {
            return channel switch
            {
                AudioChannel.SFX => m_SFXGroup,
                AudioChannel.UI => m_UIGroup,
                AudioChannel.Music => m_MusicGroup,
                AudioChannel.Ambience => m_AmbienceGroup,
                _ => m_SFXGroup
            };
        }

        public string GetChannelVolumeParameter(AudioChannel channel)
        {
            return channel switch
            {
                AudioChannel.SFX => m_SFXVolumeParameter,
                AudioChannel.UI => m_UIVolumeParameter,
                AudioChannel.Music => m_MusicVolumeParameter,
                AudioChannel.Ambience => m_AmbienceVolumeParameter,
                _ => m_SFXVolumeParameter
            };
        }

        public float GetChannelDefaultVolume(AudioChannel channel)
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

        private void OnValidate()
        {
            m_InitialPoolSize = Mathf.Max(1, m_InitialPoolSize);
            m_MaxPoolSize = Mathf.Max(m_InitialPoolSize, m_MaxPoolSize);
            m_DefaultCrossfadeDuration = Mathf.Max(0f, m_DefaultCrossfadeDuration);
        }
    }
}