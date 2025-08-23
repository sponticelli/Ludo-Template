using UnityEngine;

namespace Ludo.Audio
{
    /// <summary>
    /// Simple MonoBehaviour component that handles fade updates for MultiChannelAudioService.
    /// This component is automatically created and managed by the MultiChannelAudioService.
    /// </summary>
    internal sealed class AudioFadeUpdater : MonoBehaviour
    {
        private MultiChannelAudioService _audioService;

        public void Initialize(MultiChannelAudioService audioService)
        {
            _audioService = audioService;
        }

        private void Update()
        {
            _audioService?.UpdateFades();
        }

        private void OnDestroy()
        {
            _audioService = null;
        }
    }
}
