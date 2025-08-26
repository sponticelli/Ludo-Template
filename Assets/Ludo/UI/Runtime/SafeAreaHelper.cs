using UnityEngine;

namespace Ludo.UI
{
    public class SafeAreaHelper : MonoBehaviour
    {
        private Rect lastSafeArea;

        private RectTransform parentRectTransform;

        private void Start()
        {
            parentRectTransform = GetComponentInParent<RectTransform>();
        }

        private void Update()
        {
            if (lastSafeArea != Screen.safeArea)
            {
                ApplySafeArea();
            }
        }

        private void ApplySafeArea()
        {
            Rect safeArea = Screen.safeArea;
            float num = parentRectTransform.rect.width / (float)Screen.width;
            float x = safeArea.xMin * num;
            float x2 = (0f - ((float)Screen.width - safeArea.xMax)) * num;
            float y = ((float)Screen.height - safeArea.yMax) * num;
            float y2 = safeArea.yMin * num;
            RectTransform component = GetComponent<RectTransform>();
            component.offsetMin = new Vector2(x, y2);
            component.offsetMax = new Vector2(x2, y);
            lastSafeArea = Screen.safeArea;
        }
        
    }
}