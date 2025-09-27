using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    private List<Waypoint> waypoints = new List<Waypoint>();

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
                    // Raycast para comprobar visibilidad
                    if (!Physics.Raycast(wp.transform.position, 
                                         (other.transform.position - wp.transform.position).normalized, 
                                         dist))
                    {
                        if (!wp.neighbors.Contains(other))
                            wp.neighbors.Add(other);
                    }
                }
            }
        }
    }

    public List<Waypoint> GetWaypoints()
    {
        return waypoints;
    }
}
