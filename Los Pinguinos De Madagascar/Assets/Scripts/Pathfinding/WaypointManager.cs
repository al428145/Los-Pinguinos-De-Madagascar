using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    private List<Waypoint> waypoints = new List<Waypoint>();

    [Header("Layers block mask")]
    public LayerMask obstacleMask;

    [Header("Heigh of the raycast")]
    public float rayOffsetY = 0.25f;

    void Start()
    {
        // 1. Encuentra todos los waypoints en la escena
        waypoints.AddRange(FindObjectsOfType<Waypoint>());

        // 2. Conecta cada waypoint con los que tenga cerca
        foreach (Waypoint wp in waypoints)
        {
            foreach (Waypoint other in waypoints)
            {
                if (wp == other) continue;

                float dist = Vector3.Distance(wp.transform.position, other.transform.position);

                if (dist <= wp.connectionRadius)
                {
                    Vector3 origin = wp.transform.position + Vector3.up * rayOffsetY;
                    Vector3 target = other.transform.position + Vector3.up * rayOffsetY;
                    Vector3 dir = (target-origin).normalized;

                    Debug.DrawLine(origin, target, Color.red, 3f);

                    // Raycast para comprobar visibilidad
                    if (!Physics.Linecast(origin, target, obstacleMask))
                    {
                        if (!wp.neighbors.Contains(other))
                            wp.neighbors.Add(other);
                    }
                }
            }
        }

        Debug.Log($"WaypointManager: {waypoints.Count} waypoints conectados.");
    }

    public List<Waypoint> GetWaypoints()
    {
        return waypoints;
    }
}
