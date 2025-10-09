using System.Collections.Generic;
using UnityEngine;

public class PersecuteState : State
{
    public override void Enter(NPCBase owner)
    {
        Debug.Log($"{owner.name} comienza la persecución!");
    }

    public override void Execute(NPCBase owner)
    {
        // Aquí podrías usar tu Pathfinder:
        if (owner is Dog dog)
        {
            WaypointManager wm = Object.FindObjectOfType<WaypointManager>();
            if (wm == null) return;

            List<Waypoint> waypoints = wm.GetWaypoints();
            Waypoint enemyWaypoint = Pathfinder.FindTheNearestWaypointEnemy(dog.transform.position, dog.LastSeenPosition, waypoints);
            Waypoint playerWaypoint = Pathfinder.FindNearestWaypointPlayer(dog.LastSeenPosition, waypoints);
            List<Waypoint> ruta = Pathfinder.FindPath(enemyWaypoint, playerWaypoint);
        }
    }

    public override System.Type GetNextStateForEvent(StateEvent evt)
    {
        if (evt == StateEvent.LostPlayer)
            return typeof(PatrolState);
        return null;
    }
}
