using Ludo.Core;
using Ludo.Settings.Runtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.SettingsMenu
{
    /// <summary>
    /// UI panel for display-related settings (Resolution, Quality, VSync, etc.)
    /// </summary>
    public class UIDisplayPanel : MonoBehaviour
    {
        [Header("Resolution Settings")]
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private TMP_Dropdown windowModeDropdown;
        
        [Header("Quality Settings")]
        [SerializeField] private TMP_Dropdown qualityDropdown;
        [SerializeField] private Toggle vSyncToggle;
        [SerializeField] private Toggle batterySaverToggle;
        
        [Header("UI Scale")]
        [SerializeField] private Slider uiScaleSlider;
        [SerializeField] private TMP_Text uiScaleValueText;
        
        [Header("Apply/Reset Buttons")]
        [SerializeField] private Button applyButton;
        [SerializeField] private Button resetButton;
        
        private ISettingsService _settingsService;
        private bool _isInitialized = false;
        
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
            // Initialize Resolution Dropdown
            if (resolutionDropdown != null)
            {
                resolutionDropdown.ClearOptions();
                var resolutions = _settingsService.AvailableResolutions;
                foreach (var resolution in resolutions)
                {
                    resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(_settingsService.Res2Str(resolution)));
                }
                resolutionDropdown.RefreshShownValue();
            }
            
            // Initialize Window Mode Dropdown
            if (windowModeDropdown != null)
            {
                windowModeDropdown.ClearOptions();
                windowModeDropdown.options.Add(new TMP_Dropdown.OptionData("Fullscreen"));
                windowModeDropdown.options.Add(new TMP_Dropdown.OptionData("Borderless"));
                windowModeDropdown.options.Add(new TMP_Dropdown.OptionData("Maximized"));
                windowModeDropdown.options.Add(new TMP_Dropdown.OptionData("Windowed"));
                windowModeDropdown.RefreshShownValue();
            }
            
            // Initialize Quality Dropdown
            if (qualityDropdown != null)
            {
                qualityDropdown.ClearOptions();
                qualityDropdown.options.Add(new TMP_Dropdown.OptionData("Low"));
                qualityDropdown.options.Add(new TMP_Dropdown.OptionData("Medium"));
                qualityDropdown.options.Add(new TMP_Dropdown.OptionData("High"));
                qualityDropdown.options.Add(new TMP_Dropdown.OptionData("Ultra"));
                qualityDropdown.RefreshShownValue();
            }
            
            // Initialize UI Scale Slider
            if (uiScaleSlider != null)
            {
                uiScaleSlider.minValue = 50f;
                uiScaleSlider.maxValue = 120f;
                uiScaleSlider.wholeNumbers = true;
            }
            
            RefreshUI();
        }
        
        private void SetupEventListeners()
        {
            // Resolution
            if (resolutionDropdown != null)
                resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
                
            // Window Mode
            if (windowModeDropdown != null)
                windowModeDropdown.onValueChanged.AddListener(OnWindowModeChanged);
                
            // Quality
            if (qualityDropdown != null)
                qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
                
            // VSync
            if (vSyncToggle != null)
                vSyncToggle.onValueChanged.AddListener(OnVSyncChanged);
                
            // Battery Saver
            if (batterySaverToggle != null)
                batterySaverToggle.onValueChanged.AddListener(OnBatterySaverChanged);
                
            // UI Scale
            if (uiScaleSlider != null)
                uiScaleSlider.onValueChanged.AddListener(OnUIScaleChanged);
                
            // Apply/Reset buttons
            if (applyButton != null)
                applyButton.onClick.AddListener(OnApplyClicked);
                
            if (resetButton != null)
                resetButton.onClick.AddListener(OnResetClicked);
        }
        
        private void RefreshUI()
        {
            if (_settingsService == null) return;
            
            // Update Resolution
            if (resolutionDropdown != null)
            {
                var currentResolution = _settingsService.Resolution;
                for (int i = 0; i < resolutionDropdown.options.Count; i++)
                {
                    if (resolutionDropdown.options[i].text == currentResolution)
                    {
                        resolutionDropdown.value = i;
                        break;
                    }
                }
            }
            
            // Update Window Mode
            if (windowModeDropdown != null)
                windowModeDropdown.value = _settingsService.WindowMode;
                
            // Update Quality
            if (qualityDropdown != null)
                qualityDropdown.value = (int)_settingsService.Quality;
                
            // Update VSync
            if (vSyncToggle != null)
                vSyncToggle.isOn = _settingsService.VSync == 1;
                
            // Update Battery Saver
            if (batterySaverToggle != null)
                batterySaverToggle.isOn = _settingsService.BatterySaver;
                
            // Update UI Scale
            if (uiScaleSlider != null)
            {
                uiScaleSlider.value = _settingsService.GameUIScale;
                UpdateUIScaleText();
            }
            
            // Update Apply button state
            if (applyButton != null)
                applyButton.interactable = _settingsService.DirtyVisuals;
        }
        
        private void OnResolutionChanged(int index)
        {
            if (resolutionDropdown != null && index < resolutionDropdown.options.Count)
            {
                _settingsService.Resolution = resolutionDropdown.options[index].text;
                RefreshApplyButton();
            }
        }
        
        private void OnWindowModeChanged(int index)
        {
            _settingsService.WindowMode = index;
            RefreshApplyButton();
        }
        
        private void OnQualityChanged(int index)
        {
            _settingsService.Quality = (GraphicsQuality)index;
        }
        
        private void OnVSyncChanged(bool value)
        {
            _settingsService.VSync = value ? 1 : 0;
        }
        
        private void OnBatterySaverChanged(bool value)
        {
            _settingsService.BatterySaver = value;
        }
        
        private void OnUIScaleChanged(float value)
        {
            _settingsService.GameUIScale = (int)value;
            UpdateUIScaleText();
        }
        
        private void UpdateUIScaleText()
        {
            if (uiScaleValueText != null)
                uiScaleValueText.text = _settingsService.GameUIScaleText;
        }
        
        private void OnApplyClicked()
        {
            _settingsService.ApplyChanges();
            RefreshApplyButton();
        }
        
        private void OnResetClicked()
        {
            _settingsService.ResetVisuals();
            RefreshUI();
        }
        
        private void RefreshApplyButton()
        {
            if (applyButton != null)
                applyButton.interactable = _settingsService.DirtyVisuals;
        }
    }
}
