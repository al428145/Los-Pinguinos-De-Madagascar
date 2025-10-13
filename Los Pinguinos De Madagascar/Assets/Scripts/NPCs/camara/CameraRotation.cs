using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [Header("Configuración de rotación")]
    public float speed = 30f;         // Velocidad de rotación
    public float angle = 45f;         // Ángulo máximo hacia cada lado

    private float startY;

    void Start()
    {
        // Guardamos el ángulo inicial
        startY = transform.eulerAngles.y;
    }

    void Update()
    {
        // Creamos un movimiento de vaivén con Mathf.PingPong
        float rotation = Mathf.PingPong(Time.time * speed, angle * 2) - angle;

        // Aplicamos la rotación alrededor del eje Y (horizontal)
        transform.rotation = Quaternion.Euler(0, startY + rotation, 0);
    }
}
