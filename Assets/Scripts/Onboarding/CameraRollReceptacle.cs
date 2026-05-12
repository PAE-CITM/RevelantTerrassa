using UnityEngine;
using System;
using System.Collections;
using Oculus.Interaction;
namespace OnBoarding
{
    public class CameraRollReceptacle : MonoBehaviour
    {
        [SerializeField] private GameObject phase3;

        public string cameraRollName = "CameraRoll";

        public float delay = 1.5f;

        public Transform snapPoint;
        public event Action OnCameraRollInserted;

        [SerializeField] private narratorAudio narrator;
        public AudioClip narratorAudio;


        private bool isInserted = false;

        private void Awake()
        {
            OnCameraRollInserted += StartPhase3;
        }

        private void StartPhase3()
        {
            narrator.PlayClip(narratorAudio);
            phase3.SetActive(true);
        }

        private void OnTriggerEnter(Collider other)
        {

            if (isInserted) return;

            if (other.gameObject.name.Contains(cameraRollName))
            {
                isInserted = true;
                StartCoroutine(HandleInsertion(other.gameObject));
            }
        }

        private IEnumerator HandleInsertion(GameObject cameraRoll)
        {
            var interactables = cameraRoll.GetComponentsInChildren<IInteractableView>();
            foreach (var interactable in interactables)
            {
                if (interactable is MonoBehaviour mb) mb.enabled = false;
            }

            Grabbable grabbable = cameraRoll.GetComponent<Grabbable>();
            if (grabbable != null) grabbable.enabled = false;

            Rigidbody rb = cameraRoll.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.detectCollisions = false;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            Transform targetTransform = snapPoint != null ? snapPoint : transform;
            cameraRoll.transform.SetParent(targetTransform);
            cameraRoll.transform.position = targetTransform.position;
            cameraRoll.transform.rotation = targetTransform.rotation;

            yield return new WaitForSeconds(delay);

            OnCameraRollInserted?.Invoke();
        }
    }
}