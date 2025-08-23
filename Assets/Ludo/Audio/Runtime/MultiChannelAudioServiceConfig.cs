using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Ludo.Audio
{
    [Serializable]
    public class ChannelConfig
    {
        [Header("Channel Settings")]
        [Range(0f, 1f)]
        public float defaultVolume = 1f;
        
        [Header("AudioSource Pooling")]
        [Tooltip("Initial number of AudioSources to create in the pool")]
        public int initialPoolSize = 5;
        
        [Tooltip("Maximum number of AudioSources that can be created (0 = unlimited)")]
        public int maxPoolSize = 20;
        
        [Header("Audio Mixing")]
        public AudioMixerGroup mixerGroup;
    }

    [Serializable]
    public class MultiChannelAudioServiceConfig
    {
        [Header("General Settings")]
        public bool hideInHierarchy = true;
        public bool preWarmPools = true;
        
        [Header("Channel Configurations")]
        public ChannelConfig sfxConfig = new ChannelConfig { initialPoolSize = 8, maxPoolSize = 25 };
        public ChannelConfig uiConfig = new ChannelConfig { initialPoolSize = 3, maxPoolSize = 10 };
        public ChannelConfig musicConfig = new ChannelConfig { initialPoolSize = 2, maxPoolSize = 5 };
        public ChannelConfig ambienceConfig = new ChannelConfig { initialPoolSize = 3, maxPoolSize = 8 };
        
        /// <summary>
        /// Gets the configuration for a specific audio channel.
        /// </summary>
        public ChannelConfig GetChannelConfig(AudioChannel channel)
        {
            return channel switch
            {
                AudioChannel.SFX => sfxConfig,
                AudioChannel.UI => uiConfig,
                AudioChannel.Music => musicConfig,
                AudioChannel.Ambience => ambienceConfig,
                _ => sfxConfig // Default fallback
            };
        }
    }
}
