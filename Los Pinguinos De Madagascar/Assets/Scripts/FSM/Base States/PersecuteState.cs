using System.Collections.Generic;
using UnityEngine;

public class PersecuteState : State
{
    private List<Waypoint> rute;
    private int currentWaypointIndex;
    private float recalcTimer;
    private Vector3 lastPositionPlayer;
    private WaypointManager wm;

    public override void Enter(NPCBase owner)
    {
        Debug.Log($"{owner.name} comienza la persecucion!");
        rute = new List<Waypoint>();
        currentWaypointIndex = 0;
        recalcTimer = 0f;
        lastPositionPlayer = owner.player.transform.position;
        wm = Object.FindObjectOfType<WaypointManager>();
        //calculateRute(owner);
    }

    public override void Execute(NPCBase owner)
    {
        if(rute == null || rute.Count == 0)
        {
            calculateRute(owner);
            //Debug.Log(rute.Count);
            //return;
        }

        recalcTimer += Time.deltaTime;

        //recalculate the rute if it pass 3 second or if the player is far
        if(recalcTimer > 3f || Vector3.Distance(owner.player.transform.position, lastPositionPlayer) > 2f)
        {
            calculateRute(owner);
            recalcTimer = 0f;
            lastPositionPlayer = owner.player.transform.position;
        }

        //Move to the waypoint
        Waypoint target = rute[currentWaypointIndex];
        Vector3 dir = (target.position - owner.transform.position).normalized;
        owner.transform.position += dir * owner.currentSpeed * Time.deltaTime;

        if(dir != Vector3.zero)
            owner.transform.forward = Vector3.Lerp(owner.transform.position, dir, Time.deltaTime * 5f);
        
        float dis = Vector3.Distance(owner.transform.position, target.position);
        if(dis <= 0.5f)
        {
            currentWaypointIndex++;

            if(currentWaypointIndex >= rute.Count)
            {
                calculateRute(owner);
                currentWaypointIndex = 0;
            }
        }

        if(owner.PlayerIsBeingSeen || owner.PlayerStillInRange)
        {
            owner.FSM.TriggerEvent(StateEvent.LostPlayer);
        }

    }

    private void calculateRute(NPCBase owner)
    {
        if (wm == null) return;

        List<Waypoint> waypoints = wm.GetWaypoints();
        Waypoint enemyWaypoint = Pathfinder.FindTheNearestWaypointEnemy(owner.transform.position, owner.player.transform.position, waypoints);
        Debug.Log(enemyWaypoint.position);
        Waypoint playerWaypoint = Pathfinder.FindNearestWaypointPlayer(owner.player.transform.position, waypoints);
        rute = Pathfinder.FindPath(enemyWaypoint, playerWaypoint);

        currentWaypointIndex = 0;
    }

    public override System.Type GetNextStateForEvent(StateEvent evt)
    {
        if (evt == StateEvent.LostPlayer)
            return typeof(returnPatrolState);
        return null;
    }
}
