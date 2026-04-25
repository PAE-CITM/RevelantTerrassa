using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PuzzlePiece : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private ConnectionNode[] nodes;
    private Rigidbody rb;

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

    private void OnDrop(SelectExitEventArgs args)
    {
        foreach (var node in nodes)
        {
            if (node.isMatched)
            {
                SnapToPlace();
                return;
            }
        }
    }

    void SnapToPlace()
    {
        rb.isKinematic = true;

        // OPCIONAL: Podries posar aquí un so de "click" o unes partícules
        Debug.Log("Peça col·locada correctament!");
    }

    public void ResetPiece()
    {
        rb.isKinematic = false;
    }
}