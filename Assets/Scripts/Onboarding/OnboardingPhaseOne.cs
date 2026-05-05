using System;
using System.Collections;
using UnityEngine;

namespace OnBoarding
{
    public class OnboardingPhaseOne : MonoBehaviour
    {
        [SerializeField] private PhotoInteractable[] photos;
        [SerializeField] private float delayBetweenPhotos = 0.5f;

        [SerializeField] private ScreenFader screenFader;
        [SerializeField] private float roomFadeDuration = 2f;
        [SerializeField] private float initialDelay = 1f;

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip photoAppearClip;

        public event Action OnPhaseCompleted;

        private bool currentPhotoCompleted;

        private void Start()
        {
            StartCoroutine(RunPhaseOne());
        }

        private IEnumerator RunPhaseOne()
        {
            yield return new WaitForSeconds(initialDelay);
            
            if (screenFader != null)
                yield return StartCoroutine(screenFader.FadeInRoutine(roomFadeDuration));

            for (int i = 0; i < photos.Length; i++)
            {
                if (i > 0 && audioSource != null && photoAppearClip != null)
                    audioSource.PlayOneShot(photoAppearClip);

                photos[i].OnCompleted += HandlePhotoCompleted;
                photos[i].Show();

                currentPhotoCompleted = false;
                while (!currentPhotoCompleted)
                    yield return null;

                photos[i].OnCompleted -= HandlePhotoCompleted;

                yield return new WaitForSeconds(delayBetweenPhotos);
            }

            OnPhaseCompleted?.Invoke();
        }

        private void HandlePhotoCompleted()
        {
            currentPhotoCompleted = true;
        }
    }
}
