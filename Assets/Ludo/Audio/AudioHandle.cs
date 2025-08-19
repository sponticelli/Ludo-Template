using UnityEngine;

namespace Ludo.Audio
{
    public class AudioHandle
    {
        private AudioSource m_AudioSource;
        private bool m_IsValid;
        private AudioManager m_AudioManager;

        public bool IsPlaying => m_IsValid && m_AudioSource != null && m_AudioSource.isPlaying;
        public bool IsValid => m_IsValid && m_AudioSource != null;
        public float Volume 
        { 
            get => m_IsValid && m_AudioSource != null ? m_AudioSource.volume : 0f;
            set 
            { 
                if (m_IsValid && m_AudioSource != null) 
                    m_AudioSource.volume = Mathf.Clamp01(value); 
            }
        }
        public float Pitch 
        { 
            get => m_IsValid && m_AudioSource != null ? m_AudioSource.pitch : 1f;
            set 
            { 
                if (m_IsValid && m_AudioSource != null) 
                    m_AudioSource.pitch = value; 
            }
        }

        internal AudioHandle(AudioSource audioSource, AudioManager audioManager)
        {
            m_AudioManager = audioManager;
            m_AudioSource = audioSource;
            m_IsValid = true;
        }

        public void Stop()
        {
            if (m_IsValid && m_AudioSource != null)
            {
                m_AudioSource.Stop();
            }
        }

        public void Pause()
        {
            if (m_IsValid && m_AudioSource != null && m_AudioSource.isPlaying)
            {
                m_AudioSource.Pause();
            }
        }

        public void Resume()
        {
            if (m_IsValid && m_AudioSource != null && !m_AudioSource.isPlaying)
            {
                m_AudioSource.UnPause();
            }
        }

        public void FadeOut(float duration)
        {
            if (m_IsValid && m_AudioSource != null)
            {
                m_AudioManager.StartCoroutine(FadeVolumeCoroutine(0f, duration, true));
            }
        }

        public void FadeIn(float duration, float targetVolume = 1f)
        {
            if (m_IsValid && m_AudioSource != null)
            {
                m_AudioSource.volume = 0f;
                m_AudioManager.StartCoroutine(FadeVolumeCoroutine(targetVolume, duration, false));
            }
        }

        private System.Collections.IEnumerator FadeVolumeCoroutine(float targetVolume, float duration, bool stopAfterFade)
        {
            if (m_AudioSource == null) yield break;

            float startVolume = m_AudioSource.volume;
            float elapsed = 0f;

            while (elapsed < duration && m_AudioSource != null)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                m_AudioSource.volume = Mathf.Lerp(startVolume, targetVolume, t);
                yield return null;
            }

            if (m_AudioSource != null)
            {
                m_AudioSource.volume = targetVolume;
                if (stopAfterFade)
                {
                    m_AudioSource.Stop();
                }
            }
        }

        internal void Invalidate()
        {
            m_IsValid = false;
        }
    }
}