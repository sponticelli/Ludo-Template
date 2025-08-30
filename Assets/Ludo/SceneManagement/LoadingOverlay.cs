using System.Collections;
using UnityEngine;

namespace Ludo.SceneManagement
{
    public class LoadingOverlay : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeDuration = 0.25f;

        private void Reset()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public IEnumerator FadeIn()
        {
            canvasGroup.blocksRaycasts = true;
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Clamp01(t / fadeDuration);
                yield return null;
            }
            canvasGroup.alpha = 1f;
        }

        public IEnumerator FadeOut()
        {
            float t = fadeDuration;
            while (t > 0f)
            {
                t -= Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Clamp01(t / fadeDuration);
                yield return null;
            }
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }
    }
}