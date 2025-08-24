using UnityEngine;

namespace Ludo.Audio
{
    public interface IAudioService
    {
        void PlayOneShot(AudioClip clip, float vol = 1);
        void PlayOneShot(AudioClip clip, float vol = 1, float minPitch = 1.0f, float maxPitch = 1.0f);
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