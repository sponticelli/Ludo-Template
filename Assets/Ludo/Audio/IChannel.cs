using UnityEngine.Audio;

namespace Ludo.Audio
{
    public interface IChannel
    {
        public string Name { get; }
        public AudioMixerGroup MixerGroup { get; }
        public string VolumeParameter { get; }
    }
}