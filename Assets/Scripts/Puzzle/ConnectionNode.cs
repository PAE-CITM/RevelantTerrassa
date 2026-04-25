using UnityEngine;

public class ConnectionNode : MonoBehaviour
{
    public string nodeID;
    public string targetID;
    public bool isMatched = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ConnectionNode>(out ConnectionNode otherNode))
        {
            if (otherNode.nodeID == targetID)
            {
                isMatched = true;
                Debug.Log($"<color=cyan>{nodeID} ha trobat el seu encaix!</color>");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<ConnectionNode>(out ConnectionNode otherNode))
        {
            if (otherNode.nodeID == targetID)
            {
                isMatched = false;
            }
        }
    }
}
