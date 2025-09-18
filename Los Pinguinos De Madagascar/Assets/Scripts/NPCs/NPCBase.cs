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
        // Dirección en plano XZ
        Vector3 direccion = (objetivo - transform.position);
        direccion.y = 0f; // no mirar arriba/abajo
        direccion.Normalize();

        if (direccion.sqrMagnitude > 0.01f)
        {
            // Movimiento
            transform.position += direccion * velocidad * Time.deltaTime;

            // Rotación suave hacia el objetivo
            Quaternion rotObjetivo = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotObjetivo, velocidadRotacion * Time.deltaTime);
        }

        // Animación de caminar
        if (animator != null)
        {
            float speedValue = direccion.magnitude * velocidad; 
            //animator.SetFloat("Speed", speedValue); 
            // 👉 Debes tener en el Animator un parámetro float "Speed"
        }
    }
}
