using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gallinasActivas : MonoBehaviour
{
    public GameObject gallina; // Asigna el prefab o el objeto de la escena

    void Start()
    {
        // Activar el script Gallina
        gallina.GetComponent<Gallina>().enabled = true;

        // Asegurarte de que el GameObject también está activo
        gallina.SetActive(true);
    }

}
