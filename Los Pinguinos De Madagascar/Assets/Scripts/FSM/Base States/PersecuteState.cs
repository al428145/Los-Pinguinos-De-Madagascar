using System.Collections.Generic;
using UnityEngine;

public class PersecuteState : State
{
    private List<Waypoint> rute;
    private float recalcTimer;
    private Vector3 lastPositionPlayer;
    private WaypointManager wm;
    private float losePlayerTimer;
    private float loseDelay;
    private List<Waypoint> waypoints;

    public override void Enter(NPCBase owner)
    {
        Debug.Log($"{owner.name} comienza la persecucion!");
        rute = new List<Waypoint>();
        owner.currentSpeed = owner.speeds.GetSpeed(MovementType.Run);
        owner.animator?.SetFloat("Speed", owner.currentSpeed);
        recalcTimer = 0f;
        lastPositionPlayer = owner.player.transform.position;
        loseDelay = 2f;
        wm = Object.FindObjectOfType<WaypointManager>();
        waypoints = wm.GetWaypoints();
        calculateRute(owner);
    }

    public override void Execute(NPCBase owner)
    {
        Vector3 distToPlayer = owner.transform.position - owner.player.transform.position;
        distToPlayer.y = 0;

        // Si está muy cerca del jugador, ignora la ruta y muévete directo
        if (rute == null || rute.Count == 0 || distToPlayer.sqrMagnitude < 5f)
        {
            recalcTimer += Time.deltaTime;

            // Recalcula si pasa tiempo o el jugador se ha movido mucho
            if (recalcTimer > 3f || Vector3.Distance(owner.player.transform.position, lastPositionPlayer) > 2f)
            {
                calculateRute(owner);
                recalcTimer = 0f;
                lastPositionPlayer = owner.player.transform.position;
            }

            Vector3 dirToPlayer = (owner.player.transform.position - owner.transform.position).normalized;
            owner.transform.position += dirToPlayer * owner.currentSpeed * Time.deltaTime;

            if (dirToPlayer != Vector3.zero)
                owner.transform.forward = Vector3.Lerp(owner.transform.forward, dirToPlayer, Time.deltaTime * 5f);

            HandleLostPlayer(owner);
            return;
        }

        // Actualiza temporizador para recalcular ruta
        recalcTimer += Time.deltaTime;
        if (recalcTimer > 3f || Vector3.Distance(owner.player.transform.position, lastPositionPlayer) > 2f)
        {
            calculateRute(owner);
            recalcTimer = 0f;
            lastPositionPlayer = owner.player.transform.position;
        }

        // Movimiento siguiendo la ruta
        if (rute == null || rute.Count == 0)
            return;

        Waypoint target = rute[0];
        owner.MoverHacia(target.position, MovementType.Run);

        Vector3 direccionAlDestino = target.position - owner.transform.position;
        direccionAlDestino.y = 0;

        // Si llega al waypoint, pasa al siguiente
        if (direccionAlDestino.sqrMagnitude < owner.distanciaMinima)
        {
            if (!TryAdvanceToNextWaypoint())
            {
                calculateRute(owner);
            }
        }

        HandleLostPlayer(owner);
    }

    private bool TryAdvanceToNextWaypoint()
    {
        if (rute == null || rute.Count == 0)
            return false;

        // Elimina el waypoint actual (ya alcanzado)
        rute.RemoveAt(0);

        // Devuelve si aún quedan más puntos
        return rute.Count > 0;
    }

    private void calculateRute(NPCBase owner)
    {
        if (wm == null || waypoints == null || waypoints.Count == 0 || owner == null)
            return;

        Debug.Log("Recalculando ruta");

        Waypoint enemyWaypoint = Pathfinder.FindTheNearestWaypointEnemy(owner.transform.position, owner.player.transform.position, waypoints);
        Waypoint playerWaypoint = Pathfinder.FindNearestWaypointPlayer(owner.player.transform.position, waypoints);

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
        Debug.Log("Ruta recalculada. Nodos: " + rute.Count);
    }

    private void HandleLostPlayer(NPCBase owner)
    {
        if (!owner.PlayerIsBeingSeen && !owner.PlayerStillInRange)
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

    public override System.Type GetNextStateForEvent(StateEvent evt)
    {
        if (evt == StateEvent.LostPlayer)
            return typeof(returnPatrolState);
        return null;
    }
}
