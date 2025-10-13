using UnityEngine;

public class Gallina : NPCBase
{
    [Header("Gallina Settings")]
    public float radioAviso = 50f;
    public float tiempoAvisando = 2f;
    public LayerMask layerEnemigos;
    [Header("Efectos Visuales y sonoro")]
    public ParticleSystem particulasDormido;
    public AudioSource sonidoAlerta;
    private Renderer rend;
    public Vector3 initialPosition;

    protected override void Awake()
    {
        base.Awake();
        rend = GetComponent<Renderer>();

        initialPosition = transform.position;
        // Configurar FSM con los 3 estados
        FSM = new StateMachine(this, new System.Type[]
        {
            typeof(ChickenSleepState),
            typeof(ChickenAvisandoState),
            typeof(ChickenAlertState)
        });
        Debug.Log("Gallina Awake - FSM antes: " + FSM);
        FSM.SetState(typeof(ChickenSleepState));
        // Asegurarse de que el SecurityCamera tenga la referencia del jugador
        if (VisionDetector != null && VisionDetector.player == null)
            VisionDetector.player = player;
    }

    void Start()
    {

    }

    void Update()
    {
        FSM.Update();
    }

    public override void HandleNoise(Vector3 noisePosition)
    {
        Debug.Log("Gallina escuchó ruido");
        base.HandleNoise(noisePosition);
        FSM.TriggerEvent(StateEvent.PlayerHeard);
    }
        

    public override void HandleVision(Vector3 playerPosition)
    {
        Debug.Log("Gallina vió ruido - FSM: " + FSM);
        
        base.HandleVision(playerPosition);
        FSM.TriggerEvent(StateEvent.PlayerSeen);
    }

    public override void SelectNewDestination() { }
}
