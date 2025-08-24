using UnityEngine;

namespace Game.MainMenu.Credits.Data
{
    [CreateAssetMenu(fileName = "CreditsData", menuName = "Game/CreditsData")]
    public class CreditsData : ScriptableObject
    {
        [SerializeField] private CreditGroup[] groups;
        
        public CreditGroup[] Groups => groups;
    }
}