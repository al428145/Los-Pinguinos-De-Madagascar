using System.Collections.Generic;
using UnityEngine;

public class Guard : NPCBase
{
    [Header("Puntos de patrulla")]
    public List<Waypoint> puntosDePatrulla;
    private int indiceActual = 0;

    protected override void Awake()
    {
        base.Awake();
        FSM = new StateMachine(this, new System.Type[]
        {
            typeof(PatrolState),
            typeof(AlertedState),
            typeof(InvestigateState),
            typeof(PersecuteState),
            typeof(returnPatrolState)
        });
    }

    void Start()
    {
        SelectNewDestination();
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
        if (puntosDePatrulla.Count == 0) return;

        CurrentDestination = puntosDePatrulla[indiceActual].transform.position;
        indiceActual = (indiceActual + 1) % puntosDePatrulla.Count;
    }
}
