using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Controller;

public class SurfaceZone : MonoBehaviour
{
    private int asphaltType = 1;
    private int landType = 0;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            var player = other.GetComponent<MovePlayerInput>();
            if(player != null)
                player.setSurfaceZona(asphaltType);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            var player = other.GetComponent<MovePlayerInput>();
            if(player != null)
                player.setSurfaceZona(landType);
        }
    }
}
