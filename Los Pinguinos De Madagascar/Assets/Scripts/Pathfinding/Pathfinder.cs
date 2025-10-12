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
            PathNode targetNode = new PathNode(target);

            if(currrentNode.waypoint == targetNode.waypoint)
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

    public static Waypoint FindTheNearestWaypointEnemy(Vector3 enemyPosition, Vector3 playerPosition, List<Waypoint> allWaypoints, float angleThresholdDeg = 60f) // puedes ajustar el ángulo
    {
        if (allWaypoints == null || allWaypoints.Count == 0) return null;

        Vector3 dirToPlayer = (playerPosition - enemyPosition).normalized;
        float minDot = Mathf.Cos(angleThresholdDeg * Mathf.Deg2Rad); // dot mínimo para estar dentro del cono

        Waypoint best = null;
        float bestDistSqr = float.PositiveInfinity;

        // 1) Primer pase: filtrar por estar "delante" (dot >= minDot) y escoger el más cercano
        foreach (var wp in allWaypoints)
        {
            Vector3 toWp = wp.position - enemyPosition;
            float distSqr = toWp.sqrMagnitude;
            if (distSqr == 0f) // caso raro: el waypoint está encima del NPC
            {
                return wp;
            }

            Vector3 dirWp = toWp.normalized;
            float dot = Vector3.Dot(dirToPlayer, dirWp);

            if (dot >= minDot) // está dentro del cono frontal
            {
                if (distSqr < bestDistSqr)
                {
                    bestDistSqr = distSqr;
                    best = wp;
                }
            }
        }

        // 2) Si no hay ninguno en el cono, fallback: el más cercano absoluto
        if (best == null)
        {
            best = allWaypoints.OrderBy(w => (w.position - enemyPosition).sqrMagnitude).FirstOrDefault();
        }

        return best;
    }


    public static Waypoint FindNearestWaypointPlayer(Vector3 playerPos, List<Waypoint> allWaypoints)
    {
        return allWaypoints.OrderBy(w => Vector3.Distance(playerPos, w.position)).FirstOrDefault();
    }
}
