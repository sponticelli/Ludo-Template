using UnityEngine;
using Ludo.Core;
using Ludo.Settings.Runtime;

namespace Game.SettingsMenu
{
    /// <summary>
    /// Validation script to test Settings Menu implementation
    /// This can be attached to a GameObject in the scene for testing purposes
    /// </summary>
    public class SettingsMenuValidator : MonoBehaviour
    {
        [Header("Test Controls")]
        [SerializeField] private bool runValidationOnStart = false;
        [SerializeField] private bool logDetailedResults = true;
        
        private SettingsMenuFlowController _flowController;
        private ISettingsService _settingsService;
        
        private void Start()
        {
            if (runValidationOnStart)
            {
                Invoke(nameof(RunValidation), 1f); // Delay to ensure everything is initialized
            }
        }
        
        [ContextMenu("Run Validation")]
        public void RunValidation()
        {
            Debug.Log("=== Settings Menu Validation Started ===");
            
            bool allTestsPassed = true;
            
            allTestsPassed &= ValidateServiceLocatorIntegration();
            allTestsPassed &= ValidateFlowController();
            allTestsPassed &= ValidateSettingsService();
            allTestsPassed &= ValidateStateTransitions();
            
            Debug.Log($"=== Settings Menu Validation {(allTestsPassed ? "PASSED" : "FAILED")} ===");
        }
        
        private bool ValidateServiceLocatorIntegration()
        {
            Debug.Log("Testing ServiceLocator Integration...");
            
            try
            {
                _settingsService = ServiceLocator.Get<ISettingsService>();
                if (_settingsService == null)
                {
                    Debug.LogError("❌ ISettingsService not found in ServiceLocator");
                    return false;
                }
                
                _flowController = ServiceLocator.Get<SettingsMenuFlowController>();
                if (_flowController == null)
                {
                    Debug.LogError("❌ SettingsMenuFlowController not found in ServiceLocator");
                    return false;
                }
                
                if (logDetailedResults)
                {
                    Debug.Log("✅ ServiceLocator integration working correctly");
                }
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ ServiceLocator integration failed: {e.Message}");
                return false;
            }
        }
        
        private bool ValidateFlowController()
        {
            Debug.Log("Testing Flow Controller...");
            
            if (_flowController == null)
            {
                _flowController = FindObjectOfType<SettingsMenuFlowController>();
            }
            
            if (_flowController == null)
            {
                Debug.LogError("❌ SettingsMenuFlowController not found in scene");
                return false;
            }
            
            // Test that the flow controller has access to settings service
            if (_flowController.SettingsService == null)
            {
                Debug.LogError("❌ Flow controller doesn't have access to SettingsService");
                return false;
            }
            
            if (logDetailedResults)
            {
                Debug.Log("✅ Flow controller validation passed");
            }
            return true;
        }
        
        private bool ValidateSettingsService()
        {
            Debug.Log("Testing Settings Service...");
            
            if (_settingsService == null)
            {
                Debug.LogError("❌ Settings service not available");
                return false;
            }
            
            try
            {
                // Test basic property access
                var language = _settingsService.Language;
                var musicVolume = _settingsService.MusicVolume;
                var quality = _settingsService.Quality;
                var resolution = _settingsService.Resolution;
                
                if (logDetailedResults)
                {
                    Debug.Log($"✅ Settings Service working - Language: {language}, Music: {musicVolume}, Quality: {quality}");
                }
                
                // Test property modification
                int originalVolume = _settingsService.MusicVolume;
                _settingsService.MusicVolume = 7;
                if (_settingsService.MusicVolume != 7)
                {
                    Debug.LogError("❌ Settings service property modification failed");
                    return false;
                }
                _settingsService.MusicVolume = originalVolume; // Restore
                
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Settings service validation failed: {e.Message}");
                return false;
            }
        }
        
        private bool ValidateStateTransitions()
        {
            Debug.Log("Testing State Transitions...");
            
            if (_flowController == null)
            {
                Debug.LogError("❌ Cannot test state transitions - flow controller not available");
                return false;
            }
            
            try
            {
                // Test state transitions (these should not throw exceptions)
                _flowController.Display();
                _flowController.Sound();
                _flowController.Language();
                _flowController.Display(); // Back to display
                
                if (logDetailedResults)
                {
                    Debug.Log("✅ State transitions executed without errors");
                }
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ State transition validation failed: {e.Message}");
                return false;
            }
        }
        
        [ContextMenu("Test Display Settings")]
        public void TestDisplaySettings()
        {
            if (_settingsService == null)
            {
                _settingsService = ServiceLocator.Get<ISettingsService>();
            }
            
            Debug.Log("=== Testing Display Settings ===");
            Debug.Log($"Current Resolution: {_settingsService.Resolution}");
            Debug.Log($"Current Quality: {_settingsService.Quality}");
            Debug.Log($"VSync: {_settingsService.VSync}");
            Debug.Log($"UI Scale: {_settingsService.GameUIScaleText}");
        }
        
        [ContextMenu("Test Audio Settings")]
        public void TestAudioSettings()
        {
            if (_settingsService == null)
            {
                _settingsService = ServiceLocator.Get<ISettingsService>();
            }
            
            Debug.Log("=== Testing Audio Settings ===");
            Debug.Log($"Music Volume: {_settingsService.MusicVolumeText}");
            Debug.Log($"SFX Volume: {_settingsService.SFXVolumeText}");
            Debug.Log($"Vibrations: {_settingsService.Vibrations}");
            Debug.Log($"Is Muted: {_settingsService.IsMute()}");
        }
        
        [ContextMenu("Test Language Settings")]
        public void TestLanguageSettings()
        {
            if (_settingsService == null)
            {
                _settingsService = ServiceLocator.Get<ISettingsService>();
            }
            
            Debug.Log("=== Testing Language Settings ===");
            Debug.Log($"Current Language: {_settingsService.Language}");
        }
    }
}
