using UnityEngine;

public class ConnectionNode : MonoBehaviour
{
    public string nodeID;
    public string targetID;
    public bool isMatched = false;
    public ConnectionNode targetNode;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ConnectionNode>(out ConnectionNode otherNode))
        {
            if (otherNode.nodeID == targetID)
            {
                isMatched = true;
                targetNode = otherNode;
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
                targetNode = null;
            }
        }
    }
}
