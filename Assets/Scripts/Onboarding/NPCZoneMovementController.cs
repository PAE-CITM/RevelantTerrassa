using UnityEngine;

namespace OnBoarding
{
    public class NPCZoneMovementController : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("El componente NPCWaypointNavigator asignado al niño (nen).")]
        [SerializeField] private NPCWaypointNavigator npcNavigator;

        [Header("Configuración de Zonas")]
        [Tooltip("Asigna las 3 zonas (Colliders configurados como triggers) en orden. La Zona 0 moverá al NPC al Waypoint 0, la Zona 1 al Waypoint 1, etc.")]
        [SerializeField] private Collider[] zoneColliders = new Collider[3];

        private bool[] zoneTriggered = new bool[3];

        private void Awake()
        {
            // Inicializar el estado de los triggers
            if (zoneTriggered.Length != zoneColliders.Length)
            {
                zoneTriggered = new bool[zoneColliders.Length];
            }

            // Registrar dinámicamente los listeners para cada una de las zonas
            for (int i = 0; i < zoneColliders.Length; i++)
            {
                if (zoneColliders[i] != null)
                {
                    int index = i; // Capturar el índice de la zona para el delegado closure

                    // Asegurarnos de que el collider esté configurado como Trigger
                    zoneColliders[i].isTrigger = true;

                    // Obtener o añadir el componente auxiliar TriggerListener al GameObject de la zona
                    TriggerListener listener = zoneColliders[i].gameObject.GetComponent<TriggerListener>();
                    if (listener == null)
                    {
                        listener = zoneColliders[i].gameObject.AddComponent<TriggerListener>();
                    }

                    listener.onTriggerEntered += (Collider other) => OnPlayerEnterZone(index, other);
                }
            }
        }

        private void OnPlayerEnterZone(int zoneIndex, Collider other)
        {
            // Comprobar que sea el jugador el que entra
            if (other.CompareTag("Player"))
            {
                Debug.Log($"[NPCZoneMovementController] El jugador entró en la Zona {zoneIndex}. Moviendo NPC al Waypoint {zoneIndex}.");

                if (npcNavigator != null)
                {
                    // Mover el NPC al waypoint correspondiente utilizando su índice
                    npcNavigator.MoveToWaypoint(zoneIndex);
                }
            }
        }
    }
}
