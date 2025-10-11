using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [HideInInspector] public List<Waypoint> neighbors = new List<Waypoint>();

    [HideInInspector] public Vector3 position;

    // Radio maximo para buscar vecinos (puedes ajustarlo en el inspector)
    public float connectionRadius = 20f;

    void Start()
    {
        position = transform.position - transform.parent.position;
    }

    void OnDrawGizmos()
    {
        // Dibuja el nodo en la escena (esfera amarilla)
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.3f);
        
        //Draw the radius
        Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, connectionRadius);

        // Dibuja conexiones con vecinos (lineas azules)
        Gizmos.color = Color.cyan;
        foreach (var neighbor in neighbors)
        {
            if (neighbor != null)
                Gizmos.DrawLine(transform.position, neighbor.transform.position);
        }
    }
}
