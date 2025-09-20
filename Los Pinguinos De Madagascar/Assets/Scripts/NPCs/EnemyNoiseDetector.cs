using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EnemyNoiseDetector : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private Transform player;
    [SerializeField] private NoiseCircle playerNoiseCircle;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 3f; // Rango ajustable del enemigo
    [SerializeField] private float detectionDelay = 0.5f;
    [SerializeField] private bool showDetectionCircle = true; // Controla si se dibuja la circunferencia

    [Header("Visual Settings")]
    [SerializeField] private int circleSegments = 50;

    private float detectionTimer;
    private LineRenderer lineRenderer;

    private void Awake()
    {
        // Configuramos LineRenderer
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = false;
        lineRenderer.positionCount = circleSegments;
        
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
    }

    private void Update()
    {
        if (playerNoiseCircle == null) return;

        Vector3 npcCenter = transform.position;
        Vector3 playerCenter = playerNoiseCircle.transform.position;

        float combinedRadius = detectionRange + playerNoiseCircle.radius;
        float distance = Vector3.Distance(npcCenter, playerCenter);

        // DEBUG: imprimir posiciones y distancia
        Debug.Log($"NPC Pos: {npcCenter}, Player Pos: {playerCenter}, Distance: {distance}, Combined Radius: {combinedRadius}");

        if (distance <= combinedRadius)
        {
            detectionTimer += Time.deltaTime;
            if (detectionTimer >= detectionDelay)
            {
                Debug.Log("¡Jugador detectado por ruido!");
            }
        }
        else
        {
            detectionTimer = 0f;
        }

        // Visualización del círculo del NPC
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
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) *( radius);
            float z = Mathf.Sin(Mathf.Deg2Rad * angle) * (radius );
            lineRenderer.SetPosition(i, new Vector3(x, 0, z));
            angle += 360f / circleSegments;
        }
    }
}
