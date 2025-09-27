using UnityEngine;

public abstract class NPCBase : MonoBehaviour
{
    [Header("Parámetros de Movimiento")]

    [SerializeField, Range(0, 3f)]
    private float velocidad = 3f;

    [SerializeField, Range(0, 5f)]
    private float rangoDeteccion = 5f;
    public float distanciaMinima = 0.5f;
    
    [SerializeField, Range(0, 5f)]
    public float velocidadRotacion = 5f;

    protected Vector3 destinoActual;

    // Animación
    protected Animator animator;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
    }

    protected virtual void MoverHacia(Vector3 objetivo)
    {
        Vector3 direccion = objetivo - transform.position;
        direccion.y = 0f;
        float distancia = direccion.magnitude;
        direccion.Normalize();

        float currentSpeed = 0f;

        if (distancia > distanciaMinima)
        {
            transform.position += direccion * velocidad * Time.deltaTime;

            Quaternion rotObjetivo = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotObjetivo, velocidadRotacion * Time.deltaTime);

            currentSpeed = velocidad; // usamos la velocidad real
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", currentSpeed);
        }
    }

}
