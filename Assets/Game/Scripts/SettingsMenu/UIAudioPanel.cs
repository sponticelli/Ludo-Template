using Ludo.Core;
using Ludo.Settings.Runtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.SettingsMenu
{
    /// <summary>
    /// UI panel for audio-related settings (Music Volume, SFX Volume, Vibrations, Mute)
    /// </summary>
    public class UIAudioPanel : MonoBehaviour
    {
        [Header("Volume Controls")]
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private TMP_Text musicVolumeValueText;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private TMP_Text sfxVolumeValueText;
        
        [Header("Audio Options")]
        [SerializeField] private Toggle muteToggle;
        [SerializeField] private Toggle vibrationsToggle;
        
        [Header("Test Buttons")]
        [SerializeField] private Button testMusicButton;
        [SerializeField] private Button testSFXButton;
        
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
            // Initialize Music Volume Slider
            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.minValue = 0f;
                musicVolumeSlider.maxValue = 10f;
                musicVolumeSlider.wholeNumbers = true;
            }
            
            // Initialize SFX Volume Slider
            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.minValue = 0f;
                sfxVolumeSlider.maxValue = 10f;
                sfxVolumeSlider.wholeNumbers = true;
            }
            
            RefreshUI();
        }
        
        private void SetupEventListeners()
        {
            // Music Volume
            if (musicVolumeSlider != null)
                musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
                
            // SFX Volume
            if (sfxVolumeSlider != null)
                sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
                
            // Mute Toggle
            if (muteToggle != null)
                muteToggle.onValueChanged.AddListener(OnMuteChanged);
                
            // Vibrations Toggle
            if (vibrationsToggle != null)
                vibrationsToggle.onValueChanged.AddListener(OnVibrationsChanged);
                
            // Test Buttons
            if (testMusicButton != null)
                testMusicButton.onClick.AddListener(OnTestMusicClicked);
                
            if (testSFXButton != null)
                testSFXButton.onClick.AddListener(OnTestSFXClicked);
        }
        
        private void RefreshUI()
        {
            if (_settingsService == null) return;
            
            // Update Music Volume
            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.value = _settingsService.MusicVolume;
                UpdateMusicVolumeText();
            }
            
            // Update SFX Volume
            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.value = _settingsService.SFXVolume;
                UpdateSFXVolumeText();
            }
            
            // Update Mute Toggle
            if (muteToggle != null)
                muteToggle.isOn = _settingsService.IsMute();
                
            // Update Vibrations Toggle
            if (vibrationsToggle != null)
                vibrationsToggle.isOn = _settingsService.Vibrations;
                
            // Update slider interactability based on mute state
            bool isMuted = _settingsService.IsMute();
            if (musicVolumeSlider != null)
                musicVolumeSlider.interactable = !isMuted;
            if (sfxVolumeSlider != null)
                sfxVolumeSlider.interactable = !isMuted;
        }
        
        private void OnMusicVolumeChanged(float value)
        {
            _settingsService.MusicVolume = (int)value;
            UpdateMusicVolumeText();
        }
        
        private void OnSFXVolumeChanged(float value)
        {
            _settingsService.SFXVolume = (int)value;
            UpdateSFXVolumeText();
        }
        
        private void OnMuteChanged(bool isMuted)
        {
            _settingsService.MuteSound(isMuted);
            RefreshUI(); // Refresh to update slider interactability and values
        }
        
        private void OnVibrationsChanged(bool value)
        {
            _settingsService.Vibrations = value;
        }
        
        private void UpdateMusicVolumeText()
        {
            if (musicVolumeValueText != null)
                musicVolumeValueText.text = _settingsService.MusicVolumeText;
        }
        
        private void UpdateSFXVolumeText()
        {
            if (sfxVolumeValueText != null)
                sfxVolumeValueText.text = _settingsService.SFXVolumeText;
        }
        
        private void OnTestMusicClicked()
        {
            // TODO: Play a test music sample
            // This would typically interface with the audio service
            Debug.Log("Test Music clicked - would play music sample");
        }
        
        private void OnTestSFXClicked()
        {
            // TODO: Play a test SFX sample
            // This would typically interface with the audio service
            Debug.Log("Test SFX clicked - would play SFX sample");
        }
    }
}
