using UnityEngine;

public class ChickenSleepState : State
{
    public override void Enter(NPCBase owner)
    {
        // Activamos los detectores
        owner.NoiseDetector?.SetDetectionEnabled(true);
        owner.VisionDetector?.SetDetectionEnabled(true);

        // Activamos partículas de dormir
        if (owner is Gallina gallina && gallina.particulasDormido != null)
        {
            gallina.particulasDormido.gameObject.SetActive(true);
            gallina.particulasDormido.Play();
        }
    }

    public override void Execute(NPCBase owner)
    {
     

        // Ver si escuchó un ruido
        if (owner.NoiseDetector != null && owner.NoiseDetector.HasHeardNoise)
        {
            owner.HandleNoise(owner.NoiseDetector.LastNoisePosition);

            // Importante: resetear el flag para evitar spam
            owner.NoiseDetector.HasHeardNoise = false;
        }
    }

    public override void Exit(NPCBase owner)
    {
        if (owner is Gallina gallina && gallina.particulasDormido != null)
        {
            gallina.particulasDormido.Stop();
        }
    }

    public override System.Type GetNextStateForEvent(StateEvent evt)
    {
        Debug.Log(this.GetType().Name + " recibió evento: " + evt);
        if (evt == StateEvent.PlayerHeard || evt == StateEvent.NoiseHeard)
            return typeof(ChickenAvisandoState);
        return null;
    }
}
