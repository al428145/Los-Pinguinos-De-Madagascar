using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guardia : NPCBase
{
    [Header("Puntos de Patrulla")]
    public Transform[] puntosDePatrulla;
    private int indiceActual = 0;

    // Update is called once per frame
    void Update()
    {
        if (puntosDePatrulla.Length == 0) return;

        destinoActual = puntosDePatrulla[indiceActual].position;
        MoverHacia(destinoActual);

        if (Vector3.Distance(transform.position, destinoActual) < distanciaMinima)
        {
            indiceActual = (indiceActual + 1) % puntosDePatrulla.Length;
        }

    }
}
