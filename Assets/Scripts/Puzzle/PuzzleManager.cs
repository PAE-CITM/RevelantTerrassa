using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

public class PuzzleManager : MonoBehaviour
{
    private List<ConnectionNode> sockets = new List<ConnectionNode>();
    private bool puzzleFinished = false;

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
        Debug.Log($"<color=green><b>PUZZLE {gameObject.name} ACABAT!</b></color>");
        OnPuzzleCompleted?.Invoke();
    }
}
