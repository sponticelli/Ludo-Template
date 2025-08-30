using UnityEngine;

namespace Ludo.UI
{
    public class SafeAreaHelper : MonoBehaviour
    {
        private Rect _lastSafeArea;
        private RectTransform _parentRectTransform;
        private RectTransform _rectTransform;

        private void Start()
        {
            _parentRectTransform = GetComponentInParent<RectTransform>();
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (_lastSafeArea != Screen.safeArea)
            {
                ApplySafeArea();
            }
        }

        private void ApplySafeArea()
        {
            Rect safeArea = Screen.safeArea;
            float uiToScreenScale = _parentRectTransform.rect.width / Screen.width;
            float x = safeArea.xMin * uiToScreenScale;
            float x2 = (0f - (Screen.width - safeArea.xMax)) * uiToScreenScale;
            float y = (Screen.height - safeArea.yMax) * uiToScreenScale;
            float y2 = safeArea.yMin * uiToScreenScale;
            _rectTransform.offsetMin = new Vector2(x, y2);
            _rectTransform.offsetMax = new Vector2(x2, y);
            _lastSafeArea = Screen.safeArea;
        }
        
    }
}