using UnityEngine;

namespace Ludo.Core.Services
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
}