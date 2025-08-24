using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UIPopup : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _showDuration = 0.3f;
        [SerializeField] private float _hideDuration = 0.3f;
        
        [Header("References")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private RectTransform _contentRectTransform;
        
        
        private void Awake()
        {
            if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
            if (_backgroundImage == null) _backgroundImage = GetComponent<Image>();
            if (_contentRectTransform == null) _contentRectTransform = GetComponent<RectTransform>();
        }
        
        public void ImmediateShow()
        {
            _canvasGroup.alpha = 1;
            _backgroundImage.raycastTarget = true;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
        }
        
        public void ImmediateHide()
        {
            _canvasGroup.alpha = 0;
            _backgroundImage.raycastTarget = false;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
        }
        
        public void Show()
        {
            Debug.Log("Show popup");
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = true;
            _backgroundImage.raycastTarget = true;
            _canvasGroup.blocksRaycasts = true;
            StartCoroutine(ShowCoroutine());
        }
        
        public void Hide()
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = false;
            StartCoroutine(HideCoroutine());
        }
        
        private IEnumerator ShowCoroutine()
        {
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / _showDuration;
                Debug.Log("Show popup " + t);
                _canvasGroup.alpha = Mathf.Lerp(0, 1, t);
                yield return null;
            }
            _canvasGroup.interactable = true;
        }
        
        private IEnumerator HideCoroutine()
        {
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / _hideDuration;
                _canvasGroup.alpha = Mathf.Lerp(1, 0, t);
                yield return null;
            }
            _backgroundImage.raycastTarget = false;
            _canvasGroup.blocksRaycasts = false;
        }
        
    }
}