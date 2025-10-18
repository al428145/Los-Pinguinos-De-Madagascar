using UnityEngine;

public class SecurityCamNPC : NPCBase
{
    [Header("Security Camera Settings")]
    public float alertDuration = 5f;
    
    [Header("Aviso a enemigos")]
    public float alertRadius = 50f;
    [Header("Aviso a enemigos")]
    public AudioSource alarmSound;
    [Range(0f, 1f)] public float alarmVolume = 0.1f;


    protected override void Awake()
    {
        base.Awake();

        // Configurar FSM con los 2 estados: vigilancia y aviso
        FSM = new StateMachine(this, new System.Type[]
        {
            typeof(CameraVigilando),
            typeof(CamaraWarning)
        });

        // Estado inicial: vigilando
        FSM.SetState(typeof(CameraVigilando));

        // Vincular el player si no está asignado
        if (VisionDetector != null && VisionDetector.player == null)
            VisionDetector.player = player;

        alarmSound.volume = alarmVolume;
    }

    void Update()
    {
        FSM.Update();
    }

    public override void HandleVision(Vector3 playerPosition)
    {
        LastHeardPosition = playerPosition;
        base.HandleVision(playerPosition);
        FSM.TriggerEvent(StateEvent.PlayerSeen);
    }

    public override void SelectNewDestination() { }
}
