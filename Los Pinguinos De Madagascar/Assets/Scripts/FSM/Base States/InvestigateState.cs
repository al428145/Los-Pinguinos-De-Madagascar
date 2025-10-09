using UnityEngine;

public class InvestigateState : State
{
    public override void Enter(NPCBase owner)
    {
        Debug.Log("Pasando a investigar");
        owner.CurrentDestination = owner.LastHeardPosition;
    }

    public override void Execute(NPCBase owner)
    {
        owner.MoverHacia(owner.CurrentDestination, MovementType.Walk);

        if (Vector3.Distance(owner.transform.position, owner.CurrentDestination) < owner.distanciaMinima)
        {
            // Si ve al jugador -> perseguir
            if (owner.PlayerIsBeingSeen)
                owner.FSM.TriggerEvent(StateEvent.StartChase);
            else
                owner.FSM.TriggerEvent(StateEvent.InvestigateDone);
        }
    }

    public override System.Type GetNextStateForEvent(StateEvent evt)
    {
        if (evt == StateEvent.InvestigateDone)
            return typeof(PatrolState);
        if (evt == StateEvent.StartChase)
            return typeof(PersecuteState);
        return null;
    }
}
