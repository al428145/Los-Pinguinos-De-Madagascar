using UnityEngine;

public class ChickenSleepState : State
{
    public override void Enter(NPCBase owner)
    {
        // Color azul (solo debug visual)
        var renderer = owner.GetComponent<Renderer>();
        if (renderer != null) renderer.material.color = Color.blue;

        // Animación idle
        if (owner.animator != null)
            owner.animator.SetFloat("Speed", owner.speeds.GetSpeed(MovementType.Idle));
    }

    public override void Execute(NPCBase owner)
    {
        // No hace nada, solo espera detección
        // Si escucha ruido o ve al jugador, FSM.TriggerEvent(StateEvent.PlayerHeard/Seen)
    }

    public override System.Type GetNextStateForEvent(StateEvent evt)
    {
        if (evt == StateEvent.PlayerHeard || evt == StateEvent.PlayerSeen)
            return typeof(ChickenAvisandoState);
        return null;
    }
}
