using UnityEngine;

public class Guard : NPCBase
{
    [Header("Puntos de patrulla")]
    public Transform[] puntosDePatrulla;
    private int indiceActual = 0;

    protected override void Awake()
    {
        base.Awake();
        FSM = new StateMachine(this, new System.Type[]
        {
            typeof(PatrolState),
            typeof(AlertedState),
            typeof(InvestigateState),
            typeof(PersecuteState)
        });
    }

    void Start()
    {
        FSM.SetState(typeof(PatrolState));
    }

    void Update()
    {
        FSM.Update();
        PlayerStillInRange = false;
        PlayerIsBeingSeen = false;
    }

    public override void SelectNewDestination()
    {
        if (puntosDePatrulla.Length == 0) return;

        CurrentDestination = puntosDePatrulla[indiceActual].position;
        indiceActual = (indiceActual + 1) % puntosDePatrulla.Length;
    }
}
