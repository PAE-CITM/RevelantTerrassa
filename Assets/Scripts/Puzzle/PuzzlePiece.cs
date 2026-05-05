using Oculus.Interaction;
using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzlePiece : MonoBehaviour
{
    private Grabbable grabInteractable;

    private ConnectionNode node;

    private Rigidbody rb;
    public AudioSource sound;
    public ParticleSystem particle;

    public bool isLocked = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        node = GetComponentInChildren<ConnectionNode>();

        grabInteractable = GetComponent<Grabbable>();

        if (grabInteractable != null)
        {
            grabInteractable.WhenPointerEventRaised += OnDrop;
        }
    }

    void Update()
    {
        if (isLocked) return;

        if (Keyboard.current != null && Keyboard.current.sKey.wasPressedThisFrame)
        {
            TryManualSnap();
        }
    }

    public void TryManualSnap()
    {
        if (isLocked) return;

        if (node != null && node.isMatched && node.targetNode != null)
        {
            SnapToPlace(node);
        }
    }

    private void OnDrop(PointerEvent args)
    {
        if (isLocked || args.Type != PointerEventType.Unselect) return;

        if (node != null && node.isMatched && node.targetNode != null)
        {
            SnapToPlace(node);
        }
    }

    void SnapToPlace(ConnectionNode matchedNode)
    {
        if (isLocked) return;
        isLocked = true;

        rb.isKinematic = true;
        rb.detectCollisions = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.SetParent(matchedNode.targetNode.transform);

        transform.rotation = matchedNode.targetNode.transform.rotation;
        Vector3 nodeOffset = transform.position - matchedNode.transform.position;
        transform.position = matchedNode.targetNode.transform.position + nodeOffset;

        if (grabInteractable != null) grabInteractable.enabled = false;

        rb.detectCollisions = true;

        PuzzleManager manager = GetComponentInParent<PuzzleManager>();
        if (manager != null)
        {
            manager.CheckCompletion();
        }

        if (sound != null) sound.Play();
        if (particle != null) particle.Play();
    }



    public void ResetPiece()
    {
        isLocked = false;
        rb.isKinematic = false;
        if (grabInteractable != null) grabInteractable.enabled = true;
    }
}