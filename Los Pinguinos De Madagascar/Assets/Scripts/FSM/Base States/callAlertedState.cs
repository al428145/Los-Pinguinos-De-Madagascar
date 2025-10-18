using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class callAlertedState : State
{
    private List<Waypoint> rute;
    private List<Waypoint> allWaypoints;
    private int currentWaypointIndex;
    private Vector3 lastPositionPlayer;
    private WaypointManager wm;

    public override void Enter(NPCBase owner)
    {
        lastPositionPlayer = owner.LastHeardPosition;
        wm = Object.FindObjectOfType<WaypointManager>();
        allWaypoints = wm.GetWaypoints();
        calculateRute(owner);
    }

    public override void Execute(NPCBase owner)
    {
        if(owner.PlayerIsBeingSeen || owner.PlayerStillInRange)
        {
            owner.FSM.TriggerEvent(StateEvent.PlayerSeen);
        }

        Waypoint target = rute[currentWaypointIndex];
        owner.MoverHacia(target.position, MovementType.Walk);

        Vector3 direccionAlDestino = target.position - owner.transform.position;
        direccionAlDestino.y = 0;
        if (direccionAlDestino.sqrMagnitude < owner.distanciaMinima)
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= rute.Count)
            {
                owner.MoverHacia(lastPositionPlayer, MovementType.Walk);
            }
        }

        if(owner.transform.position == lastPositionPlayer)
        {
            owner.FSM.TriggerEvent(StateEvent.InvestigateDone);
        }
    }

    public override System.Type GetNextStateForEvent(StateEvent evt)
    {
        if (evt==StateEvent.PlayerSeen)
            return typeof(returnPatrolState);
        if (evt==StateEvent.InvestigateDone)
            return typeof(PersecuteState);
        return null;
    }

    private void calculateRute(NPCBase owner)
    {
        if(wm == null) return;
        
        Waypoint enemyWaypoint = Pathfinder.FindTheNearestWaypointEnemy(owner.transform.position, lastPositionPlayer, allWaypoints);
        Waypoint playerWaypoint = Pathfinder.FindNearestWaypointPlayer(lastPositionPlayer, allWaypoints);

        rute = Pathfinder.FindPath(enemyWaypoint, playerWaypoint);

        currentWaypointIndex = 0;
    }
}
