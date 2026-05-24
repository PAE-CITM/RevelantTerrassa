using Oculus.Interaction;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PuzzlePiece : MonoBehaviour
{
    private Grabbable grabbable;

    private ConnectionNode node;

    private Rigidbody rb;
    public AudioSource audioSource;
    public AudioClip audioPlace;
    public AudioClip audioGrab;
    public ParticleSystem particle;
    private Outline outline;

    public bool isLocked = false;

    [Header("Snapping Settings")]
    [Tooltip("Marge d'error angular màxim (en graus) per admetre l'encaix.")]
    public float rotationMargin = 30f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        node = GetComponentInChildren<ConnectionNode>();

        grabbable = GetComponent<Grabbable>();

        outline = GetComponent<Outline>();

        if (grabbable != null)
        {
            grabbable.WhenPointerEventRaised += OnDrop;
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (audioSource != null && audioGrab != null)
        {
            audioSource.clip = audioGrab;
        }
    }

    void Start()
    {
        StartCoroutine(CheckInitialSnap());
    }

    void Update()
    {
        if (isLocked) return;

        // Auto-encaix quan la peça entra al seu sensor i la rotació és correcte
        if (node != null && node.isMatched && node.targetNode != null)
        {
            float angleDiff = Quaternion.Angle(transform.rotation, node.targetNode.transform.rotation);
            if (angleDiff <= rotationMargin)
            {
                SnapToPlace(node);
                return;
            }
        }

        if (Keyboard.current != null && Keyboard.current.sKey.wasPressedThisFrame)
        {
            TryManualSnap();
        }
    }

    private IEnumerator CheckInitialSnap()
    {
        yield return new WaitForFixedUpdate();

        if (isLocked) yield break;

        if (node != null && node.isMatched && node.targetNode != null)
        {
            SnapToPlace(node);
        }
    }

    public void TryManualSnap()
    {
        if (isLocked) return;

        if (node != null && node.isMatched && node.targetNode != null)
        {
            float angleDiff = Quaternion.Angle(transform.rotation, node.targetNode.transform.rotation);
            if (angleDiff <= rotationMargin)
            {
                SnapToPlace(node);
            }
        }
    }

    private void OnDrop(PointerEvent args)
    {
        if (isLocked) return;

        if (args.Type == PointerEventType.Select)
        {
            if (audioSource != null && audioGrab != null)
            {
                audioSource.PlayOneShot(audioGrab);
            }
        }
        else if (args.Type == PointerEventType.Unselect)
        {
            if (node != null && node.isMatched && node.targetNode != null)
            {
                float angleDiff = Quaternion.Angle(transform.rotation, node.targetNode.transform.rotation);
                if (angleDiff <= rotationMargin)
                {
                    SnapToPlace(node);
                }
                else
                {
                    Debug.Log($"[PuzzlePiece] Peça '{gameObject.name}' no encaixada per angle incorrecte. Diferència: {angleDiff}° (Marge admès: {rotationMargin}°)");
                }
            }
        }
    }

    void SnapToPlace(ConnectionNode matchedNode)
    {
        if (isLocked) return;
        isLocked = true;
        
        // Disable further interaction once placed in socket. This is a hacky way,
        // but Meta XR SDK docs are a maze and I couldn't find a proper solution 
        var interactables = GetComponentsInChildren<IInteractableView>();
        foreach (var interactable in interactables) if (interactable is MonoBehaviour mb) mb.enabled = false;
        
        rb.isKinematic = true;
        rb.detectCollisions = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.SetParent(matchedNode.targetNode.transform);

        transform.rotation = matchedNode.targetNode.transform.rotation;
        Vector3 nodeOffset = transform.position - matchedNode.transform.position;
        transform.position = matchedNode.targetNode.transform.position + nodeOffset;

        if (grabbable != null) grabbable.enabled = false;

        rb.detectCollisions = true;

        PuzzleManager manager = GetComponentInParent<PuzzleManager>();
        if (manager != null)
        {
            manager.CheckCompletion();
        }

        if (audioSource != null && audioPlace != null)
        {
            audioSource.clip = audioPlace;
            audioSource.Play();
        }

        if (particle != null) particle.Play();
        if (outline != null) outline.enabled = true;
    }



    public void ResetPiece()
    {
        isLocked = false;
        rb.isKinematic = false;
        if (grabbable != null) grabbable.enabled = true;
    }
}
