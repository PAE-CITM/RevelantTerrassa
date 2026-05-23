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

        private bool isFading = false;

        [SerializeField]
        private PuzzleManager puzzleManager;

        [SerializeField]
        private NPCWaypointNavigator npcNavigator;

        [SerializeField]
        private GameObject phase4;

        [SerializeField]
        private GameObject ground_circle;

        [SerializeField]
        private GameObject phase3;
        
        [SerializeField]
        private GameObject nen;

        [SerializeField]
        private float rotationDuration = 0.5f;

        public void Awake()
        {
            OnPhaseFourCompleted += TriggerPhaseFour;

            puzzleManager.OnPuzzleCompleted.AddListener(EnablePhaseFour);


            if (interactionZone != null && interactionZone.gameObject != this.gameObject)
            {
                TriggerListener listener = interactionZone.gameObject.AddComponent<TriggerListener>();
                listener.onTriggerEntered += OnTriggerEnter;
            }
        }

        private void OnDestroy()
        {
            OnPhaseFourCompleted -= TriggerPhaseFour;  
            puzzleManager?.OnPuzzleCompleted.RemoveListener(EnablePhaseFour);
        }

        private void EnablePhaseFour()
        {
            StartCoroutine(DisablePhaseThreeDelayed(2.5f));

            if (npcNavigator != null)
            {
                StartCoroutine(RotateSmoothly(nen.transform, 75f, rotationDuration));
                npcNavigator.OnArrivedAtWaypoint += ActivatePhase4;
                npcNavigator.MoveToNext();
            }
            else
            {
                ActivatePhase4();
            }
        }

        private IEnumerator RotateSmoothly(Transform targetTransform, float angleY, float duration)
        {
            Quaternion startRot = targetTransform.rotation;
            Quaternion endRot = startRot * Quaternion.Euler(0f, angleY, 0f);

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                targetTransform.rotation = Quaternion.Slerp(startRot, endRot, t);
                yield return null;
            }
            targetTransform.rotation = endRot;
        }

        private void ActivatePhase4()
        {
            if (npcNavigator != null)
                npcNavigator.OnArrivedAtWaypoint -= ActivatePhase4;

            phase4.SetActive(true);
            ground_circle.SetActive(true);
        }

        private IEnumerator DisablePhaseThreeDelayed(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (phase3 != null)
            {
                phase3.SetActive(false);
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

            ScreenFader fader = ScreenFader.Instance;

            if (fader != null)
            {
                yield return StartCoroutine(fader.FadeOutRoutine(3f));
                fader.PrepareForSceneTransition();
            }

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Plaça Vella v1");
            asyncLoad.allowSceneActivation = false;

            while (asyncLoad.progress < 0.9f)
            {
                yield return null;
            }

            asyncLoad.allowSceneActivation = true;
        }

        private void OnTriggerEnter(Collider other)
        {    
            if (other.CompareTag("Player"))
            {
                OnPhaseFourCompleted?.Invoke();
            }
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
