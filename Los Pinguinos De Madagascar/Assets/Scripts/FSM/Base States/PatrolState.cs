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
        // Calcula el vector de direccion entre el NPC y su destino
        Vector3 direccionAlDestino = owner.CurrentDestination - owner.transform.position;

        // Importante! Ignoramos la diferencia de altura para la comprobacion
        direccionAlDestino.y = 0;
        owner.MoverHacia(owner.CurrentDestination, MovementType.Walk);
        //Debug.Log(Vector3.Distance(owner.transform.position, owner.CurrentDestination));
        if (direccionAlDestino.sqrMagnitude < owner.distanciaMinima * owner.distanciaMinima)
        {
            //Debug.Log("Ha llegado al destino. Eligiendo uno nuevo.");
            owner.SelectNewDestination();
        }
    }
    public override System.Type GetNextStateForEvent(StateEvent evt)
    {
        if (evt == StateEvent.NoiseHeard || evt == StateEvent.PlayerSeen)
            return typeof(InvestigateState);
        return null;
    }
}
