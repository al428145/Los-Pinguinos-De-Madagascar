using UnityEngine;

public class AlertedState : State
{
    private float timer;

    public override void Enter(NPCBase owner)
    {
        timer = 0f;

        if (owner is Guard guard)
            guard.PlayAlertSound();

        if (owner is Dog perro)
            perro.PlayAlertSound();

        if (owner is Guard guardia && guardia.investigateIcon != null)
            guardia.investigateIcon.SetActive(true);
    }

    public override void Execute(NPCBase owner)
    {
        timer += Time.deltaTime;
        owner.LookAtNoise(owner.LastHeardPosition);
        if (timer >= 1f)
        {
            // Si mientras est� alertado VE al jugador, que pase a persecuci�n
            if (owner.PlayerIsBeingSeen)
        {
            owner.FSM.TriggerEvent(StateEvent.PlayerSeen);
            return; // Salimos para que no ejecute lo de abajo
        }

        // Si el tiempo de alerta se acaba Y NO VIO AL JUGADOR, va a investigar
        
            owner.FSM.TriggerEvent(StateEvent.AlertTimeout);
        }
    }


    public override void Exit(NPCBase owner)
    {
        owner.NoiseDetector?.SetDetectionEnabled(true);
        owner.VisionDetector?.SetDetectionEnabled(true);
        if (owner is Guard guard)
            guard.StopAlertSound();
        if (owner is Dog perro)
            perro.StopAlertSound();

        if (owner is Guard guardia && guardia.investigateIcon != null)
            guardia.investigateIcon.SetActive(false);
    }

    public override System.Type GetNextStateForEvent(StateEvent evt)
    {
        if (evt == StateEvent.AlertTimeout)
            return typeof(InvestigateState);

        else if (evt == StateEvent.InvestigateDone) // Este 'else' ya no se usar�
            return typeof(PatrolState);

        // --- A�ADE ESTA L�NEA ---
        else if (evt == StateEvent.PlayerSeen)
            return typeof(PersecuteState);
        // -------------------------

        return null;
    }
}
