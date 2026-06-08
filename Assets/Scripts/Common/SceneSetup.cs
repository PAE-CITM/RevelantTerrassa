using System.Collections.Generic;
using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    [Header("Activation Settings")]
    [SerializeField] private List<GameObject> objectsToActivate = new List<GameObject>();
    [SerializeField] private List<GameObject> objectsToDeactivate = new List<GameObject>();

    private void Awake()
    {
        // Activate specified GameObjects
        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }

        // Deactivate specified GameObjects
        foreach (GameObject obj in objectsToDeactivate)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }
}
