using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ludo.Localization
{
    public sealed class LocalizationService : ILocalizationService
    {
        
        public event Action<string> OnLanguageChanged;
        
        private readonly Dictionary<string, LocalizedTable> _byLang = new();
        private readonly string _default;
        private string _current;
        
        public string Current => _current;
        public string Default => _default;

        public LocalizationService(string defaultLanguage, IEnumerable<LocalizedTable> tables)
        {
            _default = string.IsNullOrEmpty(defaultLanguage) ? "en" : defaultLanguage;
            _current = _default;

            if (tables == null) return;
            foreach (var t in tables)
            {
                if (t != null && !string.IsNullOrEmpty(t.languageCode))
                {
                    _byLang[t.languageCode] = t;
                }
                
            }
        }

        public void SetLanguage(string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode)) return;
            _current = languageCode;
            OnLanguageChanged?.Invoke(_current);
        }

        public bool TryGet(string key, out string value)
        {
            // 1) exact current (e.g., "it-IT"), 2) current base ("it"), 3) default, 4) default base
            if (TryGetFromLang(_current, key, out value)) return true;
            if (TryGetFromLang(Base(_current), key, out value)) return true;
            if (TryGetFromLang(_default, key, out value)) return true;
            if (TryGetFromLang(Base(_default), key, out value)) return true;

#if UNITY_EDITOR
        Debug.LogWarning($"[LOCALIZATION] Missing key '{key}' (lang '{_current}' → default '{_default}')");
#endif
            value = $"[{key}]";
            return false;
        }

        public string Get(string key)
            => TryGet(key, out var v) ? v : $"[{key}]";

        public string Get(string key, params object[] args)
        {
            var raw = Get(key);
            if (args == null || args.Length == 0) return raw;
            try
            {
                return string.Format(raw, args);
            }
            catch
            {
                return raw;
            }
        }

        private bool TryGetFromLang(string lang, string key, out string value)
        {
            if (string.IsNullOrEmpty(lang))
            {
                value = null;
                return false;
            }
            
            _byLang.TryGetValue(lang, out var table);

            if (table == null)
            {
                value = "";
                return false;
            }
            return table.TryGet(key, out value);
        }

        private static string Base(string code)
        {
            // "it-IT" -> "it"; "en" -> "en"
            if (string.IsNullOrEmpty(code)) return code;
            int i = code.IndexOf('-');
            return i > 0 ? code.Substring(0, i) : code;
        }
    }
}