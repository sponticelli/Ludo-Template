namespace Ludo.Localization
{
    public interface ILocalizationService
    {
        string Current { get; }
        string Default { get; }

        void SetLanguage(string languageCode); // e.g., "it" or "it-IT"
        bool TryGet(string key, out string value); // raw
        string Get(string key); // with fallback + [Key]
        string Get(string key, params object[] args); // string.Format
    }
}