using UnityEngine;

public class CamaraWarning : State
{
    private float timer;
    private AlertPanelController alertPanel;
    private const float warningDuration = 5f;

    public override void Enter(NPCBase owner)
    {
        Debug.Log(" Camara ALERTA ");
        timer = 0f;

        var cam = owner as SecurityCamNPC;

        if (cam?.alarmSound != null)
        {
            if (cam.alarmSound.clip != null && cam.alarmSound.clip.length > 3f)
                  // Empieza a sonar desde el segundo 3
            cam.alarmSound.Play();
        }

        if (alertPanel == null)
            alertPanel = GameObject.FindObjectOfType<AlertPanelController>();
        alertPanel?.ShowAlert();
        owner.VisionDetector?.SetDetectionEnabled(true);

        AvisarOtrosEnemigos(owner);
    }

    public override void Execute(NPCBase owner)
    {
        timer += Time.deltaTime;

        if (timer >= warningDuration)
            owner.FSM.TriggerEvent(StateEvent.AlertTimeout);
    }

    public override void Exit(NPCBase owner)
    {
        var cam = owner as SecurityCamNPC;
        if (cam?.alarmSound != null)
            cam.alarmSound.Stop();

        // No desactivamos la visión permanentemente (lo hace el estado vigilando si corresponde)
        alertPanel?.HideAlert();
    }

    public override System.Type GetNextStateForEvent(StateEvent evt)
    {
        if (evt == StateEvent.AlertTimeout)
            return typeof(CameraVigilando);

        // Si vuelve a ver al jugador, reinicia la alerta sin cambiar de estado
        //if (evt == StateEvent.PlayerSeen)
          //  return typeof(CamaraWarning);

        return null;
    }

    private void AvisarOtrosEnemigos(NPCBase owner)
    {
        var cam = owner as SecurityCamNPC;
        if (cam == null) return;

        float radioAviso = cam.alertRadius; // configurable desde el inspector
        Collider[] enemigos = Physics.OverlapSphere(owner.transform.position, radioAviso);

        foreach (Collider col in enemigos)
        {
            if (col.gameObject == owner.gameObject) continue; // Evitar autoaviso

            NPCBase npc = col.GetComponent<NPCBase>();
            if (npc == null) continue;

            // Solo avisar a enemigos relevantes
            if (col.CompareTag("Dog") || col.CompareTag("Guard") )
            {
                string stateName = npc.FSM?.getState()?.GetType().Name ?? "";

                // Solo avisar si estan en modo tranquilo
                if (stateName == "PatrolState")
                {
                    npc.LastHeardPosition = owner.transform.position;
                    npc.FSM.TriggerEvent(StateEvent.SCAlerted);
                    Debug.Log($"📢 {owner.name} avisó a {npc.name} ({stateName})");
                }
            }
        }
    }
}
