using UnityEngine;
using System.Collections;

namespace OnBoarding
{
    public class GazeRaycaster : MonoBehaviour
    {
        [SerializeField] private float rayDistance = 10f;
        [SerializeField] private LayerMask interactableLayer;
        [SerializeField] private float checkInterval = 0.1f;

        private IGazeable currentTarget;
        private bool isCasting;

        private void OnEnable()
        {
            isCasting = true;
            StartCoroutine(GazeRoutine());
        }

        private void OnDisable()
        {
            isCasting = false;

            if (currentTarget != null)
            {
                currentTarget.OnGazeExit();
                currentTarget = null;
            }
        }

        private IEnumerator GazeRoutine()
        {
            WaitForSeconds wait = new WaitForSeconds(checkInterval);

            while (isCasting)
            {
                if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, rayDistance, interactableLayer))
                {
                    IGazeable gazeable = hit.collider.GetComponent<IGazeable>();

                    if (gazeable != null)
                    {
                        if (currentTarget != gazeable)
                        {
                            currentTarget?.OnGazeExit();
                            currentTarget = gazeable;
                        }
                        else
                        {
                            currentTarget.OnGazeStay();
                        }
                    }
                    else
                    {
                        ClearCurrentTarget();
                    }
                }
                else
                {
                    ClearCurrentTarget();
                }

                yield return wait;
            }
        }

        private void ClearCurrentTarget()
        {
            if (currentTarget == null)
                return;

            currentTarget.OnGazeExit();
            currentTarget = null;
        }
    }
}