//using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public enum MovementType
{
    Idle,
    Walk,
    Run
}

public class MovementSettings
{
    public float idle = 0f;
    public float walk = 2f;
    public float run = 3f;

    public float GetSpeed(MovementType type)
    {
        switch (type)
        {
            case MovementType.Walk: return walk;
            case MovementType.Run: return run;
            default: return idle;
        }
    }
}

public abstract class NPCBase : MonoBehaviour
{
    [Header("Parámetros de Movimiento")]

    public MovementSettings speeds = new MovementSettings();

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
        float velocidad = speeds.GetSpeed(MovementType.Walk);

        Vector3 direccion = objetivo - transform.position;
        direccion.y = 0f;
        float distancia = direccion.magnitude;
        direccion.Normalize();

        float currentSpeed = speeds.GetSpeed(MovementType.Idle);

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

    public virtual void HandleNoise(Vector3 noisePosition){}

    public virtual void HandleVision(Vector3 playerPosition){}

    protected virtual void LookAtNoise(Vector3 noisePosition)
    {
        Vector3 direccion = noisePosition - transform.position;
        direccion.y = 0f;
        if (direccion.sqrMagnitude > 0.1f)
        {
            Quaternion rotObjeto = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotObjeto, velocidadRotacion * Time.deltaTime);
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", speeds.GetSpeed(MovementType.Idle));
        }

    }

}
