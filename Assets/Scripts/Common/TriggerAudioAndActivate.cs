using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TriggerAudioAndActivate : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private List<GameObject> objectsToActivate = new List<GameObject>();
    [SerializeField] private List<GameObject> objectsToDeactivateOnExit = new List<GameObject>();
    [SerializeField] private GameObject preventDestructionIfActive;

    private bool triggered = false;
    private bool playerExited = false;
    private bool audioFinished = false;

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            StartCoroutine(TriggerActionSequence());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (triggered && !playerExited && other.CompareTag("Player"))
        {
            playerExited = true;
            DeactivateExitObjects();
            
            // Disable the collider so no further trigger events occur
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }

            enabled = false;

            // Only destroy if the check object is null or inactive
            if (preventDestructionIfActive == null || !preventDestructionIfActive.activeInHierarchy)
            {
                Destroy(gameObject);
            }
        }
    }

    private void DeactivateExitObjects()
    {
        foreach (GameObject obj in objectsToDeactivateOnExit)
        {
            if (obj != null)
            {
                // Check if the object or its children has an active puzzle in progress
                PuzzleManager pm = obj.GetComponentInChildren<PuzzleManager>();
                if (pm != null && pm.IsPuzzleInProgress())
                {
                    Debug.Log($"[TriggerAudioAndActivate] Puzzle on '{obj.name}' is in progress. Skipping deactivation.");
                    continue;
                }

                obj.SetActive(false);
            }
        }
    }

    private IEnumerator TriggerActionSequence()
    {
        triggered = true;

        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }

        float clipLength = 0f;
        if (audioSource != null && audioClip != null)
        {
            audioSource.PlayOneShot(audioClip);
            clipLength = audioClip.length;
        }

        yield return new WaitForSeconds(clipLength);

        audioFinished = true;
        
        // Handle disabling script if player has already exited
        if (playerExited)
        {
            enabled = false;
        }
    }
}
