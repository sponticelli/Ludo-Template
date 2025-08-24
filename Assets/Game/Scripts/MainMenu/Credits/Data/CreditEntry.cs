using UnityEngine;

namespace Game.MainMenu.Credits.Data
{
    [System.Serializable]
    public class CreditEntry
    {
        [SerializeField] private string name;
        
        public string Name => name;
    }
}