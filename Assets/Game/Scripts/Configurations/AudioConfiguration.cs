using Ludo.Audio;
using UnityEngine;

namespace Game.Configurations
{
    [CreateAssetMenu(fileName = "AudioConfiguration", menuName = "Game/Configs/Audio")]
    public class AudioConfiguration : ScriptableObject
    {
        [SerializeField] private Channel[] channels;
        
        public Channel[] Channels => channels;
    }
}