using Controller;
using UnityEngine;

public abstract class NPCBase : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadRotacion = 5f;
    public float distanciaMinima = 0.5f;
    public MovementSettings speeds = new MovementSettings();

    [Header("FSM")]
    public StateMachine FSM;

    [HideInInspector]
    public EnemyNoiseDetector NoiseDetector;

    [HideInInspector]
    public SecurityCamera VisionDetector;

    [HideInInspector] public Vector3 CurrentDestination;
    [HideInInspector] public Vector3 LastHeardPosition;
    [HideInInspector] public bool PlayerStillInRange;
    [HideInInspector] public bool PlayerIsBeingSeen;
    [HideInInspector] public Transform player;

    protected Animator animator;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        NoiseDetector = GetComponent<EnemyNoiseDetector>();
        VisionDetector = GetComponent<SecurityCamera>();
        CreatureMover playerScript = FindObjectOfType<CreatureMover>();
        if (playerScript != null)
        {
            player = playerScript.transform;
        }
    }

    public virtual void MoverHacia(Vector3 objetivo, MovementType moveType = MovementType.Walk)
    {
        float velocidadDeseada = speeds.GetSpeed(moveType);

        Vector3 direccion = objetivo - transform.position;
        direccion.y = 0f;
        float distancia = direccion.magnitude;
        direccion.Normalize();

        float currentSpeed = speeds.idle;

        if (distancia > distanciaMinima)
        {
            transform.position += direccion * velocidadDeseada * Time.deltaTime;
            Quaternion rotObjetivo = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotObjetivo, velocidadRotacion * Time.deltaTime);
            currentSpeed = velocidadDeseada;
        }

        animator?.SetFloat("Speed", currentSpeed);
    }

    public virtual void LookAtNoise(Vector3 noisePos)
    {
        Vector3 dir = noisePos - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.1f)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, velocidadRotacion * Time.deltaTime);
        }
        animator?.SetFloat("Speed", speeds.idle);
    }

    public virtual void HandleNoise(Vector3 noisePosition)
    {
        LastHeardPosition = noisePosition;
        PlayerStillInRange = true;
        FSM?.TriggerEvent(StateEvent.NoiseHeard);
    }

    public virtual void HandleVision(Vector3 playerPosition)
    {
        LastHeardPosition = playerPosition;
        PlayerIsBeingSeen = true;
        FSM?.TriggerEvent(StateEvent.PlayerSeen);
    }

    // Método abstracto que cada NPC implementa distinto
    public abstract void SelectNewDestination();
}
