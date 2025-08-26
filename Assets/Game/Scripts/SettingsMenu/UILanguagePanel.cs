using Ludo.Core;
using Ludo.Settings.Runtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace Game.SettingsMenu
{
    /// <summary>
    /// UI panel for language-related settings
    /// </summary>
    public class UILanguagePanel : MonoBehaviour
    {
        [Header("Language Selection")]
        [SerializeField] private TMP_Dropdown languageDropdown;
        [SerializeField] private Button applyLanguageButton;
        
        [Header("Language Options")]
        [SerializeField] private List<LanguageOption> availableLanguages = new List<LanguageOption>
        {
            new LanguageOption { code = "en", displayName = "English" },
            new LanguageOption { code = "es", displayName = "Español" },
            new LanguageOption { code = "fr", displayName = "Français" },
            new LanguageOption { code = "de", displayName = "Deutsch" },
            new LanguageOption { code = "it", displayName = "Italiano" },
            new LanguageOption { code = "pt", displayName = "Português" },
            new LanguageOption { code = "ru", displayName = "Русский" },
            new LanguageOption { code = "ja", displayName = "日本語" },
            new LanguageOption { code = "ko", displayName = "한국어" },
            new LanguageOption { code = "zh", displayName = "中文" }
        };
        
        private ISettingsService _settingsService;
        private bool _isInitialized = false;
        private string _pendingLanguage;
        
        private void Awake()
        {
            _settingsService = ServiceLocator.Get<ISettingsService>();
            
            if (_settingsService == null)
            {
                Debug.LogError("ISettingsService not found in ServiceLocator!");
                return;
            }
        }
        
        private void Start()
        {
            InitializeUI();
            SetupEventListeners();
            _isInitialized = true;
        }
        
        private void OnEnable()
        {
            if (_isInitialized)
            {
                RefreshUI();
            }
        }
        
        private void InitializeUI()
        {
            // Initialize Language Dropdown
            if (languageDropdown != null)
            {
                languageDropdown.ClearOptions();
                
                foreach (var language in availableLanguages)
                {
                    languageDropdown.options.Add(new TMP_Dropdown.OptionData(language.displayName));
                }
                
                languageDropdown.RefreshShownValue();
            }
            
            RefreshUI();
        }
        
        private void SetupEventListeners()
        {
            // Language Dropdown
            if (languageDropdown != null)
                languageDropdown.onValueChanged.AddListener(OnLanguageSelectionChanged);
                
            // Apply Language Button
            if (applyLanguageButton != null)
                applyLanguageButton.onClick.AddListener(OnApplyLanguageClicked);
        }
        
        private void RefreshUI()
        {
            if (_settingsService == null) return;
            
            // Update Language Dropdown
            if (languageDropdown != null)
            {
                string currentLanguage = _settingsService.Language;
                
                for (int i = 0; i < availableLanguages.Count; i++)
                {
                    if (availableLanguages[i].code == currentLanguage)
                    {
                        languageDropdown.value = i;
                        break;
                    }
                }
            }
            
            // Update Apply button state
            RefreshApplyButton();
        }
        
        private void OnLanguageSelectionChanged(int index)
        {
            if (index >= 0 && index < availableLanguages.Count)
            {
                _pendingLanguage = availableLanguages[index].code;
                RefreshApplyButton();
            }
        }
        
        private void OnApplyLanguageClicked()
        {
            if (!string.IsNullOrEmpty(_pendingLanguage))
            {
                _settingsService.Language = _pendingLanguage;
                _pendingLanguage = null;
                
                // Show confirmation message or restart prompt
                Debug.Log($"Language changed to: {_settingsService.Language}");
                
                RefreshApplyButton();
            }
        }
        
        private void RefreshApplyButton()
        {
            if (applyLanguageButton != null)
            {
                bool hasChanges = !string.IsNullOrEmpty(_pendingLanguage) && 
                                 _pendingLanguage != _settingsService.Language;
                applyLanguageButton.interactable = hasChanges;
            }
        }
        
        /// <summary>
        /// Gets the display name for a language code
        /// </summary>
        public string GetLanguageDisplayName(string languageCode)
        {
            foreach (var language in availableLanguages)
            {
                if (language.code == languageCode)
                    return language.displayName;
            }
            return languageCode; // Fallback to code if not found
        }
        
        /// <summary>
        /// Adds a new language option (useful for runtime configuration)
        /// </summary>
        public void AddLanguageOption(string code, string displayName)
        {
            var newLanguage = new LanguageOption { code = code, displayName = displayName };
            
            if (!availableLanguages.Exists(l => l.code == code))
            {
                availableLanguages.Add(newLanguage);
                
                if (_isInitialized)
                {
                    InitializeUI(); // Refresh the dropdown
                }
            }
        }
    }
    
    /// <summary>
    /// Data structure for language options
    /// </summary>
    [System.Serializable]
    public class LanguageOption
    {
        public string code;
        public string displayName;
    }
}
