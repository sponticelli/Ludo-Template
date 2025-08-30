using UnityEngine;
using UnityEngine.Audio;

namespace Ludo.Audio
{
    [CreateAssetMenu(fileName = "Channel", menuName = "Ludo/Audio/Channel")]
    public class Channel : ScriptableObject, IChannel
    {
        [SerializeField] private string channelName;
        [SerializeField] private AudioMixerGroup mixerGroup;
        [SerializeField] private string volumeParameter;

        public string Name => channelName;
        public AudioMixerGroup MixerGroup => mixerGroup;
        public string VolumeParameter => volumeParameter;
    }
    
    
}