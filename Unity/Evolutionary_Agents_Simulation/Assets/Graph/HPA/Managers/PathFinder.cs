using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class PathFinder : IPathFinder
{



    public List<HPANode> FindAbstractPath(HPANode start, HPANode goal, int level)
    {
        // Implementation of A* search algorithm to find a path in the abstract graph
        // You can use any standard A* implementation or library
        // The search should be performed on the abstract graph at the specified level
        // Return the list of HPANodes representing the abstract path from start to goal
        // Example implementation using a placeholder A* algorithm
        List<HPANode> abstractPath = Astar.FindPath(start, goal, HPAEdgeType.INTER);
        return abstractPath;
    }

    public List<HPANode> RefinePath(List<HPANode> abstractPath, int level)
    {
        if (abstractPath == null || abstractPath.Count == 0)
        {
            Debug.LogError("Abstract path is null or empty.");
            return new List<HPANode>();
        }

        List<HPANode> refinedPath = new List<HPANode>();

        for (int i = 0; i < abstractPath.Count - 1; i++)
        {
            HPANode startNode = abstractPath[i];
            HPANode endNode = abstractPath[i + 1];

            if (startNode == null || endNode == null)
            {
                Debug.LogError($"Null node encountered in abstract path at indices {i} and {i + 1}.");
                continue;
            }

            if (startNode.Cluster == null || endNode.Cluster == null)
            {
                Debug.LogError("One of the nodes has a null cluster.");
                continue;
            }

            if (startNode.Cluster == endNode.Cluster)
            {
                List<HPANode> localPath = FindLocalPath(startNode, endNode, startNode.Cluster);
                if (localPath == null || localPath.Count == 0)
                {
                    Debug.LogError("Failed to find local path within cluster for nodes: " + startNode.Id + " to " + endNode.Id);
                    continue;
                }
                refinedPath.AddRange(localPath);
            }
            else
            {
                Entrance entrance = FindEntrance(startNode.Cluster, endNode.Cluster);
                if (entrance == null || entrance.Node1 == null || entrance.Node2 == null)
                {
                    Debug.LogError("No valid entrance or null nodes in entrance between clusters: " + startNode.Cluster.bottomLeftPos + " and " + endNode.Cluster.bottomLeftPos);
                    continue;
                }

                // Debug.Log("trying to find path to entrance");
                List<HPANode> pathToEntrance = FindLocalPath(startNode, entrance.Node1, startNode.Cluster);
                if (pathToEntrance == null || pathToEntrance.Count == 0)
                {
                    Debug.LogError("Failed to find path to entrance for startNode.");
                    continue;
                }
                refinedPath.AddRange(pathToEntrance);

                List<HPANode> pathFromEntrance = FindLocalPath(entrance.Node2, endNode, endNode.Cluster);
                if (pathFromEntrance == null || pathFromEntrance.Count == 0)
                {
                    Debug.LogError("Failed to find path from entrance for endNode.");
                    continue;
                }
                if (pathFromEntrance.Count > 0 && pathFromEntrance.First() == entrance.Node2)
                    pathFromEntrance.RemoveAt(0);
                refinedPath.AddRange(pathFromEntrance);
            }
        }

        if (refinedPath.LastOrDefault() != abstractPath.Last())
        {
            if (refinedPath.Count > 0 && refinedPath.Last().Cluster == abstractPath.Last().Cluster)
            {
                List<HPANode> finalSegment = FindLocalPath(refinedPath.Last(), abstractPath.Last(), abstractPath.Last().Cluster);
                if (finalSegment != null && finalSegment.Count > 0)
                {
                    if (finalSegment.First() == refinedPath.Last())
                        finalSegment.RemoveAt(0);
                    refinedPath.AddRange(finalSegment);
                }
                else
                {
                    Debug.LogError("No valid path to final node found.");
                }
            }
            else
            {
                refinedPath.Add(abstractPath.Last());
            }
        }

        return refinedPath;
    }

    public List<HPANode> FindLocalPath(HPANode startNode, HPANode endNode, Cluster cluster)
    {
        if (startNode.Cluster != cluster || endNode.Cluster != cluster)
        {
            return null;
        }

        List<HPANode> localPath = Astar.FindPath(startNode, endNode, HPAEdgeType.INTRA);
        return localPath;
    }

    private Entrance FindEntrance(Cluster startCluster, Cluster endCluster)
    {
        foreach (Entrance entrance in startCluster.Entrances)
        {
            if ((entrance.Cluster1 == startCluster && entrance.Cluster2 == endCluster) ||
                (entrance.Cluster1 == endCluster && entrance.Cluster2 == startCluster))
            {
                return entrance;
            }
        }
        return null;
    }

    public double CalculateDistance(List<HPANode> path)
    {
        // Implementation to calculate the distance of a path
        // This method should calculate the total distance of the given path
        // You can sum the distances between consecutive points in the path
        // Return the total distance of the path

        if (path == null)
        {
            return double.PositiveInfinity;
        }
        double distance = 0;

        for (int i = 0; i < path.Count - 1; i++)
        {
            if (path[i] == null || path[i + 1] == null)
            {
                return double.PositiveInfinity;
            }
            distance += Vector2Int.Distance(path[i].Position, path[i + 1].Position);
        }

        return distance;

    }


}