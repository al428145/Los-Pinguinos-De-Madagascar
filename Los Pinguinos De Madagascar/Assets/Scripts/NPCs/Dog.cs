using UnityEngine;

public class Dog : NPCBase
{
    public enum DogState
    {
        Patrol,
        Alerted,
        Investigate
    }

    [Header("Zona de Patrulla")]
    public BoxCollider zonaPatrulla;
    private Vector3 destinoRandom;
    public DogState state = DogState.Patrol;
    private Vector3 noiseSource;
    private bool playerStillInRange;
    private float alertTimer;

    void Start()
    {
        ElegirNuevoDestino();
    }

    void Update()
    {
        switch (state)
        {
            case DogState.Patrol:
                MoverHacia(destinoRandom);

                if (Vector3.Distance(transform.position, destinoRandom) < distanciaMinima)
                {
                    ElegirNuevoDestino();
                }
                break;

            case DogState.Alerted:
                alertTimer += Time.deltaTime;

                LookAtNoise(noiseSource);
                if (alertTimer >= 2f && !playerStillInRange)
                {
                    state = DogState.Patrol;
                    alertTimer = 0;
                    ElegirNuevoDestino();
                }

                else if (alertTimer >= 2f && playerStillInRange)
                {
                    Debug.Log($"{gameObject.name} pasa a investigar!");
                    state = DogState.Investigate;
                    alertTimer = 0;
                }
                break;

            case DogState.Investigate:
                break;
        }
        playerStillInRange = false;
    }

    void ElegirNuevoDestino()
    {
        if (zonaPatrulla == null) return;

        // Generar punto aleatorio dentro de la zona
        Vector3 centro = zonaPatrulla.center + zonaPatrulla.transform.position;
        Vector3 tamanio = zonaPatrulla.size / 2;

        float x = Random.Range(centro.x - tamanio.x, centro.x + tamanio.x);
        float z = Random.Range(centro.z - tamanio.z, centro.z + tamanio.z);
        destinoRandom = new Vector3(x, transform.position.y, z);
    }

    public override void HandleNoise(Vector3 noisePosition)
    {
        base.HandleNoise(noisePosition);

        noiseSource = noisePosition;
        playerStillInRange = true;
        if (state != DogState.Alerted)
        {
            state = DogState.Alerted;
            alertTimer = 0f;
        }
    }
}
