using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RandomAudioOnApproach : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> audioClips = new List<AudioClip>();
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float cooldownTime = 3.5f;

    private List<AudioClip> availableClips = new List<AudioClip>();
    private AudioClip lastPlayed;
    private AudioClip secondLastPlayed;
    private float lastTriggerTime;

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        ResetAvailableClips();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (audioSource != null && audioSource.isPlaying) return;

            PlayRandomAudio();
            lastTriggerTime = Time.time;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (audioSource != null && audioSource.isPlaying) return;

            if (Time.time >= lastTriggerTime + cooldownTime)
            {
                PlayRandomAudio();
                lastTriggerTime = Time.time;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            lastTriggerTime = 0f;
        }
    }

    private void PlayRandomAudio()
    {
        if (audioClips.Count == 0) return;

        if (availableClips.Count == 0)
        {
            ResetAvailableClips();
        }

        List<AudioClip> candidates = new List<AudioClip>();
        foreach (AudioClip clip in availableClips)
        {
            if (clip != lastPlayed && clip != secondLastPlayed)
            {
                candidates.Add(clip);
            }
        }

        if (candidates.Count == 0)
        {
            foreach (AudioClip clip in availableClips)
            {
                if (clip != lastPlayed)
                {
                    candidates.Add(clip);
                }
            }
        }

        if (candidates.Count == 0)
        {
            candidates.AddRange(availableClips);
        }

        AudioClip chosenClip = candidates[Random.Range(0, candidates.Count)];
        
        PlayClip(chosenClip);

        secondLastPlayed = lastPlayed;
        lastPlayed = chosenClip;
        availableClips.Remove(chosenClip);
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }

    private void ResetAvailableClips()
    {
        availableClips = new List<AudioClip>(audioClips);
    }
}
