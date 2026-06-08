using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class narratorAudio : MonoBehaviour
{
    
    public AudioSource audioSource;

    public AudioClip firstClip;

    [SerializeField] private float firstClipDelay = 10f;

    [Header("Idle Random Audios")]
    [SerializeField] private List<AudioClip> idleClips = new List<AudioClip>();
    [SerializeField] private float minIntervalSeconds = 180f; 
    [SerializeField] private float maxIntervalSeconds = 240f; 
    [SerializeField] private float checkStatusInterval = 2f; 

    private List<AudioClip> remainingIdleClips;
    private Coroutine idleAudioCoroutine;

    private void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        remainingIdleClips = new List<AudioClip>(idleClips);
        if (remainingIdleClips.Count > 0)
        {
            idleAudioCoroutine = StartCoroutine(IdleAudioSequence());
        }
    }

   
    public void PlayClip(AudioClip clip)
    {
        if (clip == null) return;
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
        
    }

    private IEnumerator IdleAudioSequence()
    {
        while (remainingIdleClips.Count > 0)
        {
            float waitTime = Random.Range(minIntervalSeconds, maxIntervalSeconds);
            yield return new WaitForSeconds(waitTime);

            while (IsNarratorPlaying() || IsAnyPuzzleInProgress())
            {
                yield return new WaitForSeconds(checkStatusInterval);
            }

            if (remainingIdleClips.Count == 0)
                break;

            int randomIndex = Random.Range(0, remainingIdleClips.Count);
            AudioClip chosenClip = remainingIdleClips[randomIndex];

            PlayClip(chosenClip);

            remainingIdleClips.RemoveAt(randomIndex);
        }
    }

    private bool IsNarratorPlaying()
    {
        return audioSource != null && audioSource.isPlaying;
    }

    private bool IsAnyPuzzleInProgress()
    {
        PuzzleManager[] puzzles = FindObjectsByType<PuzzleManager>(FindObjectsSortMode.None);
        foreach (var pm in puzzles)
        {
            if (pm != null && pm.gameObject.activeInHierarchy && pm.IsPuzzleInProgress())
            {
                return true;
            }
        }
        return false;
    }
}