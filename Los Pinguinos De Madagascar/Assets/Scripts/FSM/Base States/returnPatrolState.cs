using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class returnPatrolState : State
{
    private List<Waypoint> rute;
    private int currentWaypointIndex;
    private WaypointManager wm;    
    private List<Waypoint> patrolPoints;
    private List<Waypoint> allWaypoints;
    private Waypoint patrolWaypoint;

    public override void Enter(NPCBase owner)
    {
        rute = new List<Waypoint>();
        currentWaypointIndex = 0;
        wm = Object.FindObjectOfType<WaypointManager>();
        allWaypoints = wm.GetWaypoints();
    }

    public override void Execute(NPCBase owner)
    {
        if (owner.CompareTag("Guard"))
        {
            Guard guard = owner as Guard;
            if (rute == null || rute.Count == 0)
            {
                patrolPoints = guard.puntosDePatrulla;
                calculateRute(owner, patrolPoints);
            }
        }

        else if (owner.CompareTag("Dog"))
        {
            Dog dog = owner as Dog;
            if (rute == null || rute.Count == 0)
            {
                patrolPoints = dog.waypointsZonePatrol;
                calculateRute(owner, patrolPoints);
            }
        }

        if (owner.PlayerIsBeingSeen)
            owner.FSM.TriggerEvent(StateEvent.StartChase);
        
        if(rute == null || rute.Count == 0)
        {
            Vector3 dirToWaypoint = (patrolWaypoint.position - owner.transform.position).normalized;
            owner.transform.position += dirToWaypoint * owner.currentSpeed * Time.deltaTime;

            if (dirToWaypoint != Vector3.zero)
                owner.transform.forward = Vector3.Lerp(owner.transform.forward, dirToWaypoint, Time.deltaTime * 5f);
        }

        Waypoint target = rute[0];
        Vector3 dir = (target.position - owner.transform.position).normalized;
        owner.transform.position += dir * owner.currentSpeed * Time.deltaTime;

        if(dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            owner.transform.rotation = Quaternion.Lerp(owner.transform.rotation, targetRot, Time.deltaTime * 5f);
        }
        
        Vector3 dis = target.position - owner.transform.position;
        if(dis.sqrMagnitude < owner.distanciaMinima)
        {
            if(!TryAdvanceToNextWaypoint())
            {
                owner.FSM.TriggerEvent(StateEvent.returnRute);
            }
        }
    }

    private bool TryAdvanceToNextWaypoint()
    {
        if (rute == null || rute.Count == 0)
            return false;

        // Elimina el waypoint actual (ya alcanzado)
        rute.RemoveAt(0);

        // Devuelve si aun quedan mas puntos
        return rute.Count > 0;
    }

    private void calculateRute(NPCBase owner, List<Waypoint> patrolPoints)
    {
        if(wm == null)return;
        
        Waypoint enemyWaypoint = Pathfinder.FindNearestWaypointPlayer(owner.transform.position, allWaypoints);
        patrolWaypoint = Pathfinder.FindNearestWaypointPlayer(owner.transform.position, patrolPoints);
        rute = Pathfinder.FindPath(enemyWaypoint, patrolWaypoint);

    }

    public override System.Type GetNextStateForEvent(StateEvent evt)
    {
        if (evt == StateEvent.returnRute)
            return typeof(PatrolState);
        if (evt == StateEvent.StartChase)
            return typeof(PersecuteState);
        return null;
    } 
}
