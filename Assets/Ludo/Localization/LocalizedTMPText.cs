using Ludo.Core.Events;
using TMPro;
using UnityEngine;

namespace Ludo.Localization
{
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedTMP_Text : MonoBehaviour
    {
        [SerializeField] private TMP_Text tmpText;
        [SerializeField] private string localizationKey;
        [SerializeField] private LocalizedFontSettings fontSettings;

        private readonly EventBinder _binder = new EventBinder();

        private bool _dynamicLocalization = true;

        private void Awake()
        {
            FindReferences();
        }

        public void Start()
        {
            Localize();
            _binder.Bind(LocalizationManager.LocalizationChanged, Localize);
        }

        public void OnDestroy()
        {
            _binder.Unbind();
        }

        public string GetText()
        {
            return tmpText != null ? tmpText.text : string.Empty;
        }

        public void SetText(string text, bool isKey)
        {
            if (isKey)
            {
                SetKey(text);
            }
            else
            {
                SetStaticText(text);
            }
        }

        public void SetKey(string key)
        {
            localizationKey = key;
            _dynamicLocalization = true;
            Localize();
        }

        public void SetStaticText(string text)
        {
            _dynamicLocalization = false;
            tmpText.text = text;
            UpdateFont();
        }

        private void Localize()
        {
            UpdateFont();
            if (_dynamicLocalization)
            {
                tmpText.text = LocalizationManager.Localize(localizationKey);
            }
        }

        private void UpdateFont()
        {
            if (fontSettings != null)
            {
                TMP_FontAsset fontAssetForCode = fontSettings.GetFontAssetForCode(LocalizationManager.Language);
                if (fontAssetForCode != null)
                {
                    tmpText.font = fontAssetForCode;
                }
            }
        }

        private void OnValidate()
        {
            FindReferences();
        }

        private void FindReferences()
        {
            if (tmpText == null)
            {
                tmpText = GetComponent<TMP_Text>();
            }
        }
    }
}