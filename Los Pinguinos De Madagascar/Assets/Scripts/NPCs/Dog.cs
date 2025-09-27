using UnityEngine;

public class Dog : NPCBase
{
    [Header("Zona de Patrulla")]
    public BoxCollider zonaPatrulla;
    private Vector3 destinoRandom;

    void Start()
    {
        ElegirNuevoDestino();
    }

    void Update()
    {
        MoverHacia(destinoRandom);

        if (Vector3.Distance(transform.position, destinoRandom) < distanciaMinima)
        {
            ElegirNuevoDestino();
        }
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
}
