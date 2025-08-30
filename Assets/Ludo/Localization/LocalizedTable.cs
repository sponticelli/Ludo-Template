using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ludo.Localization
{
    [CreateAssetMenu(fileName = "LOC_en", menuName = "Ludo/Localization/Table")]
    public sealed class LocalizedTable : ScriptableObject
    {
        public string languageCode = "en"; // IETF like "en", "it-IT"

        [System.Serializable]
        public struct Entry
        {
            public string key;
            [TextArea] public string value;
        }

        public Entry[] entries;

        private Dictionary<string, string> _map;

        private void OnEnable()
        {
            _map = new Dictionary<string, string>(entries?.Length ?? 0);
            if (entries == null) return;
            for (int i = 0; i < entries.Length; i++)
            {
                var e = entries[i];
                if (!string.IsNullOrEmpty(e.key)) _map[e.key] = e.value ?? "";
            }
        }

        public bool TryGet(string key, out string value)
        {
            if (_map == null) OnEnable();
            return _map.TryGetValue(key, out value);
        }
    }
}