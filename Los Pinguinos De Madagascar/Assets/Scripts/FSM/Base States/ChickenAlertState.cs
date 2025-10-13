using UnityEngine;

public class ChickenAlertState : State
{
    private float timer;
    private const float alertDuration = 5f;

    private AlertPanelController alertPanel;
    private Vector3 centerPosition;
    private float angle; // Ángulo actual en la circunferencia
    private float radius = 1.5f; // Radio del círculo
    private float angularSpeed = 90f; // Grados por segundo

    public override void Enter(NPCBase owner)
    {
        timer = 0f;
        centerPosition = owner.transform.position; // Guardar posición inicial como centro
        angle = 0f;

        // Buscar panel de alerta
        if (alertPanel == null)
            alertPanel = GameObject.FindObjectOfType<AlertPanelController>();
        alertPanel?.ShowAlert();

        if (owner.animator != null)
            owner.animator.SetBool("correr", true);

        owner.NoiseDetector?.SetDetectionEnabled(true);
        owner.VisionDetector?.SetDetectionEnabled(true);
    }

    public override void Execute(NPCBase owner)
    {
        timer += Time.deltaTime;

        // Actualizar ángulo
        angle += angularSpeed * Time.deltaTime;
        if (angle >= 360f)
        {
            // Vuelta completa, volver al centro y cambiar a sueño
            owner.FSM.TriggerEvent(StateEvent.AlertTimeout);
            return;
        }

        // Calcular posición circular
        float rad = angle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * radius;
        Vector3 targetPosition = centerPosition + offset;

        // Mover gallina hacia esa posición
        owner.MoverHacia(targetPosition, MovementType.Walk);
    }

    public override void Exit(NPCBase owner)
    {
        // Volver al centro exacto
        owner.transform.position = centerPosition;

        if (owner.animator != null)
            owner.animator.SetBool("correr", false);

        owner.NoiseDetector?.SetDetectionEnabled(false);
        owner.VisionDetector?.SetDetectionEnabled(false);

        alertPanel?.HideAlert();
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
