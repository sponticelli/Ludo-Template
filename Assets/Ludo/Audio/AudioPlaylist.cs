using System.Collections.Generic;
using UnityEngine;

namespace Ludo.Audio
{
    public enum PlaylistMode
    {
        Sequential,
        Random,
        Shuffle
    }

    public enum RepeatMode
    {
        None,
        Single,
        All
    }

    [System.Serializable]
    public class AudioPlaylist
    {
        [SerializeField] private List<AudioClip> m_Tracks = new List<AudioClip>();
        [SerializeField] private PlaylistMode m_PlaylistMode = PlaylistMode.Sequential;
        [SerializeField] private RepeatMode m_RepeatMode = RepeatMode.All;
        
        private int m_CurrentTrackIndex = 0;
        private List<int> m_ShuffledIndices = new List<int>();
        private int m_ShuffleIndex = 0;

        public List<AudioClip> Tracks => m_Tracks;
        public PlaylistMode PlaylistMode 
        { 
            get => m_PlaylistMode; 
            set 
            { 
                m_PlaylistMode = value;
                if (value == PlaylistMode.Shuffle)
                    GenerateShuffledOrder();
            }
        }
        public RepeatMode RepeatMode { get => m_RepeatMode; set => m_RepeatMode = value; }
        public int CurrentTrackIndex => m_CurrentTrackIndex;
        public AudioClip CurrentTrack => HasTracks ? m_Tracks[m_CurrentTrackIndex] : null;
        public bool HasTracks => m_Tracks != null && m_Tracks.Count > 0;
        public int TrackCount => m_Tracks?.Count ?? 0;

        public AudioPlaylist()
        {
        }

        public AudioPlaylist(List<AudioClip> tracks, PlaylistMode mode = PlaylistMode.Sequential, RepeatMode repeat = RepeatMode.All)
        {
            m_Tracks = tracks ?? new List<AudioClip>();
            m_PlaylistMode = mode;
            m_RepeatMode = repeat;
            
            if (mode == PlaylistMode.Shuffle)
                GenerateShuffledOrder();
        }

        public void AddTrack(AudioClip clip)
        {
            if (clip != null)
            {
                m_Tracks.Add(clip);
                if (m_PlaylistMode == PlaylistMode.Shuffle)
                    GenerateShuffledOrder();
            }
        }

        public void RemoveTrack(AudioClip clip)
        {
            if (m_Tracks.Remove(clip))
            {
                if (m_CurrentTrackIndex >= m_Tracks.Count)
                    m_CurrentTrackIndex = 0;
                
                if (m_PlaylistMode == PlaylistMode.Shuffle)
                    GenerateShuffledOrder();
            }
        }

        public void ClearTracks()
        {
            m_Tracks.Clear();
            m_CurrentTrackIndex = 0;
            m_ShuffledIndices.Clear();
            m_ShuffleIndex = 0;
        }

        public AudioClip GetNextTrack()
        {
            if (!HasTracks) return null;

            switch (m_PlaylistMode)
            {
                case PlaylistMode.Sequential:
                    return GetNextSequential();
                case PlaylistMode.Random:
                    return GetNextRandom();
                case PlaylistMode.Shuffle:
                    return GetNextShuffled();
                default:
                    return GetNextSequential();
            }
        }

        public AudioClip GetPreviousTrack()
        {
            if (!HasTracks) return null;

            switch (m_PlaylistMode)
            {
                case PlaylistMode.Sequential:
                    return GetPreviousSequential();
                case PlaylistMode.Random:
                    return GetNextRandom(); // For random, previous is just another random
                case PlaylistMode.Shuffle:
                    return GetPreviousShuffled();
                default:
                    return GetPreviousSequential();
            }
        }

        public void SetCurrentTrack(int index)
        {
            if (HasTracks && index >= 0 && index < m_Tracks.Count)
            {
                m_CurrentTrackIndex = index;
            }
        }

        private AudioClip GetNextSequential()
        {
            if (m_CurrentTrackIndex >= m_Tracks.Count - 1)
            {
                if (m_RepeatMode == RepeatMode.All)
                {
                    m_CurrentTrackIndex = 0;
                }
                else if (m_RepeatMode == RepeatMode.None)
                {
                    return null;
                }
            }
            else
            {
                m_CurrentTrackIndex++;
            }

            return m_Tracks[m_CurrentTrackIndex];
        }

        private AudioClip GetPreviousSequential()
        {
            if (m_CurrentTrackIndex <= 0)
            {
                if (m_RepeatMode == RepeatMode.All)
                {
                    m_CurrentTrackIndex = m_Tracks.Count - 1;
                }
                else if (m_RepeatMode == RepeatMode.None)
                {
                    return null;
                }
            }
            else
            {
                m_CurrentTrackIndex--;
            }

            return m_Tracks[m_CurrentTrackIndex];
        }

        private AudioClip GetNextRandom()
        {
            if (m_RepeatMode == RepeatMode.Single)
            {
                return m_Tracks[m_CurrentTrackIndex];
            }

            int newIndex;
            if (m_Tracks.Count == 1)
            {
                newIndex = 0;
            }
            else
            {
                do
                {
                    newIndex = Random.Range(0, m_Tracks.Count);
                } while (newIndex == m_CurrentTrackIndex && m_RepeatMode != RepeatMode.All);
            }

            m_CurrentTrackIndex = newIndex;
            return m_Tracks[m_CurrentTrackIndex];
        }

        private AudioClip GetNextShuffled()
        {
            if (m_ShuffledIndices.Count == 0)
            {
                if (m_RepeatMode == RepeatMode.None)
                    return null;
                GenerateShuffledOrder();
                m_ShuffleIndex = 0;
            }

            if (m_ShuffleIndex >= m_ShuffledIndices.Count)
            {
                if (m_RepeatMode == RepeatMode.All)
                {
                    GenerateShuffledOrder();
                    m_ShuffleIndex = 0;
                }
                else if (m_RepeatMode == RepeatMode.None)
                {
                    return null;
                }
            }

            if (m_ShuffleIndex < m_ShuffledIndices.Count)
            {
                m_CurrentTrackIndex = m_ShuffledIndices[m_ShuffleIndex];
                m_ShuffleIndex++;
                return m_Tracks[m_CurrentTrackIndex];
            }

            return null;
        }

        private AudioClip GetPreviousShuffled()
        {
            if (m_ShuffleIndex > 0)
            {
                m_ShuffleIndex--;
                m_CurrentTrackIndex = m_ShuffledIndices[m_ShuffleIndex];
                return m_Tracks[m_CurrentTrackIndex];
            }

            if (m_RepeatMode == RepeatMode.All)
            {
                m_ShuffleIndex = m_ShuffledIndices.Count - 1;
                if (m_ShuffleIndex >= 0)
                {
                    m_CurrentTrackIndex = m_ShuffledIndices[m_ShuffleIndex];
                    return m_Tracks[m_CurrentTrackIndex];
                }
            }

            return null;
        }

        private void GenerateShuffledOrder()
        {
            m_ShuffledIndices.Clear();
            for (int i = 0; i < m_Tracks.Count; i++)
            {
                m_ShuffledIndices.Add(i);
            }

            // Fisher-Yates shuffle
            for (int i = m_ShuffledIndices.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                int temp = m_ShuffledIndices[i];
                m_ShuffledIndices[i] = m_ShuffledIndices[randomIndex];
                m_ShuffledIndices[randomIndex] = temp;
            }

            m_ShuffleIndex = 0;
        }
    }
}