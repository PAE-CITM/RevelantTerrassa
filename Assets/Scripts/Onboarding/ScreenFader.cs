using System;
using System.Collections;
using System.Threading.Tasks;
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
                transform.root.gameObject.SetActive(false);
                Destroy(transform.root.gameObject);
                return;
            }

            Instance = this;

            fadeImage = GetComponent<Image>();
            canvas = GetComponentInParent<Canvas>();

            transform.root.SetParent(null);
            transform.root.localScale = Vector3.one;
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
                FadeAsync(1.0f, 0.0f,autoFadeDuration);
        }

        private void OnDestroy()
        {
            Instance = null;
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

        private async void FadeInAsyncAfterSceneLoad()
        {
            SetAlpha(1f);

            // Ideally this gets called ONCE (similar performance penalty to calling GetComponent() repeatedly)
            while (!Camera.main)
                await Task.Delay(50);
            

            FindCamera();
            await Task.Yield();
            FadeAsync(1.0f, 0.0f,sceneFadeInDuration > 0f ? sceneFadeInDuration : 5f);
        }

        private IEnumerator FadeInAfterSceneLoad()
        {
            SetAlpha(1f);
            
            // Ideally this gets called ONCE (similar performance penalty to calling GetComponent() repeatedly)
            while (!Camera.main)
                yield return null;
            

            FindCamera();
            if (targetCamera != null)
                FollowCamera();

            FadeAsync(1.0f, 0.0f,sceneFadeInDuration > 0f ? sceneFadeInDuration : 5f);
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
            rt.localPosition = Vector3.zero;
            rt.localRotation = Quaternion.identity;

            canvas.sortingOrder = 999;

            var overlays = canvas.GetComponents<MonoBehaviour>();
            foreach (var comp in overlays)
            {
                if (comp != null && comp.GetType().Name.Contains("OVROverlay"))
                {
                    comp.enabled = false;
                }
            }

            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler != null)
                scaler.enabled = false;

            RectTransform imageRT = fadeImage.GetComponent<RectTransform>();
            imageRT.anchorMin = Vector2.zero;
            imageRT.anchorMax = Vector2.one;
            imageRT.offsetMin = Vector2.zero;
            imageRT.offsetMax = Vector2.zero;
            imageRT.localPosition = Vector3.zero;
            imageRT.localRotation = Quaternion.identity;
            imageRT.localScale = Vector3.one;
        }

        private void FollowCamera()
        {
            if (targetCamera != null)
            {
                canvas.transform.position = targetCamera.transform.position + targetCamera.transform.forward * distanceFromCamera;
                canvas.transform.rotation = targetCamera.transform.rotation;
            }
        }
        
        public async Task FadeAsync(float fromAlpha, float toAlpha, float duration)
        {
            SetAlpha(fromAlpha);
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                SetAlpha(Mathf.Lerp(fromAlpha, toAlpha, elapsed / duration));
                await Task.Yield();
            }

            SetAlpha(toAlpha);
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

        public void SetAlpha(float alpha)
        {
            Color color = fadeImage.color;
            color.a = alpha;
            fadeImage.color = color;
        }
    }
}
