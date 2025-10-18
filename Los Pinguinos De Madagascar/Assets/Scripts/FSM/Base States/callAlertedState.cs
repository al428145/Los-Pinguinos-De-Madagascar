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
            owner.FSM.TriggerEvent(StateEvent.playerFindInRute);
            return;
        }

        if(rute.Count > 0)
        {
            Waypoint target = rute[currentWaypointIndex];
            owner.MoverHacia(target.position, MovementType.Walk);

            Vector3 direccionAlDestino = target.position - owner.transform.position;
            direccionAlDestino.y = 0;
            if (direccionAlDestino.sqrMagnitude < owner.distanciaMinima)
            {
                currentWaypointIndex++;

                if (currentWaypointIndex >= rute.Count)
                {
                    rute = new List<Waypoint>();
                }
            }

            if(rute == null || rute.Count == 0)
            {
                Vector3 dirToPlayer = (lastPositionPlayer - owner.transform.position).normalized;
                owner.transform.position += dirToPlayer * owner.currentSpeed * Time.deltaTime;

                if (dirToPlayer != Vector3.zero)
                    owner.transform.forward = Vector3.Lerp(owner.transform.forward, dirToPlayer, Time.deltaTime * 5f);
            }

            Vector3 direccion = owner.transform.position - lastPositionPlayer;
            direccion.y = 0;
            if(direccion.sqrMagnitude < owner.distanciaMinima)
            {
                owner.FSM.TriggerEvent(StateEvent.investigationFinished);
            }
        }
        
    }

    public override System.Type GetNextStateForEvent(StateEvent evt)
    {
        if (evt==StateEvent.playerFindInRute)
            return typeof(PersecuteState);
        if (evt==StateEvent.investigationFinished)
            return typeof(returnPatrolState);
        return null;
    }

    private void calculateRute(NPCBase owner)
    {
        if(wm == null) return;
        
        Waypoint enemyWaypoint = Pathfinder.FindTheNearestWaypointEnemy(owner.transform.position, lastPositionPlayer, allWaypoints);
        Waypoint playerWaypoint = Pathfinder.FindNearestWaypointPlayer(lastPositionPlayer, allWaypoints);

        rute = Pathfinder.FindPath(enemyWaypoint, playerWaypoint);

        currentWaypointIndex = 0;
        Debug.Log(rute.Count);
    }
}
