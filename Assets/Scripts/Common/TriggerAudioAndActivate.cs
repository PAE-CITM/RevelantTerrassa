using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TriggerAudioAndActivate : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private List<GameObject> objectsToActivate = new List<GameObject>();

    private bool triggered = false;

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

    private IEnumerator TriggerActionSequence()
    {
        triggered = true;

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Destroy(col);
        }

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

        Destroy(gameObject);
    }
}
