using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public int totalNodesNecessary; 
    private bool puzzleFinished = false;

    void Start()
    {
        ConnectionNode[] allNodesInScene = FindObjectsOfType<ConnectionNode>();
        totalNodesNecessary = allNodesInScene.Length;
        Debug.Log($"Nodes totals per guanyar: {totalNodesNecessary}");
    }

    void Update()
    {
        if (puzzleFinished) return;

        GameObject puzzleParent = GameObject.Find("Puzzle_Completat");

        if (puzzleParent != null)
        {
            ConnectionNode[] completedNodes = puzzleParent.GetComponentsInChildren<ConnectionNode>();

            int matchesFound = 0;
            foreach (var node in completedNodes)
            {
                if (node.isMatched) matchesFound++;
            }

            if (matchesFound >= totalNodesNecessary && totalNodesNecessary > 0)
            {
                puzzleFinished = true;
                WinGame();
            }
        }
    }

    void WinGame()
    {
        Debug.Log("<color=green><b>PUZZLE ACABAT REALMENT!</b></color>");
    }
}