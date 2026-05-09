using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

namespace OnBoarding
{
    public class OnBoardingGoPlaza : MonoBehaviour
    {

        public event Action OnPhaseFourCompleted;

        [SerializeField] 
        private MeshCollider interactionZone;

        [SerializeField]
        private ScreenFader screenFader;

        private bool isFading = false;

        public void Awake()
        {
            OnPhaseFourCompleted += TriggerPhaseFour;  

            if (interactionZone != null && interactionZone.gameObject != this.gameObject)
            {
                TriggerListener listener = interactionZone.gameObject.AddComponent<TriggerListener>();
                listener.onTriggerEntered += OnTriggerEnter;
            }
        }

        private void TriggerPhaseFour()
        {
            if (!isFading)
            {
                StartCoroutine(FadeAndLoadScene());
            }
        }

        private IEnumerator FadeAndLoadScene()
        {
            isFading = true;
            if (screenFader != null)
            {
                yield return StartCoroutine(screenFader.FadeOutRoutine(3f));
            }
            else
            {
                Debug.LogWarning("ScreenFader no está asignado. Cargando escena directamente.");
            }
            // Cargar de forma asíncrona para evitar el "reloj de arena" y el congelamiento
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Plaça Vella v1");
            asyncLoad.allowSceneActivation = false; // No activamos la escena hasta que termine de cargar

            while (asyncLoad.progress < 0.9f)
            {
                yield return null;
            }

            // Una vez cargada en segundo plano, la activamos
            asyncLoad.allowSceneActivation = true;
        }

        private void OnTriggerEnter(Collider other)
        {    
            OnPhaseFourCompleted?.Invoke();
        }
    }

    public class TriggerListener : MonoBehaviour
    {
        public Action<Collider> onTriggerEntered;
        private void OnTriggerEnter(Collider other)
        {
            onTriggerEntered?.Invoke(other);
        }
    }
}
