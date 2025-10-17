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

    public override void Enter(NPCBase owner)
    {
        Debug.Log($"{owner.name} vuelve a patrullar");
        rute = new List<Waypoint>();
        currentWaypointIndex = 0;
        wm = Object.FindObjectOfType<WaypointManager>();
        allWaypoints = wm.GetWaypoints();
    }

    public override void Execute(NPCBase owner)
    {
        if(owner.CompareTag("Guard"))
        {
            Guard guard = owner as Guard;
            if(rute == null || rute.Count == 0)
            {
                patrolPoints = guard.puntosDePatrulla;
                calculateRute(owner, patrolPoints);
            }
        }

        else if(owner.CompareTag("Dog"))
        {
            Dog dog = owner as Dog;
            if(rute == null || rute.Count == 0)
            {
                patrolPoints = dog.waypointsZonePatrol;
                calculateRute(owner, patrolPoints);
            }
        }

        Waypoint target = rute[currentWaypointIndex];
        Vector3 dir = (target.position - owner.transform.position).normalized;
        owner.transform.position += dir * owner.currentSpeed * Time.deltaTime;

        if(dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            owner.transform.rotation = Quaternion.Lerp(owner.transform.rotation, targetRot, Time.deltaTime * 5f);
        }
        
        float dis = Vector3.Distance(owner.transform.position, target.position);
        if(dis <= 0.5f)
        {
            currentWaypointIndex++;

            if(currentWaypointIndex >= rute.Count)
            {
                owner.FSM.TriggerEvent(StateEvent.returnRute);
            }
        }
    }

    private void calculateRute(NPCBase owner, List<Waypoint> patrolPoints)
    {
        if(wm == null)return;
        
        Waypoint enemyWaypoint = Pathfinder.FindNearestWaypointPlayer(owner.transform.position, waypoints);
        Waypoint patrolWaypoint = Pathfinder.FindNearestWaypointPlayer(owner.transform.position, patrolPoints);
        rute = Pathfinder.FindPath(enemyWaypoint, patrolWaypoint);

        currentWaypointIndex = 0;
    }

    public override System.Type GetNextStateForEvent(StateEvent evt)
    {
        if(evt == StateEvent.returnRute)
            return typeof(PatrolState);
        return null;
    } 
}
