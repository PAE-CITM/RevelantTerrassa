using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace OnBoarding
{
    [RequireComponent(typeof(Image))]
    public class ScreenFader : MonoBehaviour
    {
        public static ScreenFader Instance { get; private set; }

        private Image fadeImage;
        private Canvas canvas;
        private Camera targetCamera;

        [Header("Auto Fade Settings")]
        public bool fadeOnAwake = false;
        public float autoFadeDuration = 1.5f;

        [Header("VR Settings")]
        public float distanceFromCamera = 0.3f;

        [Header("Scene Transition")]
        public float sceneFadeInDuration = 5.0f;

        private bool waitingForSceneLoad = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(transform.root.gameObject);
                return;
            }

            Instance = this;

            fadeImage = GetComponent<Image>();
            canvas = GetComponentInParent<Canvas>();

            transform.root.SetParent(null);
            DontDestroyOnLoad(transform.root.gameObject);

            if (fadeOnAwake)
                SetAlpha(1f);
            else
                SetAlpha(0f);

            SetupCanvas();
            FindCamera();
        }

        private void Start()
        {
            if (fadeOnAwake)
                StartCoroutine(FadeInRoutine(autoFadeDuration));
        }

        private void LateUpdate()
        {
            if (targetCamera == null)
                FindCamera();

            if (targetCamera != null)
                FollowCamera();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            targetCamera = null;

            if (!waitingForSceneLoad) return;
            waitingForSceneLoad = false;

            StartCoroutine(FadeInAfterSceneLoad());
        }

        private IEnumerator FadeInAfterSceneLoad()
        {
            SetAlpha(1f);

            while (Camera.main == null)
                yield return null;

            FindCamera();
            yield return null;
            yield return FadeInRoutine(sceneFadeInDuration > 0f ? sceneFadeInDuration : 5f);
        }

        public void PrepareForSceneTransition()
        {
            waitingForSceneLoad = true;
        }

        private void FindCamera()
        {
            targetCamera = Camera.main;
        }

        private void SetupCanvas()
        {
            if (canvas == null) return;

            canvas.renderMode = RenderMode.WorldSpace;

            RectTransform rt = canvas.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(15f, 15f);
            rt.localScale = Vector3.one;

            canvas.sortingOrder = 999;

            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler != null)
                scaler.enabled = false;

            RectTransform imageRT = fadeImage.GetComponent<RectTransform>();
            imageRT.anchorMin = Vector2.zero;
            imageRT.anchorMax = Vector2.one;
            imageRT.offsetMin = Vector2.zero;
            imageRT.offsetMax = Vector2.zero;
        }

        private void FollowCamera()
        {
            canvas.transform.position = targetCamera.transform.position + targetCamera.transform.forward * distanceFromCamera;
            canvas.transform.rotation = targetCamera.transform.rotation;
        }

        public IEnumerator FadeInRoutine(float duration)
        {
            SetAlpha(1f);
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
            SetAlpha(0f);
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
