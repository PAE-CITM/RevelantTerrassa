using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PuzzlePiece : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private ConnectionNode[] nodes;
    private Rigidbody rb;
    public AudioSource sound;
    public ParticleSystem particle;

    public bool isLocked = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        nodes = GetComponentsInChildren<ConnectionNode>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable != null)
        {
            grabInteractable.selectExited.AddListener(OnDrop);
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

        foreach (var node in nodes)
        {
            if (node.isMatched && node.targetNode != null)
            {
                SnapToPlace(node);
                return;
            }
        }
    }

    private void OnDrop(SelectExitEventArgs args)
    {
        if (isLocked) return;

        foreach (var node in nodes)
        {
            if (node.isMatched && node.targetNode != null)
            {
                SnapToPlace(node);
                return;
            }
        }
    }

    void SnapToPlace(ConnectionNode node)
    {
        if (isLocked) return;
        isLocked = true;

        transform.SetParent(null);

        //rb.isKinematic = true;
        rb.detectCollisions = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.rotation = node.targetNode.transform.rotation;

        Vector3 nodeOffset = transform.position - node.transform.position;
        transform.position = node.targetNode.transform.position + nodeOffset;

        GameObject puzzleParent = GameObject.Find("Puzzle_Completat");
        if (puzzleParent != null)
        {
            transform.SetParent(puzzleParent.transform);
        }
        else
        {
            puzzleParent = new GameObject("Puzzle_Completat");
            transform.SetParent(puzzleParent.transform);
        }

        transform.SetParent(puzzleParent.transform);

        if (node.targetNode.transform.root.name == "Puzzle_Completat" || node.targetNode.GetComponentInParent<PuzzlePiece>() == null)
        {
            if (grabInteractable != null) grabInteractable.enabled = false;
        }

        rb.detectCollisions = true;

        sound.Play();
        particle.Play();
    }

    public void ResetPiece()
    {
        isLocked = false;
        //rb.isKinematic = false;
        if (grabInteractable != null) grabInteractable.enabled = true;
    }
}