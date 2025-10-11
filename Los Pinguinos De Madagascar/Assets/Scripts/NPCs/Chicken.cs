using UnityEngine;

public class Gallina : NPCBase
{
    [Header("Gallina Settings")]
    public float radioAviso = 5f;
    public float tiempoAvisando = 2f;
    public LayerMask layerEnemigos;

    private Renderer rend;

    protected override void Awake()
    {
        base.Awake();
        rend = GetComponent<Renderer>();

        // Configurar FSM con los 3 estados
        FSM = new StateMachine(this, new System.Type[]
        {
            typeof(ChickenSleepState),
            typeof(ChickenAvisandoState),
            typeof(ChickenAlertState)
        });
    }

    void Start()
    {
        FSM.SetState(typeof(ChickenSleepState));
    }

    void Update()
    {
        FSM.Update();
    }

    public override void HandleNoise(Vector3 noisePosition)
    {
        base.HandleNoise(noisePosition);
        FSM.TriggerEvent(StateEvent.PlayerHeard);
    }

    public override void HandleVision(Vector3 playerPosition)
    {
        base.HandleVision(playerPosition);
        FSM.TriggerEvent(StateEvent.PlayerSeen);
    }

    public override void SelectNewDestination() { }
}
