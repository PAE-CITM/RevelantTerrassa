using System;
using System.Collections;
using UnityEngine;

namespace OnBoarding
{
    public class PhotoInteractable : MonoBehaviour, IGazeable
    {
        [SerializeField] private Renderer photoRenderer;
        [SerializeField] private float fadeDuration = 2f;
        [SerializeField] private float requiredLookTime = 2.5f;
        [SerializeField] private KeyCode debugSkipKey = KeyCode.Space;

        public event Action OnCompleted;

        private float currentLookTime;
        private bool isCompleted;

        private void OnEnable()
        {
            currentLookTime = 0f;
            isCompleted = false;
            SetAlpha(1f);
        }

        private void Update()
        {
            if (isCompleted)
                return;

            if (Input.GetKeyDown(debugSkipKey))
                CompletePhoto();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            StartCoroutine(Fade(0f, 1f));
        }

        public void OnGazeStay()
        {
            if (isCompleted) return;

            currentLookTime += Time.deltaTime;

            if (currentLookTime >= requiredLookTime)
                CompletePhoto();
        }

        public void OnGazeExit()
        {
            if (isCompleted) return;
            currentLookTime = Mathf.Max(0f, currentLookTime - 0.3f);
        }

        private void CompletePhoto()
        {
            if (isCompleted) return;
            isCompleted = true;
            StartCoroutine(FadeAndComplete());
        }

        private IEnumerator FadeAndComplete()
        {
            yield return StartCoroutine(Fade(1f, 0f));
            gameObject.SetActive(false);
            OnCompleted?.Invoke();
        }

        private IEnumerator Fade(float from, float to)
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;
                SetAlpha(Mathf.Lerp(from, to, t));
                yield return null;
            }
            SetAlpha(to);
        }

        private void SetAlpha(float alpha)
        {
            if (photoRenderer == null) return;
            
            Color color = photoRenderer.material.color;
            color.a = alpha;
            photoRenderer.material.color = color;
        }
    }
}