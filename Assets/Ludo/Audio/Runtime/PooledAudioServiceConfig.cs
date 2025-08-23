using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Ludo.Audio
{
    [Serializable]
    public class PooledAudioServiceConfig
    {
        public bool hideInHierarchy = true;
        public AudioMixerGroup mixerGroup;

        [Header("AudioSource Pooling")]
        [Tooltip("Initial number of AudioSources to create in the pool")]
        public int initialPoolSize = 5;

        [Tooltip("Maximum number of AudioSources that can be created (0 = unlimited)")]
        public int maxPoolSize = 20;

        [Tooltip("Whether to pre-warm the pool on initialization")]
        public bool preWarmPool = true;
    }
}