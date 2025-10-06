using UnityEngine;
using System;

public class NPCDetector : MonoBehaviour
{
    private NPCBase npcBase;

    [Header("Configuraci�n del campo de visi�n")]
    public float viewRadius = 10f;        // Distancia m�xima de visi�n
    [Range(0, 360)]
    public float viewAngle = 90f;         // �ngulo de visi�n (ej: 90�)

    [Header("Referencia del jugador")]
    public Transform player;              // Asigna el jugador en el inspector

    [Header("Opciones de detecci�n")]
    public LayerMask targetMask;          // Capa del jugador
    public LayerMask obstructionMask;     // Capa de obst�culos (paredes, etc.)

    [Header("Debug")]
    public bool canSeePlayer;

    void Awake()
    {
        npcBase = GetComponent<NPCBase>();
    }

    void Update()
    {
        DetectPlayer();
    }

    void DetectPlayer()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float distToPlayer = Vector3.Distance(transform.position, player.position);

        // Verificamos si el jugador est� dentro del �ngulo de visi�n
        if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
        {
            if (distToPlayer < viewRadius)
            {
                // Raycast para comprobar obst�culos
                if (!Physics.Raycast(transform.position, dirToPlayer, distToPlayer, obstructionMask))
                {
                    canSeePlayer = true;
                    npcBase.HandleVision(player.position);
                    return;
                }
            }
        }

        canSeePlayer = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 angleA = DirFromAngle(-viewAngle / 2);
        Vector3 angleB = DirFromAngle(viewAngle / 2);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + angleA * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + angleB * viewRadius);
    }

    Vector3 DirFromAngle(float angleInDegrees)
    {
        Quaternion rotation = Quaternion.Euler(0, angleInDegrees + transform.eulerAngles.y, 0);
        return rotation * Vector3.forward;
    }
}
