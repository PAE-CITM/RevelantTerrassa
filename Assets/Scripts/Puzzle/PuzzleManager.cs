using UnityEngine;
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

    // UnityEvent for rigging within the Inspector
    public UnityEvent OnPuzzleCompleted;
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
        if (!puzzleFinished)
        {
            musicSource.clip = puzzleMusicClip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    private void OnDisable()
    {
        musicSource.Stop();
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
        musicSource.Stop();
        audioSource.PlayOneShot(puzzleCompletedClip);

        OnPuzzleCompleted?.Invoke();
    }
}
