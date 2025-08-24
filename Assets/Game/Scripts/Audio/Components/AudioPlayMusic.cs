using Ludo.Audio;
using Ludo.Core;
using UnityEngine;

namespace Game.Audio.Components
{
    public class AudioPlayMusic : MonoBehaviour
    {
        
        private IMultiChannelAudioService _audioService;
        
        [Header("Music")]
        [SerializeField] private AudioClip _musicClip;
        [SerializeField] private float _fadeDuration = 1f;
        [SerializeField] private float _volume = 1f;
        [SerializeField] private bool playOnStart = true;
        
        
        private void Awake()
        {
            _audioService = ServiceLocator.Get<IMultiChannelAudioService>();
        }
        
        private void Start()
        {
            if (playOnStart) Play();
        }

        public void Play()
        {
            _audioService.PlayLoopWithFadeIn(_musicClip, AudioChannel.Music, _volume, _fadeDuration);
        }
    }
}