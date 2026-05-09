using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace OnBoarding
{
    [RequireComponent(typeof(Image))]
    public class ScreenFader : MonoBehaviour
    {
        private Image fadeImage;

        [Header("Auto Fade Settings")]
        public bool fadeOnAwake = false;
        public float autoFadeDuration = 3f;

        private void Awake()
        {
            fadeImage = GetComponent<Image>();
            SetAlpha(1f); // Comienza en negro siempre si va a hacer fade
        }

        private void Start()
        {
            if (fadeOnAwake)
            {
                StartCoroutine(FadeInRoutine(autoFadeDuration));
            }
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

        public IEnumerator FadeOutRoutine(float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                SetAlpha(Mathf.Lerp(0f, 1f, elapsed / duration));
                yield return null;
            }

            SetAlpha(1f);
        }

        private void SetAlpha(float alpha)
        {
            Color color = fadeImage.color;
            color.a = alpha;
            fadeImage.color = color;
        }
    }
}
