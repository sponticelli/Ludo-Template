using System;

namespace Ludo.Localization
{
    public interface ILocalizationService
    {
        public event Action<string> OnLanguageChanged;
        
        string Current { get; }
        string Default { get; }

        void SetLanguage(string languageCode); // e.g., "it" or "it-IT"
        bool TryGet(string key, out string value); // raw
        string Get(string key); // with fallback + [Key]
        string Get(string key, params object[] args); // string.Format
    }
}