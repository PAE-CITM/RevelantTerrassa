using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

public class PuzzleManager : MonoBehaviour
{
    private List<ConnectionNode> sockets = new List<ConnectionNode>();
    private bool puzzleFinished = false;

    public AudioSource audioSource;
    public AudioClip puzzleCompletedClip;

    [Header("Puzzle Music Settings")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip puzzleMusicClip;
    [SerializeField] private float fadeDuration = 1.5f;

    private float maxMusicVolume = 1f;
    private Coroutine fadeCoroutine;

    // UnityEvent for rigging within the Inspector
    public UnityEvent OnPuzzleCompleted;

    [Header("Puzzle Completion Effects")]
    [SerializeField] private ParticleSystem completionParticles;
    [SerializeField] private float completionAudioDelay = 2.0f;

    private void Awake()
    {
        if (musicSource != null)
        {
            maxMusicVolume = musicSource.volume;
        }
    }

    void Start()
    {
        ConnectionNode[] foundNodes = GetComponentsInChildren<ConnectionNode>();
        
        foreach (var node in foundNodes)
        {
            sockets.Add(node);
        }
    }

    private void OnEnable()
    {
        if (!puzzleFinished && musicSource != null)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            musicSource.clip = puzzleMusicClip;
            musicSource.loop = true;
            musicSource.volume = 0f;
            musicSource.Play();
            fadeCoroutine = StartCoroutine(FadeMusic(maxMusicVolume, fadeDuration));
        }
    }

    private void OnDisable()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
            musicSource.volume = maxMusicVolume;
        }
    }

    public void CheckCompletion()
    {
        if (puzzleFinished) return;

        int matchesFound = 0;
        foreach (var socket in sockets)
        {
            if (socket.isMatched) matchesFound++;
        }

        if (matchesFound >= sockets.Count && sockets.Count > 0)
        {
            puzzleFinished = true;
            WinGame();
        }
    }

    void WinGame()
    {
        if (musicSource != null)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeMusic(0f, fadeDuration, true));
        }

        if (audioSource != null && puzzleCompletedClip != null)
        {
            StartCoroutine(PlayCompletionAudioDelayed(completionAudioDelay));
        }

        if (completionParticles != null)
        {
            completionParticles.Play();
        }

        OnPuzzleCompleted?.Invoke();
    }

    private IEnumerator PlayCompletionAudioDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (audioSource != null && puzzleCompletedClip != null)
        {
            audioSource.PlayOneShot(puzzleCompletedClip);
        }
    }

    private IEnumerator FadeMusic(float targetVolume, float duration, bool stopOnComplete = false)
    {
        if (musicSource == null) yield break;

        float startVolume = musicSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
            yield return null;
        }

        musicSource.volume = targetVolume;

        if (stopOnComplete)
        {
            musicSource.Stop();
        }
    }
}
