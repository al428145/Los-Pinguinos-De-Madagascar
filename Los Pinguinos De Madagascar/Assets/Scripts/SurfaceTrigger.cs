using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceTrigger : MonoBehaviour
{
    private int earth;
    private int asphalt;
   
    void Start()
    {
        earth = 0;
        asphalt = 1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var input = other.GetComponent<Controller.MovePlayerInput>();
            if(input != null)
                input.setSurface(asphalt);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var input = other.GetComponent<Controller.MovePlayerInput>();
            if(input != null)
                input.setSurface(earth);
        }
    }
}
