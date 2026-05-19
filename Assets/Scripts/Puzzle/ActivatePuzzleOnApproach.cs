using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ActivatePuzzleOnApproach : MonoBehaviour
{
    [SerializeField] private GameObject objectToActivate;

    // Might be better visually to implement a fade-in and fade-out system for gameobjects
    private void OnTriggerEnter(Collider other)
    {
        objectToActivate.SetActive(true);   
    }

    private void OnTriggerExit(Collider other)
    {
        objectToActivate.SetActive(false);
    }
}
