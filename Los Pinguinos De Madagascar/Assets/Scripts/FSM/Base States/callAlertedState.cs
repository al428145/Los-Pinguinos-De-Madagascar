using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class callAlertedState : State
{
    private List<Waypoint> rute;
    private List<Waypoint> allWaypoints;
    private Vector3 lastPositionPlayer;
    private WaypointManager wm;
    private float timer;

    public override void Enter(NPCBase owner)
    {
        if (rute != null && rute.Count > 0)
        {
            Debug.Log($"{owner.name}: ya estaba alertado, no recalculo ruta.");
            return;
        }
        timer = 0f;
        lastPositionPlayer = owner.player.transform.position;
        rute = new List<Waypoint>();
        wm = Object.FindObjectOfType<WaypointManager>();
        allWaypoints = wm.GetWaypoints();
        calculateRute(owner);
    }

    public override void Execute(NPCBase owner)
    {
        if (timer < 3f)
        {
            timer += Time.deltaTime;
        }
        else if (timer >= 3f)
        {
            calculateRute(owner);
            timer = -999999999;
        }

        if(owner.PlayerIsBeingSeen || owner.PlayerStillInRange)
        {
            owner.FSM.TriggerEvent(StateEvent.playerFindInRute);
            return;
        }

        if (rute.Count > 0)
        {
            Waypoint target = rute[0];
            owner.MoverHacia(target.position, MovementType.Walk);

            Vector3 direccionAlDestino = target.position - owner.transform.position;
            direccionAlDestino.y = 0;
            if (direccionAlDestino.sqrMagnitude < owner.distanciaMinima)
            {
                if (!TryAdvanceToNextWaypoint())
                {
                    rute = new List<Waypoint>();
                }
            }
        }
        
        else if(rute == null || rute.Count == 0)
        {
            Vector3 dirToPlayer = (lastPositionPlayer - owner.transform.position);

            if(dirToPlayer.sqrMagnitude > owner.distanciaMinima)
            {
                dirToPlayer.Normalize();
                owner.transform.position += dirToPlayer * owner.currentSpeed * Time.deltaTime;
                owner.transform.forward = Vector3.Lerp(owner.transform.forward, dirToPlayer, Time.deltaTime * 5f);
            }
            else
            {
                owner.FSM.TriggerEvent(StateEvent.investigationFinished);
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

        if (enemyWaypoint == null || playerWaypoint == null)
        {
            rute = new List<Waypoint>();
            return;
        }

        if (enemyWaypoint == playerWaypoint)
        {
            rute = new List<Waypoint> { enemyWaypoint };
            return;
        }

        rute = Pathfinder.FindPath(enemyWaypoint, playerWaypoint);
        Debug.Log("La ruta del callAlerted es de esta longitud" + rute.Count);
    }
}
