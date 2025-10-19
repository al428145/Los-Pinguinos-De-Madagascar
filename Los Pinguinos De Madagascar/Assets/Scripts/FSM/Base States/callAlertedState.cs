using System.Collections.Generic;
using UnityEngine;

public class callAlertedState : State
{
    private List<Waypoint> rute;
    private List<Waypoint> allWaypoints;
    private Vector3 lastPositionPlayer;
    private WaypointManager wm;

    private float timer;                  // para recalcular ruta
    private float stuckTimer;             // para detectar atasco
    private Vector3 lastPositionNPC;      // última posición del NPC

    public override void Enter(NPCBase owner)
    {
        Debug.Log($"{owner.name} ha entrado en estado ALERTADO.");
        timer = 0f;
        stuckTimer = 0f;
        lastPositionNPC = owner.transform.position;

        lastPositionPlayer = owner.player.transform.position;
        rute = new List<Waypoint>();
        wm = Object.FindObjectOfType<WaypointManager>();
        allWaypoints = wm.GetWaypoints();

        calculateRute(owner);
    }

    public override void Execute(NPCBase owner)
    {
        // --- 1. Detección de atasco ---
        float moved = Vector3.Distance(owner.transform.position, lastPositionNPC);
        if (moved < 0.05f)
        {
            stuckTimer += Time.deltaTime;
        }
        else
        {
            stuckTimer = 0f;
        }
        lastPositionNPC = owner.transform.position;

        if (stuckTimer >= 3f)
        {
            Debug.LogWarning($"{owner.name} está atascado en callAlertedState. Volviendo a patrullar...");
            owner.FSM.TriggerEvent(StateEvent.investigationFinished);
            return;
        }

        // --- 2. Recalcular ruta periódicamente ---
        if (timer < 3f)
        {
            timer += Time.deltaTime;
        }
        else
        {
            calculateRute(owner);
            timer = 0f;
        }

        // --- 3. Si ve al jugador, cambia de estado ---
        if (owner.PlayerIsBeingSeen || owner.PlayerStillInRange)
        {
            owner.FSM.TriggerEvent(StateEvent.playerFindInRute);
            return;
        }

        // --- 4. Seguir ruta ---
        if (rute != null && rute.Count > 0)
        {
            Waypoint target = rute[0];
            owner.MoverHacia(target.position, MovementType.Walk);

            Vector3 direccionAlDestino = target.position - owner.transform.position;
            direccionAlDestino.y = 0;

            if (direccionAlDestino.sqrMagnitude < owner.distanciaMinima)
            {
                if (!TryAdvanceToNextWaypoint())
                    rute.Clear();
            }
        }
        else
        {
            // --- 5. Si no hay ruta, ir directamente al último punto donde se vio al jugador ---
            Vector3 dirToPlayer = (lastPositionPlayer - owner.transform.position);
            if (dirToPlayer.sqrMagnitude > owner.distanciaMinima)
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

        rute.RemoveAt(0);
        return rute.Count > 0;
    }

    public override System.Type GetNextStateForEvent(StateEvent evt)
    {
        if (evt == StateEvent.playerFindInRute)
            return typeof(PersecuteState);
        if (evt == StateEvent.investigationFinished)
            return typeof(returnPatrolState);
        return null;
    }

    private void calculateRute(NPCBase owner)
    {
        if (wm == null) return;

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
        Debug.Log($"{owner.name}: ruta alertada calculada ({rute.Count} puntos).");
    }
}
