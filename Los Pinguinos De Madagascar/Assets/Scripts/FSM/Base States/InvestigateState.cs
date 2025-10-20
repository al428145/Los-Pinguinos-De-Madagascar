using UnityEngine;

public class InvestigateState : State
{
    public override void Enter(NPCBase owner)
    {
        owner.CurrentDestination = owner.LastHeardPosition;

        if (owner is Guard guard && guard.investigateIcon != null)
            guard.investigateIcon.SetActive(true);
    }

    public override void Execute(NPCBase owner)
    {

        if (owner.PlayerIsBeingSeen)
        {
            owner.FSM.TriggerEvent(StateEvent.StartChase);
            return; 
        }

        owner.MoverHacia(owner.CurrentDestination, MovementType.Walk);

        if (Vector3.Distance(owner.transform.position, owner.CurrentDestination) < owner.distanciaMinima)
        {
            owner.FSM.TriggerEvent(StateEvent.InvestigateDone);
        }
    }

    public override void Exit(NPCBase owner)
    {
        // Desactivar imagen de investigaci�n
        if (owner is Guard guard && guard.investigateIcon != null)
            guard.investigateIcon.SetActive(false);
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