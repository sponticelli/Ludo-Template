using System.Collections;
using Game.MainMenu.Credits.Data;
using Game.UI;
using Ludo.Localization;
using TMPro;
using UnityEngine;

namespace Game.MainMenu.Credits
{
    public class UICreditsPanel : UIPopup
    {
        [SerializeField] private RectTransform _creditsContainer;
        
        [Header("Configuration")]
        [SerializeField] private CreditsData _creditsData;
        [SerializeField] LocalizedTMProText _sectionTitlePrefab;
        [SerializeField] TMP_Text _entryPrefab;
        [SerializeField] private bool autoScroll = false;
        [SerializeField] private float scrollSpeed = 50f;

        private bool _initialized;
        private Coroutine _autoScrollCoroutine;
        
        
        public void Initialize()
        {
            ResetScrollPosition();
            if (_initialized) return;
            _initialized = true;
            BuildCredits();
            StartAutoScrollIfNeeded();
        }

        private void ResetScrollPosition()
        {
            _creditsContainer.anchoredPosition = new Vector2(_creditsContainer.anchoredPosition.x, 0);
        }

        private void BuildCredits()
        {
            foreach (var group in _creditsData.Groups)
            {
                var title = Instantiate(_sectionTitlePrefab, _creditsContainer);
                title.SetKey(group.TitleKey);
                foreach (var entry in group.Entries)
                {
                    var e = Instantiate(_entryPrefab, _creditsContainer);
                    e.text = entry.Name;
                }
            }
            
            // Add 3 empty entries to make sure the last entry is not cut off
            for (var i = 0; i < 3; i++)
            {
                var e = Instantiate(_entryPrefab, _creditsContainer);
                e.text = " ";
            }

        }

        private void StartAutoScrollIfNeeded()
        {
            if (autoScroll && IsVisible)
            {
                if (_autoScrollCoroutine != null)
                {
                    StopCoroutine(_autoScrollCoroutine);
                }
                _autoScrollCoroutine = StartCoroutine(AutoScrollCoroutine());
            }
        }

        private void StopAutoScroll()
        {
            if (_autoScrollCoroutine != null)
            {
                StopCoroutine(_autoScrollCoroutine);
                _autoScrollCoroutine = null;
            }
        }
        

        private IEnumerator AutoScrollCoroutine()
        {
            while (autoScroll && IsVisible)
            {
                // Move the credits container up
                var currentPos = _creditsContainer.anchoredPosition;
                var newY = currentPos.y + scrollSpeed * Time.deltaTime;

                // Get the total height of the content
                var contentHeight = _creditsContainer.rect.height;
                var viewportHeight = ((RectTransform)_creditsContainer.parent).rect.height;

                // Reset to bottom when we've scrolled past the top
                if (newY > contentHeight + viewportHeight)
                {
                    newY = -viewportHeight;
                }

                _creditsContainer.anchoredPosition = new Vector2(currentPos.x, newY);
                yield return null;
            }
        }

        private void Update()
        {
            // Start auto-scroll if conditions are met and it's not already running
            if (autoScroll && IsVisible && _autoScrollCoroutine == null)
            {
                StartAutoScrollIfNeeded();
            }
            // Stop auto-scroll if conditions are no longer met
            else if ((!autoScroll || !IsVisible) && _autoScrollCoroutine != null)
            {
                StopAutoScroll();
            }
        }

        private void OnDisable()
        {
            StopAutoScroll();
        }
    }
}