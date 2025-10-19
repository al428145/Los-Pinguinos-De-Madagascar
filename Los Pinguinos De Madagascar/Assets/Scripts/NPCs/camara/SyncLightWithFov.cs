using UnityEngine;

// Asegura que este script esté en un objeto que tenga un componente Light
[RequireComponent(typeof(Light))]
public class SyncLightWithFOV : MonoBehaviour
{
    private Light spotLight;
    private SecurityCamera securityCamera;

    void Awake()
    {
        // 1. Obtener la luz en este mismo objeto
        spotLight = GetComponent<Light>();

        // 2. Obtener el script de la cámara en el objeto padre
        securityCamera = GetComponentInParent<SecurityCamera>();

        // 3. Comprobar que todo está en orden
        if (securityCamera == null)
        {
            Debug.LogError("Error en SyncLightWithFOV: No se encontró el script 'SecurityCamera' en el objeto padre.", this);
            enabled = false; // Desactivar este script si no encuentra la cámara
            return;
        }

        // 4. Asegurarse de que la luz es un Spot Light
        if (spotLight.type != LightType.Spot)
        {
            Debug.LogWarning("La luz asignada no era un Spot Light. Se ha cambiado automáticamente.", this);
            spotLight.type = LightType.Spot;
        }
    }

    // Usamos LateUpdate para asegurarnos de que se ejecuta DESPUÉS
    // de cualquier posible cambio en los valores de SecurityCamera en su Update.
    void LateUpdate()
    {
        if (securityCamera == null) return;

        // --- Sincronización ---

        // 1. Sincronizar el rango de la luz con el radio de visión
        spotLight.range = securityCamera.viewRadius;

        // 2. Sincronizar el ángulo de la luz con el ángulo de visión
        spotLight.spotAngle = securityCamera.viewAngle;

        // 3. Sincronizar la inclinación de la luz
        // Aplicamos 'tiltDown' como la rotación local en el eje X de la luz.
        transform.localRotation = Quaternion.Euler(securityCamera.tiltDown, 0, 0);
    }
}