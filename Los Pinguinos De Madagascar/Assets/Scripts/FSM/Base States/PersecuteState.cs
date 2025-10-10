using System.Collections.Generic;
using UnityEngine;

public class PersecuteState : State
{
    public override void Enter(NPCBase owner)
    {
        Debug.Log($"{owner.name} comienza la persecucion!");
    }

    public override void Execute(NPCBase owner)
    {
        WaypointManager wm = Object.FindObjectOfType<WaypointManager>();
        if (wm == null) return;

        List<Waypoint> waypoints = wm.GetWaypoints();
        Waypoint enemyWaypoint = Pathfinder.FindTheNearestWaypointEnemy(owner.transform.position, owner.player.transform.position, waypoints);
        Waypoint playerWaypoint = Pathfinder.FindNearestWaypointPlayer(owner.player.transform.position, waypoints);
        List<Waypoint> ruta = Pathfinder.FindPath(enemyWaypoint, playerWaypoint);
    }

    public override System.Type GetNextStateForEvent(StateEvent evt)
    {
        if (evt == StateEvent.LostPlayer)
            return typeof(PatrolState);
        return null;
    }
}
