using System.Collections.Generic;
using UnityEngine;

public class PersecuteState : State
{
    private List<Waypoint> rute;
    private int currentWaypointIndex;
    private float recalcTimer;
    private Vector3 lastPositionPlayer;
    private WaypointManager wm;
    private float losePlayerTimer;
    private float loseDelay;

    public override void Enter(NPCBase owner)
    {
        Debug.Log($"{owner.name} comienza la persecucion!");
        rute = new List<Waypoint>();
        currentWaypointIndex = 0;
        owner.currentSpeed = owner.speeds.GetSpeed(MovementType.Run);
        owner.animator?.SetFloat("Speed", owner.currentSpeed);
        recalcTimer = 0f;
        lastPositionPlayer = owner.player.transform.position;
        loseDelay = 2f;
        wm = Object.FindObjectOfType<WaypointManager>();
        calculateRute(owner);
    }

    public override void Execute(NPCBase owner)
    {
        Vector3 distToPlayer = owner.transform.position - owner.player.transform.position;
        distToPlayer.y = 0;
        
        if(rute == null || rute.Count == 0 || distToPlayer.sqrMagnitude < 5f)
        {
            recalcTimer += Time.deltaTime;

            //recalculate the rute if it pass 3 second or if the player is far
            if (recalcTimer > 3f || Vector3.Distance(owner.player.transform.position, lastPositionPlayer) > 2f)
            {
                calculateRute(owner);
                recalcTimer = 0f;
                lastPositionPlayer = owner.player.transform.position;
            }
        
            Vector3 dirToPlayer = (owner.player.transform.position - owner.transform.position).normalized;
            owner.transform.position += dirToPlayer * owner.currentSpeed * Time.deltaTime;

            if(dirToPlayer != Vector3.zero)
                owner.transform.forward = Vector3.Lerp(owner.transform.forward, dirToPlayer, Time.deltaTime * 5f);

            
            if (!owner.PlayerIsBeingSeen || !owner.PlayerStillInRange)
            {
                losePlayerTimer += Time.deltaTime;
                if (losePlayerTimer > loseDelay)
                    owner.FSM.TriggerEvent(StateEvent.LostPlayer);
            }
            else
            {
                losePlayerTimer = 0f;
            }

            return;
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
        owner.MoverHacia(target.position, MovementType.Run);

        Vector3 direccionAlDestino = target.position - owner.transform.position;
        direccionAlDestino.y = 0;
        if (direccionAlDestino.sqrMagnitude < owner.distanciaMinima)
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= rute.Count)
            {
                calculateRute(owner);
                currentWaypointIndex = 0;
            }
        }
        Debug.Log(currentWaypointIndex);

        if (!owner.PlayerIsBeingSeen || !owner.PlayerStillInRange)
        {
            losePlayerTimer += Time.deltaTime;
            if (losePlayerTimer > loseDelay)
                owner.FSM.TriggerEvent(StateEvent.LostPlayer);
        }
        else
        {
            losePlayerTimer = 0f;
        }
    }

    private void calculateRute(NPCBase owner)
    {
        if (wm == null) return;
        Debug.Log("Recalculando ruta");

        List<Waypoint> waypoints = wm.GetWaypoints();
        Waypoint enemyWaypoint = Pathfinder.FindTheNearestWaypointEnemy(owner.transform.position, owner.player.transform.position, waypoints);
        //Debug.Log("WP enemigo: " + enemyWaypoint.position);
        Waypoint playerWaypoint = Pathfinder.FindNearestWaypointPlayer(owner.player.transform.position, waypoints);
        //Debug.Log("WP player: " + playerWaypoint.position);

        if(enemyWaypoint == playerWaypoint)
        {
            rute = new List<Waypoint>();
            return;
        }

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
