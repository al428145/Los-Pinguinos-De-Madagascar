using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public static List<Waypoint> FindPath(Waypoint origin, Waypoint target)
    {
        //List of open nodes and closed one
        List<PathNode> openList = new List<PathNode>();
        List<Waypoint> closeList = new List<Waypoint>();

        //Inicial mode
        PathNode startNode = new PathNode(origin);
        startNode.gCost = 0;
        startNode.hCost = Vector3.Distance(origin.position, target.position);
        startNode.fCost = startNode.gCost + startNode.hCost;
        startNode.parent = null;
        openList.Add(startNode);

        while(openList.Count > 0)
        {
            //Select the node with less fCost
            PathNode currrentNode = openList.OrderBy(n => n.fCost).First();

            if(currrentNode == target)
                return ReconstructPath(currrentNode);

            openList.Remove(currrentNode);
            closeList.Add(currrentNode.waypoint);

            //Explore neighbors
            foreach(Waypoint neighbor in currrentNode.waypoint.neighbors)
            {
                if(closeList.Contains(neighbor))
                    continue;
                
                float tentativeG = currrentNode.gCost + Vector3.Distance(currrentNode.waypoint.position, neighbor.position);
                PathNode neighborNode = openList.FirstOrDefault(n=> n.waypoint == neighbor);

                if(neighborNode == null)
                {
                    neighborNode = new PathNode(neighbor);
                    openList.Add(neighborNode);
                }
                else if(tentativeG >= neighborNode.gCost)
                    continue;
                
                neighborNode.parent = currrentNode;
                neighborNode.gCost = tentativeG;
                neighborNode.hCost = Vector3.Distance(neighbor.position, target.position);
                neighborNode.fCost = neighborNode.gCost + neighborNode.hCost;
            }
        }

        return new List<Waypoint>();
    }

    private static List<Waypoint> ReconstructPath(PathNode current)
    {
        List<Waypoint> path = new List<Waypoint>();

        while(current !=null)
        {
            path.Add(current.waypoint);
            current = current.parent;
        }

        path.Reverse();

        return path;
    }
}
