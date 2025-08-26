using UnityEngine;
using UnityEngine.UI;

namespace Game.SettingsMenu
{
    /// <summary>
    /// UI panel containing category buttons for settings navigation (Display, Audio, Language)
    /// </summary>
    public class UICategoryPanel : MonoBehaviour
    {
        [Header("Category Buttons")]
        [SerializeField] private Button displayButton;
        [SerializeField] private Button audioButton;
        [SerializeField] private Button languageButton;
        
        [Header("Visual Feedback")]
        [SerializeField] private Color selectedColor = Color.white;
        [SerializeField] private Color normalColor = Color.gray;
        
        private SettingsMenuFlowController _flowController;
        private Button _currentSelectedButton;
        
        private void Awake()
        {
            // Find the flow controller in the scene
            _flowController = FindObjectOfType<SettingsMenuFlowController>();
            
            if (_flowController == null)
            {
                Debug.LogError("SettingsMenuFlowController not found in scene!");
                return;
            }
            
            // Setup button listeners
            if (displayButton != null)
                displayButton.onClick.AddListener(() => OnCategorySelected(SettingsMenuEvent.Display, displayButton));
                
            if (audioButton != null)
                audioButton.onClick.AddListener(() => OnCategorySelected(SettingsMenuEvent.Sound, audioButton));
                
            if (languageButton != null)
                languageButton.onClick.AddListener(() => OnCategorySelected(SettingsMenuEvent.Language, languageButton));
        }
        
        private void Start()
        {
            // Set initial selection to Display
            SetSelectedButton(displayButton);
        }
        
        private void OnCategorySelected(SettingsMenuEvent category, Button selectedButton)
        {
            // Update visual selection
            SetSelectedButton(selectedButton);
            
            // Dispatch the event to the flow controller
            switch (category)
            {
                case SettingsMenuEvent.Display:
                    _flowController.Display();
                    break;
                case SettingsMenuEvent.Sound:
                    _flowController.Sound();
                    break;
                case SettingsMenuEvent.Language:
                    _flowController.Language();
                    break;
            }
        }
        
        private void SetSelectedButton(Button button)
        {
            // Reset all buttons to normal color
            if (displayButton != null)
                SetButtonColor(displayButton, normalColor);
            if (audioButton != null)
                SetButtonColor(audioButton, normalColor);
            if (languageButton != null)
                SetButtonColor(languageButton, normalColor);
            
            // Set selected button to selected color
            if (button != null)
            {
                SetButtonColor(button, selectedColor);
                _currentSelectedButton = button;
            }
        }
        
        private void SetButtonColor(Button button, Color color)
        {
            var colors = button.colors;
            colors.normalColor = color;
            colors.selectedColor = color;
            button.colors = colors;
        }
        
        /// <summary>
        /// Public method to update selection from external sources
        /// </summary>
        public void UpdateSelection(SettingsMenuEvent currentState)
        {
            Button targetButton = currentState switch
            {
                SettingsMenuEvent.Display => displayButton,
                SettingsMenuEvent.Sound => audioButton,
                SettingsMenuEvent.Language => languageButton,
                _ => displayButton
            };
            
            SetSelectedButton(targetButton);
        }
    }
}
