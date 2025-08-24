using UnityEngine;

namespace Game.MainMenu.Credits.Data
{
    [System.Serializable]
    public class CreditGroup
    {
        [SerializeField] private string titleKey;
        [SerializeField] private CreditEntry[] entries;
        
        public string TitleKey => titleKey;
        public CreditEntry[] Entries => entries;
    }
}