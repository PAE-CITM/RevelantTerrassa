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
        
        Debug.Log($"[PuzzleManager] {gameObject.name} inicialitzat amb {sockets.Count} sockets.");
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

        Debug.Log($"[PuzzleManager] Progrés: {matchesFound}/{sockets.Count}");

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

        Debug.Log($"<color=green><b>PUZZLE {gameObject.name} ACABAT!</b></color>");
        OnPuzzleCompleted?.Invoke();
    }
}
