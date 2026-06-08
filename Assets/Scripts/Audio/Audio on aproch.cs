using UnityEngine;

public class Audioonaproch : MonoBehaviour
{
    private bool onetime = true;

    [SerializeField] private narratorAudio narrator;
    public AudioClip narratorAudio;

    private void Start()
    {
        if (narrator == null)
        {
            narrator = FindFirstObjectByType<narratorAudio>();
            if (narrator == null)
            {
                Debug.LogError("narratorAudio not found in the scene! Please assign it in the inspector.", this);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (onetime == true && other.CompareTag("Player"))
        {
            if (narrator != null)
            {
                narrator.PlayClip(narratorAudio);
            }
            onetime = false;

            Collider[] colliders = GetComponents<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = false;
            }
        }
    }
}
