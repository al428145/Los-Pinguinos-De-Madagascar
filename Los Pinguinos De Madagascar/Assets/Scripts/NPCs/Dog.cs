using UnityEngine;

public class Dog : NPCBase
{
    public enum DogState
    {
        Patrol,
        Alerted,
        Investigate,
        Persecute
    }

    [Header("Zona de Patrulla")]
    public BoxCollider zonaPatrulla;
    private Vector3 destinoRandom;
    private DogState state = DogState.Patrol;
    private Vector3 noiseSource;
    private bool playerStillInRange;
    private bool playerIsBeingSeen;
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
                if (alertTimer >= 2f && !playerStillInRange && !playerIsBeingSeen)
                {
                    Debug.Log("Pasando a patrullar");
                    state = DogState.Patrol;
                    alertTimer = 0;
                    ElegirNuevoDestino();
                }

                else if (alertTimer >= 2f && (playerStillInRange || playerIsBeingSeen))
                {
                    Debug.Log($"{gameObject.name} pasa a investigar!");
                    state = DogState.Investigate;
                    alertTimer = 0;
                }
                break;

            case DogState.Investigate:
                MoverHacia(noiseSource);
                Vector3 direccion = noiseSource - transform.position;
                direccion.y = 0f;
                float distancia = direccion.magnitude;
                direccion.Normalize();
                if (distancia <= 0.5f)
                {
                    Debug.Log("He llegado a la fuente del ruido");
                    if (playerIsBeingSeen)
                    {
                        //Coments pa mañana: llamar a funcion en npcbase que tenga los nodos que debe seguir
                        // y que los siga en funcion de las diferentes posiciones.
                        state = DogState.Persecute;
                        Debug.Log("Dog pasa a perseguir");
                    }

                    else
                    {
                        //comentarios pa mañana: llamar a corrutina que espere 1 segundo o 2
                        //en NPCBase y que vuelva a patrol.
                        state = DogState.Patrol;
                    }
                }
                break;

            case DogState.Persecute:
                //llamar a funcion de NPCBase que persiga y cambie velocidad
                
                break;
        }
        playerStillInRange = false;
        playerIsBeingSeen = false;
    }

    void ElegirNuevoDestino()
    {
        if (zonaPatrulla == null) return;

        // Generar punto aleatorio dentro de la zona
        Vector3 centro = zonaPatrulla.transform.TransformPoint(zonaPatrulla.center);
        Vector3 tamanio = zonaPatrulla.size / 2;

        float x = Random.Range(-tamanio.x, tamanio.x);
        float z = Random.Range(-tamanio.z, tamanio.z);
        Vector3 puntoLocal = new Vector3(x, 0f, z); 
        Vector3 puntoMundo = zonaPatrulla.transform.TransformPoint(puntoLocal + zonaPatrulla.center);

        destinoRandom = new Vector3(puntoMundo.x, transform.position.y, puntoMundo.z);
        Debug.Log(destinoRandom);
    }

    public override void HandleNoise(Vector3 noisePosition)
    {
        base.HandleNoise(noisePosition);

        playerStillInRange = true;
        if (state == DogState.Patrol)
        {
            state = DogState.Alerted;
            noiseSource = noisePosition;
            alertTimer = 0f;
        }
    }

    public override void HandleVision(Vector3 playerPosition)
    {
        base.HandleVision(playerPosition);

        playerIsBeingSeen = true;
        if (state == DogState.Patrol)
        {
            state = DogState.Alerted;
            noiseSource = playerPosition;
            alertTimer = 0f;
        }
    }
}
