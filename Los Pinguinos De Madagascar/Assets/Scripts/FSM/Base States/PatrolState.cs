using UnityEngine;

public class PatrolState : State
{
    public override void Enter(NPCBase owner)
    {
        Debug.Log("Patrullando");
        owner.NoiseDetector?.SetDetectionEnabled(true);
        owner.VisionDetector?.SetDetectionEnabled(true);
        owner.SelectNewDestination();
    }

    public override void Execute(NPCBase owner)
    {
        owner.MoverHacia(owner.CurrentDestination, MovementType.Walk);

        if (Vector3.Distance(owner.transform.position, owner.CurrentDestination) < owner.distanciaMinima)
            owner.SelectNewDestination();
    }

    public override System.Type GetNextStateForEvent(StateEvent evt)
    {
        if (evt == StateEvent.NoiseHeard || evt == StateEvent.PlayerSeen)
            return typeof(AlertedState);
        return null;
    }
}
