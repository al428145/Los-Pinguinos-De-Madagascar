using System.Collections;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class ChickenAvisandoState : State
{
    private bool avisando;
    private float timer;

    public override void Enter(NPCBase owner)
    {
        Debug.Log("Gallina entra en Avisando  :)");
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
        var gallina = owner as Gallina;
        if (gallina != null)
        {
            // Detener partículas de sueño
            if (gallina.particulasDormido != null)
                gallina.particulasDormido.Stop();

            // Reproducir sonido de alerta
            if (gallina.sonidoAlerta != null)
                gallina.sonidoAlerta.Play();
        }
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
    public override void Exit(NPCBase owner)
    {
        var gallina = owner as Gallina;
        if (gallina != null && gallina.sonidoAlerta != null)
            gallina.sonidoAlerta.Stop();

        if (owner.animator != null)
            owner.animator.SetBool("Alerta", false);
    }
    public override System.Type GetNextStateForEvent(StateEvent evt)
    {
        if (evt == StateEvent.AlertTimeout)
            return typeof(ChickenAlertState);

        // Reaccionar también a ruido o visión del jugador
        if ( evt == StateEvent.PlayerHeard || evt == StateEvent.PlayerSeen)
            return typeof(ChickenAvisandoState);
        return null;
    }


    private void AvisarOtrosEnemigos(NPCBase owner)
    {
        Gallina gallina = owner as Gallina;
        if (gallina == null) return;

        float radioAviso = gallina.radioAviso;
        Collider[] enemigos = Physics.OverlapSphere(owner.transform.position, radioAviso);

        foreach (Collider col in enemigos)
        {
            if (col.gameObject == owner.gameObject) continue;

            NPCBase npc = col.GetComponent<NPCBase>();
            if (npc == null) continue;

            // Avisar solo a NPCs relevantes (puedes filtrar por tags)
            if (col.CompareTag("Dog") ||  col.CompareTag("Guard"))
            {
                string stateName = npc.FSM?.getState()?.GetType().Name ?? "";

                // Solo avisar si están en modo tranquilo o dormidos
                if (stateName == "PatrolState" )
                {
                    npc.LastHeardPosition = owner.transform.position;
                    npc.FSM.TriggerEvent(StateEvent.SCAlerted);
                    Debug.Log($"{owner.name} avisó a {npc.name} en estado {stateName}");
                }
            }
        }
    }


}
