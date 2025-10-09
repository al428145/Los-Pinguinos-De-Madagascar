using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gallina : NPCBase
{
    public enum EstadoGallina
    {
        Dormida,
        Avisando,
        Alerta
    }

    [Header("Estados")]
    [SerializeField] private EstadoGallina estadoActual = EstadoGallina.Dormida;

    [Header("Detecci�n y Aviso")]
   
    [SerializeField] private float radioAviso = 5f; 
    [SerializeField] private float tiempoAvisando = 2f; 
    [SerializeField] private LayerMask layerEnemigos; 

    private EnemyNoiseDetector noiseDetector;
    private Transform playerTransform;
    private Renderer rend;
     private bool estaAvisando = false;
    protected override void Awake()
    {
        base.Awake();
        rend = GetComponent<Renderer>();
        noiseDetector = GetComponent<EnemyNoiseDetector>();

        if (noiseDetector != null)
        {
            noiseDetector.OnNoiseHeard += OnPlayerDetectado;
        }

      
        if (noiseDetector != null && noiseDetector.player != null)
        {
            playerTransform = noiseDetector.player;
        }
    }

    private void Update()
    {
        switch (estadoActual)
        {
            case EstadoGallina.Dormida:
                rend.material.color = Color.blue;   // Dormida
                EstadoDormida();
                break;
            case EstadoGallina.Avisando:
                rend.material.color = Color.red;    // Avisando
                EstadoAvisando();
                break;
            case EstadoGallina.Alerta:
                rend.material.color = Color.yellow; // Alerta
                EstadoAlerta();
                break;
        }
    }

    private void EstadoDormida()
    {
        // cambiar esto para que se muestre el sistema particulas
        if (animator != null)
            animator.SetFloat("Speed", speeds.GetSpeed(MovementType.Idle));
    }

  private void EstadoAvisando()
{
    if (playerTransform == null) return;

    LookAtNoise(playerTransform.position);

    if (!estaAvisando)
    {
        estaAvisando = true;

        // Hacer alg�n sonido o animaci�n aqu�
        AvisarOtrosEnemigos();

        StartCoroutine(CambiarEstadoDespuesDe(tiempoAvisando, EstadoGallina.Alerta));
    }
}


    private void EstadoAlerta()
    {
        // Podr�a moverse lentamente o mirar alrededor
        // Por ahora, simplemente idle con animaci�n de alerta
        if (animator != null)
            animator.SetBool("Alerta", true);
    }


    private void OnPlayerDetectado(Vector3 posicionJugador)
    {
        if (estadoActual == EstadoGallina.Dormida || estadoActual == EstadoGallina.Alerta)
        {
            estadoActual = EstadoGallina.Avisando;
        }
    }

    private void AvisarOtrosEnemigos()
    {
        Collider[] enemigos = Physics.OverlapSphere(transform.position, radioAviso, layerEnemigos);

        foreach (Collider col in enemigos)
        {
            NPCBase npc = col.GetComponent<NPCBase>();
            if (npc != null && npc != this)
            {
                npc.HandleNoise(playerTransform.position);
            }
        }
    }

    private IEnumerator CambiarEstadoDespuesDe(float segundos, EstadoGallina nuevoEstado)
    {
        yield return new WaitForSeconds(segundos);
        estadoActual = nuevoEstado;
        estaAvisando = false; // <- resetear el flag
    }


    public override void HandleNoise(Vector3 noisePosition)
    {
        // Si otro enemigo la alerta
        if (estadoActual == EstadoGallina.Dormida)
        {
            estadoActual = EstadoGallina.Avisando;
        }
    }

    public override void HandleVision(Vector3 playerPosition)
    {
        OnPlayerDetectado(playerPosition);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioAviso);
    }

    public override void SelectNewDestination()
    {
        throw new System.NotImplementedException();
    }
}
