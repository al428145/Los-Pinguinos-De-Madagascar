using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [Header("Configuraci�n de rotaci�n")]
    public float speed = 30f;         // Velocidad de rotaci�n
    public float angle = 45f;         // �ngulo m�ximo hacia cada lado

    private float startY;

    void Start()
    {
        // Guardamos el �ngulo inicial
        startY = transform.eulerAngles.y;
    }

    void Update()
    {
        // Creamos un movimiento de vaiv�n con Mathf.PingPong
        float rotation = Mathf.PingPong(Time.time * speed, angle * 2) - angle;

        // Aplicamos la rotaci�n alrededor del eje Y (horizontal)
        transform.rotation = Quaternion.Euler(0, startY + rotation, 0);
    }
}
