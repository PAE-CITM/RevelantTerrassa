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

        [Header("Hand Touch Trigger Settings")]
        [SerializeField] private Collider handTouchTrigger;


        private bool isInserted = false;
        private bool isReadyForTouch = false;

        private void Awake()
        {
            OnCameraRollInserted += StartPhase3;
        }

        private void Start()
        {
            if (handTouchTrigger != null)
            {
                handTouchTrigger.gameObject.SetActive(false);

                TriggerListener listener = handTouchTrigger.GetComponent<TriggerListener>();
                if (listener == null)
                {
                    listener = handTouchTrigger.gameObject.AddComponent<TriggerListener>();
                }
                listener.onTriggerEntered += HandleHandTouch;
            }
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
                isReadyForTouch = false;
                if (handTouchTrigger != null)
                {
                    handTouchTrigger.gameObject.SetActive(true);
                }
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

            if (handTouchTrigger == null)
            {
                OnCameraRollInserted?.Invoke();
            }
            else
            {
                isReadyForTouch = true;
            }
        }

        private void HandleHandTouch(Collider other)
        {
            if (!isInserted || !isReadyForTouch) return;

            if (IsHand(other))
            {
                if (handTouchTrigger != null)
                {
                    handTouchTrigger.gameObject.SetActive(false);
                }

                OnCameraRollInserted?.Invoke();
            }
        }

        private bool IsHand(Collider other)
        {
            if (other == null) return false;

            string nameLower = other.name.ToLower();
            string tagLower = other.tag.ToLower();

            if (nameLower.Contains("hand") || nameLower.Contains("finger") || nameLower.Contains("palm") ||
                nameLower.Contains("index") || nameLower.Contains("thumb") || nameLower.Contains("contact"))
            {
                return true;
            }

            if (tagLower.Contains("hand") || tagLower.Contains("player"))
            {
                return true;
            }

            Transform current = other.transform;
            while (current != null)
            {
                Component[] components = current.GetComponents<Component>();
                if (components != null)
                {
                    for (int i = 0; i < components.Length; i++)
                    {
                        if (components[i] == null) continue;
                        string typeName = components[i].GetType().Name;
                        if (typeName.Contains("Hand") || typeName.Contains("OVRHand") || typeName.Contains("HandRef"))
                        {
                            return true;
                        }
                    }
                }
                current = current.parent;
            }

            return false;
        }
    }
}