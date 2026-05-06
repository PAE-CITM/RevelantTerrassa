using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Oculus.Interaction;

public class CameraRollReceptacle : MonoBehaviour
{
    public string cameraRollName = "CameraRoll";
    
    public float delay = 1.5f;
    
    public Transform snapPoint;
    public UnityEvent OnCameraRollInserted;

    private bool isInserted = false;

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
