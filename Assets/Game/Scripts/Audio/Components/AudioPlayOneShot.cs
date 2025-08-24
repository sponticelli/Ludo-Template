using Ludo.Audio;
using Ludo.Core;
using UnityEngine;

namespace Game.Audio.Components
{
    public class AudioPlayOneShot : MonoBehaviour
    {
        private IMultiChannelAudioService _audioService;
        
        [Header("One Shot")]
        [SerializeField] private AudioChannel _channel = AudioChannel.UI;
        [SerializeField] private AudioClip _oneShotClip;
        [SerializeField] private float _volume = 1f;
        [SerializeField] private bool playOnStart = false;

        [Header("Pitch Randomization")]
        [SerializeField] private float _minPitch = 0.8f;
        [SerializeField] private float _maxPitch = 1.1f;
        
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
            _audioService.PlayOneShot(_oneShotClip, _channel, _volume, _minPitch, _maxPitch);
        }
    }
}