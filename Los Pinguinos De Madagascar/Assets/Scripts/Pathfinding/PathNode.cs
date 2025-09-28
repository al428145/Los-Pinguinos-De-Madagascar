using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode : MonoBehaviour
{
    public Waypoint waypoint;
    public PathNode parent;
    public float gCost;
    public float hCost;
    public float fCost;

    public PathNode(Waypoint wp)
    {
        waypoint = wp;
    }
}
