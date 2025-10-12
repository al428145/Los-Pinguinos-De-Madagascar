using Controller;
using System;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EnemyNoiseDetector : MonoBehaviour
{
    public event Action<Vector3> OnNoiseHeard;
    private NPCBase npcBase;

    [Header("Player Reference")]
    [SerializeField] public Transform player;
    [SerializeField] private NoiseCircle playerNoiseCircle;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 3f; // Rango del enemigo
    [SerializeField] private float detectionDelay = 0.5f;
    [SerializeField] private bool showDetectionCircle = true; // Dibuja el círculo de detección

    [Header("Visual Settings")]
    [SerializeField] private int circleSegments = 50;

    private float detectionTimer;
    private LineRenderer lineRenderer;

    [HideInInspector] public bool HasHeardNoise = false;
    [HideInInspector] public Vector3 LastNoisePosition;
    private bool detectionEnabled = true;

    private void Awake()
    {
        if (player == null)
            player = FindObjectOfType<CreatureMover>()?.transform;

        if (playerNoiseCircle == null)
            playerNoiseCircle = FindObjectOfType<NoiseCircle>();

        npcBase = GetComponent<NPCBase>();

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = false;
        lineRenderer.positionCount = circleSegments;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
    }

    private void Update()
    {
        if (!detectionEnabled || playerNoiseCircle == null) return;

        Vector3 npcCenter = transform.position;
        Vector3 playerCenter = playerNoiseCircle.transform.position;

        float combinedRadius = detectionRange + playerNoiseCircle.radius;
        float distance = Vector3.Distance(npcCenter, playerCenter);

        if (distance <= combinedRadius)
        {
            detectionTimer += Time.deltaTime;

            if (detectionTimer >= detectionDelay)
            {
                detectionTimer = 0f;

                HasHeardNoise = true;
                LastNoisePosition = playerCenter;

                if (npcBase != null)
                    npcBase.HandleNoise(playerCenter);
                else
                    Debug.LogWarning("NPCBase es null, no se puede llamar HandleNoise");

                OnNoiseHeard?.Invoke(playerCenter);
            }
        }
        else
        {
            detectionTimer = 0f;
        }

        DrawDetectionCircle(detectionRange);
    }

    private void DrawDetectionCircle(float radius)
    {
        if (!showDetectionCircle)
        {
            lineRenderer.enabled = false;
            return;
        }

        lineRenderer.enabled = true;

        float angle = 0f;
        for (int i = 0; i < circleSegments; i++)
        {
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            float z = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            lineRenderer.SetPosition(i, new Vector3(x, 0, z));
            angle += 360f / circleSegments;
        }
    }

    public void SetDetectionEnabled(bool enabled)
    {
        detectionEnabled = enabled;
        lineRenderer.enabled = enabled && showDetectionCircle;
    }
}
