using UnityEngine;

public class Audioonaproch : MonoBehaviour
{
    private bool onetime = true;

    [SerializeField] private narratorAudio narrator;
    public AudioClip narratorAudio;

    private void OnTriggerEnter(Collider other)
    {
        if (onetime == true && other.CompareTag("Player"))
        {
            narrator.PlayClip(narratorAudio);
            onetime = false;

            Collider[] colliders = GetComponents<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = false;
            }
        }
    }
}
