using System;
using TMPro;
using UnityEngine;

namespace Ludo.Localization
{
    [Serializable]
    public class CustomFontSettings
    {
        public string languageCode;
        public TMP_FontAsset font;
    }
    
    [CreateAssetMenu(fileName = "FontSettings", menuName = "Ludo/Localization/FontSettings")]
    public class FontSettings : ScriptableObject
    {
        [SerializeField] private TMP_FontAsset defaultFont; 
        [SerializeField] private CustomFontSettings[] customFonts;

        public TMP_FontAsset GetFont(string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode)) return defaultFont;
            foreach (var f in customFonts)
            {
                if (f.languageCode == languageCode) return f.font;
            }
            return defaultFont;
        }
    }
}