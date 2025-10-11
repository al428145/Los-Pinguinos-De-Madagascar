using System.Collections;
using UnityEngine;

public class ChickenAvisandoState : State
{
    private bool avisando;
    private float timer;

    public override void Enter(NPCBase owner)
    {
        avisando = false;
        timer = 0f;

        var renderer = owner.GetComponent<Renderer>();
        if (renderer != null) renderer.material.color = Color.red;

        // Mirar hacia el jugador si tiene referencia
        if (owner.LastHeardPosition != Vector3.zero)
            owner.LookAtNoise(owner.LastHeardPosition);

        // Reproducir animación o sonido de aviso
        if (owner.animator != null)
            owner.animator.SetTrigger("Avisar");

        // Avisar a los enemigos cercanos
        AvisarOtrosEnemigos(owner);
    }

    public override void Execute(NPCBase owner)
    {
        timer += Time.deltaTime;

        // Mantener mirada hacia la posición del jugador
        if (owner.LastHeardPosition != Vector3.zero)
            owner.LookAtNoise(owner.LastHeardPosition);

        // Esperar unos segundos antes de pasar a alerta
        if (timer >= 2f)
            owner.FSM.TriggerEvent(StateEvent.AlertTimeout);
    }

    public override System.Type GetNextStateForEvent(StateEvent evt)
    {
        if (evt == StateEvent.AlertTimeout)
            return typeof(ChickenAlertState);
        return null;
    }

    private void AvisarOtrosEnemigos(NPCBase owner)
    {
        Gallina gallina = owner as Gallina;
        if (gallina == null) return;

        Collider[] enemigos = Physics.OverlapSphere(owner.transform.position, gallina.radioAviso, gallina.layerEnemigos);

        foreach (Collider col in enemigos)
        {
            NPCBase npc = col.GetComponent<NPCBase>();
            if (npc != null && npc != owner)
            {
                npc.HandleNoise(owner.LastHeardPosition);
            }
        }
    }
}
