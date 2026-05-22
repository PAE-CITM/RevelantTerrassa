using System;
using System.Collections;
using UnityEngine;

namespace OnBoarding
{
    public class NPCWaypointNavigator : MonoBehaviour
    {
        [SerializeField] private Transform npcTransform;
        [SerializeField] private Transform[] waypoints;
        [SerializeField] private float moveDuration = 1.5f;
        [SerializeField] private float waitBeforeMove = 0.5f;
        [SerializeField] private float rotationDuration = 0.5f;
        [SerializeField] private Animator animator;
        [SerializeField] private string walkParam = "isWalking";
        [SerializeField] private float[] arrivalYRotation;

        private Transform Npc => npcTransform != null ? npcTransform : transform;

        private int currentIndex;

        public event Action OnArrivedAtWaypoint;
        public event Action OnAllWaypointsCompleted;

        public void MoveToNext()
        {
            if (currentIndex < waypoints.Length - 1)
            {
                currentIndex++;
                StartCoroutine(MoveToWaypoint(waypoints[currentIndex]));
            }
        }

        public void MoveToWaypoint(int index)
        {
            if (index >= 0 && index < waypoints.Length)
            {
                currentIndex = index;
                StartCoroutine(MoveToWaypoint(waypoints[currentIndex]));
            }
        }

        private IEnumerator MoveToWaypoint(Transform target)
        {
            CharacterController charController = Npc.GetComponent<CharacterController>();
            bool originalCharControllerState = false;

            if (charController != null)
            {
                originalCharControllerState = charController.enabled;
                charController.enabled = false;
            }

            if (animator != null)
            {
                animator.SetBool(walkParam, true);
            }
            
            yield return new WaitForSeconds(waitBeforeMove);

            Vector3 start = Npc.position;
            Vector3 end = target.position;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / moveDuration;
                Npc.position = Vector3.Lerp(start, end, t);
                yield return null;
            }

            Npc.position = end;

            if (animator != null)
            {
                animator.SetBool(walkParam, false);
            }

            if (charController != null)
                charController.enabled = originalCharControllerState;

            Quaternion startRot = Npc.rotation;
            Quaternion endRot = target.rotation;

            if (arrivalYRotation != null && currentIndex < arrivalYRotation.Length && arrivalYRotation[currentIndex] != 0f)
            {
                endRot = endRot * Quaternion.Euler(0f, arrivalYRotation[currentIndex], 0f);
            }

            float rotT = 0f;
            while (rotT < 1f)
            {
                rotT += Time.deltaTime / rotationDuration;
                Npc.rotation = Quaternion.Slerp(startRot, endRot, rotT);
                yield return null;
            }
            Npc.rotation = endRot;

            OnArrivedAtWaypoint?.Invoke();

            if (currentIndex >= waypoints.Length - 1)
                OnAllWaypointsCompleted?.Invoke();
        }

        public void SnapToWaypoint(int index)
        {
            if (index >= 0 && index < waypoints.Length)
            {
                currentIndex = index;
                Npc.position = waypoints[index].position;
                Npc.rotation = waypoints[index].rotation;
            }
        }

        public int CurrentIndex => currentIndex;
    }
}
