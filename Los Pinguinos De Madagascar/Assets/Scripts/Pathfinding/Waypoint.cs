using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [HideInInspector] public List<Waypoint> neighbors = new List<Waypoint>();

    // Radio máximo para buscar vecinos (puedes ajustarlo en el inspector)
    public float connectionRadius = 10f;

    void OnDrawGizmos()
    {
        // Dibuja el nodo en la escena (esfera amarilla)
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.3f);

        // Dibuja conexiones con vecinos (líneas azules)
        Gizmos.color = Color.cyan;
        foreach (var neighbor in neighbors)
        {
            if (neighbor != null)
                Gizmos.DrawLine(transform.position, neighbor.transform.position);
        }
    }
}
