using UnityEngine;
using System;
using Controller;

public class SecurityCamera : MonoBehaviour
{
    private NPCBase npcBase;

    [Header("Configuración del campo de visión")]
    public float viewRadius = 10f;        // Distancia máxima de visión
    [Range(0, 360)]
    public float viewAngle = 90f;         // Ángulo de visión
    [Range(-89f, 89f)]
    public float tiltDown = 20f;          // Inclinación hacia abajo

    [Header("Referencia del jugador")]
    public Transform player;              // Asigna el jugador en el inspector

    [Header("Opciones de detección")]
    public LayerMask targetMask;          // Capa del jugador
    public LayerMask obstructionMask;     // Capa de obstáculos

    [Header("Debug")]
    public bool canSeePlayer;

    [HideInInspector] public bool HasSeenPlayer = false;
    [HideInInspector] public Vector3 LastSeenPosition;

    private bool detectionEnabled = true;

    void Awake()
    {
        npcBase = GetComponent<NPCBase>();

        if (player == null)
        {
            var playerObj = FindObjectOfType<CreatureMover>();
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    void Update()
    {
        if (!detectionEnabled || player == null)
            return;

        DetectPlayer();
    }

    void DetectPlayer()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float distToPlayer = Vector3.Distance(transform.position, player.position);

        Vector3 forwardTilted = Quaternion.Euler(tiltDown, 0, 0) * transform.forward;

        if (Vector3.Angle(forwardTilted, dirToPlayer) < viewAngle / 2)
        {
            if (distToPlayer < viewRadius)
            {
                if (!Physics.Raycast(transform.position, dirToPlayer, distToPlayer, obstructionMask))
                {
                    canSeePlayer = true;

                    HasSeenPlayer = true;
                    LastSeenPosition = player.position;

                    npcBase?.HandleVision(player.position);

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
        Quaternion rotation = Quaternion.Euler(tiltDown, angleInDegrees + transform.eulerAngles.y, 0);
        return rotation * Vector3.forward;
    }

    public void SetDetectionEnabled(bool enabled)
    {
        detectionEnabled = enabled;
        canSeePlayer = false;
    }
}
