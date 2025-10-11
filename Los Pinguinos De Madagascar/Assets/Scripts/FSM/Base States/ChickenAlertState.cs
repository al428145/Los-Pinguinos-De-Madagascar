using UnityEngine;

public class ChickenAlertState : State
{
    public override void Enter(NPCBase owner)
    {
        var renderer = owner.GetComponent<Renderer>();
        if (renderer != null) renderer.material.color = Color.yellow;

        if (owner.animator != null)
            owner.animator.SetBool("Alerta", true);
    }

    public override void Execute(NPCBase owner)
    {
        // Aquí podrías hacer que mire alrededor, etc.
    }

    public override System.Type GetNextStateForEvent(StateEvent evt)
    {
        // Podrías hacer que si escucha otro ruido, vuelva a avisar
        if (evt == StateEvent.PlayerHeard || evt == StateEvent.PlayerSeen)
            return typeof(ChickenAvisandoState);
        return null;
    }
}
