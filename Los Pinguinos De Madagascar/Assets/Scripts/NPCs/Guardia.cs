using System.Collections.Generic;
using UnityEngine;

public class Guard : NPCBase
{
    [Header("Puntos de patrulla")]
    public List<Waypoint> puntosDePatrulla;
    private int indiceActual = 0;

    [Header("🔊 Sonido de alerta")]
    public AudioSource alertSound;
    [Range(0f, 1f)] public float alertVolume = 1f;
    [Header("UI de investigación")]
    public GameObject investigateIcon; // asigna la imagen en el inspector




    protected override void Awake()
    {
        base.Awake();
        FSM = new StateMachine(this, new System.Type[]
        {
            typeof(PatrolState),
            typeof(AlertedState),
            typeof(InvestigateState),
            typeof(PersecuteState),
            typeof(returnPatrolState),
            typeof(callAlertedState)
        });
        if (alertSound != null)
        {
            alertSound.playOnAwake = false;
            alertSound.loop = false;      
            alertSound.volume = alertVolume;
        }
    }

    void Start()
    {
        SelectNewDestination();
        FSM.SetState(typeof(PatrolState));
    }

    void Update()
    {
        FSM.Update();
        PlayerStillInRange = false;
        PlayerIsBeingSeen = false;
    }

    public override void SelectNewDestination()
    {
        if (puntosDePatrulla.Count == 0) return;

        CurrentDestination = puntosDePatrulla[indiceActual].transform.position;
        indiceActual = (indiceActual + 1) % puntosDePatrulla.Count;
    }
    // 👇 Opcional: método para reproducir el sonido de alerta desde cualquier estado
    public void PlayAlertSound()
    {
        if (alertSound != null)
        {
            alertSound.volume = alertVolume;
            alertSound.Play();
        }
    }

    public void StopAlertSound()
    {
        if (alertSound != null && alertSound.isPlaying)
            alertSound.Stop();
    }
}
