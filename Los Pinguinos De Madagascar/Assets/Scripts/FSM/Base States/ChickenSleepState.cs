using UnityEngine;

public class ChickenSleepState : State
{
    public override void Enter(NPCBase owner)
    {
        owner.NoiseDetector?.SetDetectionEnabled(true);
        owner.VisionDetector?.SetDetectionEnabled(true);
        
        var gallina = owner as Gallina;
        if (gallina != null && gallina.particulasDormido != null)
        {
            gallina.particulasDormido.gameObject.SetActive(true);
            gallina.particulasDormido.Play();
        }
    }
    public override void Exit(NPCBase owner)
    {
        var gallina = owner as Gallina;
        gallina.particulasDormido.Stop();
         
        
    }

    public override void Execute(NPCBase owner)
    {
        // No hace nada, solo espera detección
        // Si escucha ruido o ve al jugador, FSM.TriggerEvent(StateEvent.PlayerHeard/Seen)
    }

    public override System.Type GetNextStateForEvent(StateEvent evt)
    {
        Debug.Log(this.GetType().Name + " recibió evento: " + evt);
        if (evt == StateEvent.NoiseHeard || evt == StateEvent.PlayerHeard || evt == StateEvent.PlayerSeen)
            return typeof(ChickenAvisandoState);
        return null;
    }

}
