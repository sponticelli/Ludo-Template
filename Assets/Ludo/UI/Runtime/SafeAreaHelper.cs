using UnityEngine;
using UnityEngine.Events;

namespace Ludo.UI.Runtime
{
    public class SafeAreaHelper : MonoBehaviour
    {
        [SerializeField] private UnityEvent onSafeAreaChanged;
        
        private Rect _lastSafeArea;
        private RectTransform _parentRectTransform;

        private void Start()
        {
            _parentRectTransform = GetComponentInParent<RectTransform>();
        }

        private void Update()
        {
            if (_lastSafeArea != Screen.safeArea)
            {
                ApplySafeArea();
                onSafeAreaChanged?.Invoke();
            }
        }

        private void ApplySafeArea()
        {
            Rect safeArea = Screen.safeArea;
            float num = _parentRectTransform.rect.width / (float)Screen.width;
            float x = safeArea.xMin * num;
            float x2 = (0f - ((float)Screen.width - safeArea.xMax)) * num;
            float y = ((float)Screen.height - safeArea.yMax) * num;
            float y2 = safeArea.yMin * num;
            RectTransform component = GetComponent<RectTransform>();
            component.offsetMin = new Vector2(x, y2);
            component.offsetMax = new Vector2(x2, y);
            _lastSafeArea = Screen.safeArea;
        }
        
    }
}