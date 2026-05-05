using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace OnBoarding
{
    [RequireComponent(typeof(Image))]
    public class ScreenFader : MonoBehaviour
    {
        private Image fadeImage;

        private void Awake()
        {
            fadeImage = GetComponent<Image>();
            SetAlpha(1f);
        }

        public IEnumerator FadeInRoutine(float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                SetAlpha(Mathf.Lerp(1f, 0f, elapsed / duration));
                yield return null;
            }

            SetAlpha(0f);
        }

        private void SetAlpha(float alpha)
        {
            Color color = fadeImage.color;
            color.a = alpha;
            fadeImage.color = color;
        }
    }
}
