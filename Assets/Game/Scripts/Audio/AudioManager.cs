using Game.Configurations;
using Ludo.Audio;
using Ludo.Core;
using UnityEngine;
using AudioConfiguration = Game.Configurations.AudioConfiguration;

namespace Game.Audio
{
    /// <summary>
    /// High-level audio manager that provides game-specific audio methods
    /// and abstracts away the channel system from developers
    /// </summary>
    public class AudioManager : MonoBehaviour, IAudioManager
    {
        
        private IAudioService _audioService;
        private IChannel _uiChannel;
        private IChannel _sfxChannel;
        private IChannel _musicChannel;
        private IChannel _ambienceChannel;
        private AudioHandle _currentMusicHandle;
        private AudioConfiguration _audioConfiguration;
        
        public bool IsInitialized { get; private set; }
        
        
        public void Initialize(AudioConfiguration audioConfiguration)
        {
            // Get the audio service from ServiceLocator
            if (!ServiceLocator.TryGet<IAudioService>(out _audioService))
            {
                Debug.LogError("[AudioManager] AudioService not found in ServiceLocator");
                return;
            }
            
            // Initialize channels from configuration
            if (audioConfiguration == null)
            {
                Debug.LogError("[AudioManager] AudioConfiguration is null");
                return;
            }
            
            _audioConfiguration = audioConfiguration;
            
            InitializeChannels();
            IsInitialized = true;
        }
        
        private void InitializeChannels()
        {
            var channels = _audioConfiguration.Channels;
            if (channels == null || channels.Length == 0)
            {
                Debug.LogError("[AudioManager] No channels found in AudioConfiguration");
                return;
            }
            
            // Find channels by name (case-insensitive)
            foreach (var channel in channels)
            {
                if (channel == null) continue;
                
                var channelName = channel.Name?.ToLower();
                switch (channelName)
                {
                    case "ui":
                        _uiChannel = channel;
                        break;
                    case "sfx":
                    case "soundeffects":
                        _sfxChannel = channel;
                        break;
                    case "music":
                        _musicChannel = channel;
                        break;
                    case "ambience":
                    case "ambient":
                        _ambienceChannel = channel;
                        break;
                }
            }
            
            // Log warnings for missing channels
            if (_uiChannel == null) Debug.LogWarning("[AudioManager] UI channel not found");
            if (_sfxChannel == null) Debug.LogWarning("[AudioManager] SFX channel not found");
            if (_musicChannel == null) Debug.LogWarning("[AudioManager] Music channel not found");
            if (_ambienceChannel == null) Debug.LogWarning("[AudioManager] Ambience channel not found");
        }
        
        // UI Audio Methods
        public void PlayUI(AudioClip audioClip, float volume = 1f, float pitch = 1f)
        {
            if (!IsInitialized || _uiChannel == null) return;
            _audioService.PlayOneShot(audioClip, _uiChannel, volume, pitch);
        }
        
        // SFX Audio Methods
        public void PlaySFX(AudioClip audioClip, float volume = 1f, float pitch = 1f)
        {
            if (!IsInitialized || _sfxChannel == null) return;
            _audioService.PlayOneShot(audioClip, _sfxChannel, volume, pitch);
        }
        
        public void PlaySFXAtPosition(AudioClip audioClip, Vector3 position, float volume = 1f, float pitch = 1f)
        {
            if (!IsInitialized || _sfxChannel == null) return;
            _audioService.PlayOneShotAtPosition(audioClip, _sfxChannel, position, volume, pitch);
        }
        
        public AudioHandle PlaySFXLooping(AudioClip audioClip, float volume = 1f, float pitch = 1f)
        {
            if (!IsInitialized || _sfxChannel == null) return null;
            return _audioService.PlayLooping(audioClip, _sfxChannel, volume, pitch);
        }
        
        public AudioHandle PlaySFXLoopingAtPosition(AudioClip audioClip, Vector3 position, float volume = 1f, float pitch = 1f)
        {
            if (!IsInitialized || _sfxChannel == null) return null;
            return _audioService.PlayLoopingAtPosition(audioClip, _sfxChannel, position, volume, pitch);
        }
        
        // Music Audio Methods
        public AudioHandle PlayMusic(AudioClip audioClip, bool loop = true, float volume = 1f, float pitch = 1f)
        {
            if (!IsInitialized || _musicChannel == null) return null;
            
            // Stop current music if playing
            StopMusic();
            
            if (loop)
            {
                _currentMusicHandle = _audioService.PlayLooping(audioClip, _musicChannel, volume, pitch);
            }
            else
            {
                _audioService.PlayOneShot(audioClip, _musicChannel, volume, pitch);
                _currentMusicHandle = null;
            }
            
            return _currentMusicHandle;
        }
        
        public AudioHandle PlayMusicWithFadeIn(AudioClip audioClip, float fadeDuration, bool loop = true, float targetVolume = 1f, float pitch = 1f)
        {
            if (!IsInitialized || _musicChannel == null) return null;
            
            // Stop current music if playing
            StopMusic();
            
            if (loop)
            {
                _currentMusicHandle = _audioService.PlayLoopingWithFadeIn(audioClip, _musicChannel, fadeDuration, targetVolume, pitch);
            }
            else
            {
                // For non-looping music with fade-in, we still use the looping method but will need to stop it manually
                _currentMusicHandle = _audioService.PlayLoopingWithFadeIn(audioClip, _musicChannel, fadeDuration, targetVolume, pitch);
                // TODO: Could add a coroutine to stop after clip length for non-looping
            }
            
            return _currentMusicHandle;
        }
        
        public void StopMusicWithFadeOut(float fadeDuration)
        {
            if (_currentMusicHandle != null)
            {
                _audioService.FadeOutAndStop(_currentMusicHandle, fadeDuration);
                _currentMusicHandle = null;
            }
        }
        
        public void StopMusic()
        {
            if (_currentMusicHandle != null)
            {
                _audioService.Stop(_currentMusicHandle);
                _currentMusicHandle = null;
            }
        }
        
        // Ambience Audio Methods
        public AudioHandle PlayAmbience(AudioClip audioClip, bool loop = true, float volume = 1f, float pitch = 1f)
        {
            if (!IsInitialized || _ambienceChannel == null) return null;
            
            if (loop)
            {
                return _audioService.PlayLooping(audioClip, _ambienceChannel, volume, pitch);
            }
            else
            {
                _audioService.PlayOneShot(audioClip, _ambienceChannel, volume, pitch);
                return null;
            }
        }
        
        public AudioHandle PlayAmbienceAtPosition(AudioClip audioClip, Vector3 position, bool loop = true, float volume = 1f, float pitch = 1f)
        {
            if (!IsInitialized || _ambienceChannel == null) return null;
            
            if (loop)
            {
                return _audioService.PlayLoopingAtPosition(audioClip, _ambienceChannel, position, volume, pitch);
            }
            else
            {
                _audioService.PlayOneShotAtPosition(audioClip, _ambienceChannel, position, volume, pitch);
                return null;
            }
        }
        
        public AudioHandle PlayAmbienceWithFadeIn(AudioClip audioClip, float fadeDuration, bool loop = true, float targetVolume = 1f, float pitch = 1f)
        {
            if (!IsInitialized || _ambienceChannel == null) return null;
            return _audioService.PlayLoopingWithFadeIn(audioClip, _ambienceChannel, fadeDuration, targetVolume, pitch);
        }
        
        public AudioHandle PlayAmbienceWithFadeInAtPosition(AudioClip audioClip, Vector3 position, float fadeDuration, bool loop = true, float targetVolume = 1f, float pitch = 1f)
        {
            if (!IsInitialized || _ambienceChannel == null) return null;
            return _audioService.PlayLoopingWithFadeInAtPosition(audioClip, _ambienceChannel, position, fadeDuration, targetVolume, pitch);
        }
        
        // General Control Methods
        public void Stop(AudioHandle handle)
        {
            if (!IsInitialized) return;
            _audioService.Stop(handle);
        }
        
        public void FadeOutAndStop(AudioHandle handle, float fadeDuration)
        {
            if (!IsInitialized) return;
            _audioService.FadeOutAndStop(handle, fadeDuration);
        }
        
        public void PauseAll()
        {
            if (!IsInitialized) return;
            _audioService.PauseAll();
        }
        
        public void UnpauseAll()
        {
            if (!IsInitialized) return;
            _audioService.UnpauseAll();
        }
    }
}
