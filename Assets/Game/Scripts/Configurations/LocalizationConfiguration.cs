using Ludo.Localization;
using UnityEngine;

namespace Game.Configurations
{
    [CreateAssetMenu(fileName = "LocalizationConfiguration", menuName = "Game/Configs/Localization")]
    public class LocalizationConfiguration : ScriptableObject
    {
        [SerializeField] private string defaultLanguage = "en";
        [SerializeField] private LocalizedTable[] tables;
        
        public LocalizedTable[] Tables => tables;
        public string DefaultLanguage => defaultLanguage;   
    }
}