using Ludo.Core;
using Ludo.Core.Events;
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
        private IEventHub _eventHub;


        private void Awake()
        {
            Bind();
            if (textField == null) textField = GetComponent<TMPro.TMP_Text>();
        }

        private void OnEnable()
        {
            _eventHub.Subscribe<LanguageChangedEvent>(OnLanguageChanged);
            Refresh();
        }

        private void OnDisable()
        {
            _eventHub.Unsubscribe<LanguageChangedEvent>(OnLanguageChanged);
        }
        

        private void Bind()
        {
            _eventHub = ServiceLocator.Get<IEventHub>();
            _localizationService = ServiceLocator.Get<ILocalizationService>();
        }

        private void OnLanguageChanged(LanguageChangedEvent e) => Refresh();
        public void Refresh()
        {
            if (!textField) return;
            
            textField.text = _localizationService.Get(key);
            textField.font = fontSettings.GetFont(_localizationService.Current);
        }
    }
}