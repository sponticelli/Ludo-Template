using UnityEngine;

namespace Ludo.Audio
{
    public interface IAudioService
    {
        void PlayOneShot(AudioClip clip, float vol = 1);
        IAudioHandle PlayLoop(AudioClip clip, float vol = 1);
    }

    public interface IAudioHandle
    {
        public bool IsPlaying { get; }
        public void Stop();
    }

    public interface IVolumeControlledAudioHandle : IAudioHandle
    {
        public void SetVolume(float volume);
        public float GetVolume();
    }
}