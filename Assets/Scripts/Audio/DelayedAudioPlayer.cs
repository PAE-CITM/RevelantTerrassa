using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DelayedAudioPlayer : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private float delaySeconds = 2.5f;

    private void Start()
    {
        // Fallback to local AudioSource if not assigned
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (audioClip != null && audioSource != null)
        {
            // Method 1: Using Unity's built-in PlayDelayed
            audioSource.clip = audioClip;
            audioSource.PlayDelayed(delaySeconds);
        }
        else
        {
            Debug.LogWarning($"DelayedAudioPlayer on {gameObject.name}: Missing AudioClip or AudioSource reference.", this);
        }
    }
}
