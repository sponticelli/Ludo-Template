using System;
using TMPro;
using UnityEngine;

namespace Ludo.Localization
{
    [CreateAssetMenu(fileName = "LocalizedFontSettings", menuName = "Ludo/Localization/Localized Font Settings", order = 11)]
    public class LocalizedFontSettings : ScriptableObject
    {
        [Serializable]
        public class FontMapping
        {
            public string code;
            public TMP_FontAsset fontAsset;
        }
        
        
        
       [SerializeField] private TMP_FontAsset defaultFont;
       [SerializeField] private FontMapping[] fontMappings;
       
        public TMP_FontAsset GetFontAssetForCode(string code)
        {
            foreach (var map in fontMappings)
            {
                if (map.code == code) return map.fontAsset;
            }
            return defaultFont;
        }
    }

}