using Ludo.Core;
using UnityEngine;

namespace Ludo.Localization
{
    public class LocalizedTMProText : MonoBehaviour
    {
        [Header("Localization")]
        [SerializeField] private string key;
        [SerializeField] private FontSettings fontSettings;

        [Header("References")]
        [SerializeField] private TMPro.TMP_Text textField;
        
        private ILocalizationService _localizationService;


        
        public void SetKey(string newKey)
        {
            key = newKey;
            Refresh();
        }

        private void Awake()
        {
            _localizationService = ServiceLocator.Get<ILocalizationService>();
            if (textField == null) textField = GetComponent<TMPro.TMP_Text>();
        }
        

        private void OnEnable()
        {
            Bind();
            Refresh();
        }

        private void OnDisable()
        {
            Unbind();
        }

        private void Bind()
        {
            _localizationService.OnLanguageChanged += OnLanguageChanged;
        }
        
        private void Unbind()
        {
            _localizationService.OnLanguageChanged -= OnLanguageChanged;
        }

        private void OnLanguageChanged(string e) => Refresh();
        public void Refresh()
        {
            if (!textField) return;
            
            textField.text = _localizationService.Get(key);
            textField.font = fontSettings.GetFont(_localizationService.Current);
        }
    }
}