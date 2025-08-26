using Game.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.SettingsMenu
{
    /// <summary>
    /// Main settings panel that coordinates the category panel and individual settings panels
    /// </summary>
    public class UISettingsPanel : UIPopup
    {
        [Header("Panel Structure")]
        [SerializeField] private UICategoryPanel categoryPanel;
        [SerializeField] private UIDisplayPanel displayPanel;
        [SerializeField] private UIAudioPanel audioPanel;
        [SerializeField] private UILanguagePanel languagePanel;

        [Header("Back Button")]
        [SerializeField] private Button backButton;

        private SettingsMenuFlowController _flowController;

        protected override void Awake()
        {
            base.Awake();

            // Find the flow controller
            _flowController = FindObjectOfType<SettingsMenuFlowController>();

            if (_flowController == null)
            {
                Debug.LogError("SettingsMenuFlowController not found in scene!");
            }
        }

        private void Start()
        {
            // Setup back button
            if (backButton != null && _flowController != null)
            {
                backButton.onClick.AddListener(_flowController.Back);
            }
        }

        /// <summary>
        /// Updates the category panel selection when state changes
        /// </summary>
        public void OnStateChanged(SettingsMenuEvent currentState)
        {
            if (categoryPanel != null)
            {
                categoryPanel.UpdateSelection(currentState);
            }
        }

        /// <summary>
        /// Refreshes all panels when the settings panel becomes active
        /// </summary>
        public override void Show()
        {
            base.Show();
            RefreshAllPanels();
        }

        private void RefreshAllPanels()
        {
            // This method is called when the settings panel is shown
            // The actual panel visibility is managed by the flow controller's OnStateChanged method
            // We just ensure the panels are properly initialized
            Debug.Log("Settings panel refreshed - panels will be managed by flow controller");
        }

        /// <summary>
        /// Ensures proper cleanup when the panel is hidden
        /// </summary>
        public override void Hide()
        {
            base.Hide();

            // Hide all individual panels when the main panel is hidden
            if (displayPanel != null) displayPanel.gameObject.SetActive(false);
            if (audioPanel != null) audioPanel.gameObject.SetActive(false);
            if (languagePanel != null) languagePanel.gameObject.SetActive(false);
        }
    }
}