using UnityEngine;

public class ChickenAlertState : State
{
    private float timer;
    private const float alertDuration = 5f; // 🕒 Duración de la alerta en segundos

    public override void Enter(NPCBase owner)
    {
        Debug.Log("Gallina entra en AlertState 🐔⚠️");

        timer = 0f;

        var renderer = owner.GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = Color.yellow;

        if (owner.animator != null)
            owner.animator.SetBool("Alerta", true);

        // Activar sensores
        owner.NoiseDetector?.SetDetectionEnabled(true);
        owner.VisionDetector?.SetDetectionEnabled(true);
    }

    public override void Execute(NPCBase owner)
    {
        timer += Time.deltaTime;

        // 👀 Si vuelve a ver o escuchar al jugador, se mantiene alerta
        if (owner.VisionDetector != null && owner.VisionDetector.HasSeenPlayer)
        {
            owner.HandleVision(owner.VisionDetector.LastSeenPosition);
            owner.VisionDetector.HasSeenPlayer = false;
            timer = 0f; // Reinicia el temporizador
        }

        if (owner.NoiseDetector != null && owner.NoiseDetector.HasHeardNoise)
        {
            owner.HandleNoise(owner.NoiseDetector.LastNoisePosition);
            owner.NoiseDetector.HasHeardNoise = false;
            timer = 0f;
        }

        // 👁️ Mantener mirando hacia donde escuchó o vio algo
        if (owner.LastHeardPosition != Vector3.zero)
            owner.LookAtNoise(owner.LastHeardPosition);

        // ⏳ Si pasan 5 segundos sin estímulos, volver a dormir
        if (timer >= alertDuration)
        {
            Debug.Log("Gallina se calma y vuelve a dormir 😴");
            owner.FSM.TriggerEvent(StateEvent.AlertTimeout);
        }
    }

    public override void Exit(NPCBase owner)
    {
        if (owner.animator != null)
            owner.animator.SetBool("Alerta", false);

        // Desactivar visión/ruido (para que no gaste recursos mientras duerme)
        owner.NoiseDetector?.SetDetectionEnabled(false);
        owner.VisionDetector?.SetDetectionEnabled(false);
    }

    public override System.Type GetNextStateForEvent(StateEvent evt)
    {
        if (evt == StateEvent.PlayerHeard || evt == StateEvent.PlayerSeen)
            return typeof(ChickenAvisandoState);

        if (evt == StateEvent.AlertTimeout)
            return typeof(ChickenSleepState);

        return null;
    }
}
