using UnityEngine;

public class AlertedState : State
{
    private float timer;

    public override void Enter(NPCBase owner)
    {
        Debug.Log("Estado de alerta");
        timer = 0f;
    }

    public override void Execute(NPCBase owner)
    {
        timer += Time.deltaTime;
        owner.LookAtNoise(owner.LastHeardPosition);

        if (timer >= 2f)
        {
            if (owner.PlayerStillInRange || owner.PlayerIsBeingSeen)
            {
                owner.FSM.TriggerEvent(StateEvent.AlertTimeout);
            }
            else
            {
                owner.FSM.TriggerEvent(StateEvent.InvestigateDone);
            }

            owner.PlayerStillInRange = false; // reset
        }
    }


    public override void Exit(NPCBase owner)
    {
        owner.NoiseDetector?.SetDetectionEnabled(true);
        owner.VisionDetector?.SetDetectionEnabled(true);
    }

    public override System.Type GetNextStateForEvent(StateEvent evt)
    {
        if (evt == StateEvent.AlertTimeout)
            return typeof(InvestigateState);

        else if (evt == StateEvent.InvestigateDone)
            return typeof(PatrolState);
        return null;
    }
}
