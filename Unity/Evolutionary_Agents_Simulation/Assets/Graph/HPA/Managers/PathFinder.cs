using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class PathFinder : IPathFinder
{



    public HPAPath FindAbstractPath(HPANode start, HPANode goal, int level)
    {
        // Implementation of A* search algorithm to find a path in the abstract graph
        // You can use any standard A* implementation or library
        // The search should be performed on the abstract graph at the specified level
        // Return the list of HPANodes representing the abstract path from start to goal
        // Example implementation using a placeholder A* algorithm
        HPAPath abstractPath = Astar.FindPath(start, goal, HPAEdgeType.INTER);
        return abstractPath;
    }

    // public HPAPath RefinePath(HPAPath abstractPath, int level) //TODO: fetch the intra paths from the inter edges. 
    // {
    //     if (abstractPath == null || abstractPath.path.Count == 0)
    //     {
    //         Debug.LogError("Abstract path is null or empty.");
    //         return new HPAPath(new List<HPANode>());
    //     }

    //     HPAPath refinedPath = new HPAPath(new List<HPANode>());

    //     for (int i = 0; i < abstractPath.path.Count - 1; i++)
    //     {
    //         HPANode startNode = abstractPath.path[i];
    //         HPANode endNode = abstractPath.path[i + 1];

    //         if (startNode == null || endNode == null)
    //         {
    //             Debug.LogError($"Null node encountered in abstract path at indices {i} and {i + 1}.");
    //             continue;
    //         }

    //         if (startNode.Cluster == null || endNode.Cluster == null)
    //         {
    //             Debug.LogError("One of the nodes has a null cluster.");
    //             continue;
    //         }

    //         if (startNode.Cluster == endNode.Cluster)
    //         {
    //             HPAPath localPath = FindLocalPath(startNode, endNode, startNode.Cluster);
    //             if (localPath == null || localPath.path.Count == 0)
    //             {
    //                 Debug.LogError("Failed to find local path within cluster for nodes: " + startNode.Id + " to " + endNode.Id);
    //                 continue;
    //             }
    //             refinedPath.AddRange(localPath);
    //         }
    //         else
    //         {
    //             Entrance entrance = FindEntrance(startNode.Cluster, endNode.Cluster);
    //             if (entrance == null || entrance.Node1 == null || entrance.Node2 == null)
    //             {
    //                 Debug.LogError("No valid entrance or null nodes in entrance between clusters: " + startNode.Cluster.bottomLeftPos + " and " + endNode.Cluster.bottomLeftPos);
    //                 continue;
    //             }

    //             // Debug.Log("trying to find path to entrance");
    //             HPAPath pathToEntrance = FindLocalPath(startNode, entrance.Node1, startNode.Cluster);
    //             if (pathToEntrance == null || pathToEntrance.path.Count == 0)
    //             {
    //                 Debug.LogError("Failed to find path to entrance for startNode.");
    //                 continue;
    //             }
    //             refinedPath.AddRange(pathToEntrance);

    //             HPAPath pathFromEntrance = FindLocalPath(entrance.Node2, endNode, endNode.Cluster);
    //             if (pathFromEntrance == null || pathFromEntrance.path.Count == 0)
    //             {
    //                 Debug.LogError("Failed to find path from entrance for endNode.");
    //                 continue;
    //             }
    //             if (pathFromEntrance.path.Count > 0 && pathFromEntrance.path.First() == entrance.Node2)
    //                 pathFromEntrance.path.RemoveAt(0);
    //             refinedPath.AddRange(pathFromEntrance);
    //         }
    //     }

    //     if (refinedPath.path.LastOrDefault() != abstractPath.path.Last())
    //     {
    //         if (refinedPath.path.Count > 0 && refinedPath.path.Last().Cluster == abstractPath.path.Last().Cluster)
    //         {
    //             HPAPath finalSegment = FindLocalPath(refinedPath.path.Last(), abstractPath.path.Last(), abstractPath.path.Last().Cluster);
    //             if (finalSegment != null && finalSegment.path.Count > 0)
    //             {
    //                 if (finalSegment.path.First() == refinedPath.path.Last())
    //                     finalSegment.path.RemoveAt(0);
    //                 refinedPath.AddRange(finalSegment);
    //             }
    //             else
    //             {
    //                 Debug.LogError("No valid path to final node found.");
    //             }
    //         }
    //         else
    //         {
    //             refinedPath.path.Add(abstractPath.path.Last());
    //         }
    //     }

    //     return refinedPath;
    // }

    public HPAPath RefinePath(HPAPath abstractPath, int level)
    {
        if (abstractPath == null || abstractPath.path.Count == 0)
        {
            // Debug.LogError("Abstract path is null or empty.");
            return new HPAPath(new List<HPANode>());
        }

        HPAPath refinedPath = new HPAPath(new List<HPANode>());

        for (int i = 0; i < abstractPath.path.Count - 1; i++)
        {
            HPANode startNode = abstractPath.path[i];
            HPANode endNode = abstractPath.path[i + 1];

            if (startNode == null || endNode == null)
            {
                // Debug.LogError($"Null node encountered in abstract path at indices {i} and {i + 1}.");
                continue;
            }

            if (startNode.Cluster == endNode.Cluster)
            {
                // Find local path within the same cluster
                HPAPath localPath = FindLocalPath(startNode, endNode, startNode.Cluster);
                refinedPath.AddRange(localPath);
            }
            else
            {
                // Attempt to use precomputed path in HPAInterEdge
                HPAInterEdge interEdge = FindInterEdge(startNode, endNode);
                if (interEdge != null && interEdge.IntraPaths != null)
                {
                    refinedPath.AddRange(interEdge.IntraPaths);
                }
                else
                {
                    // Debug.LogError("No valid HPAInterEdge or local path found between clusters.");
                    continue;
                }
            }
        }

        // Handle final node match
        HPANode lastNodeInRefined = refinedPath.path.LastOrDefault();
        HPANode lastNodeInAbstract = abstractPath.path.Last();
        if (lastNodeInRefined != lastNodeInAbstract)
        {
            if (lastNodeInRefined != null && lastNodeInRefined.Cluster == lastNodeInAbstract.Cluster)
            {
                HPAPath finalSegment = FindLocalPath(lastNodeInRefined, lastNodeInAbstract, lastNodeInAbstract.Cluster);
                refinedPath.AddRange(finalSegment);
            }
            else
            {
                refinedPath.path.Add(lastNodeInAbstract);
            }
        }

        return refinedPath;
    }


    public HPAPath FindLocalPath(HPANode startNode, HPANode endNode, Cluster cluster)
    {
        if (!cluster.Contains(startNode.Position) || !cluster.Contains(endNode.Position))
        {
            // Debug.LogError("Nodes do not belong to the specified cluster.");
            return null;
        }

        HPAPath localPath = Astar.FindPath(startNode, endNode, HPAEdgeType.INTRA);
        return localPath;
    }

    private HPAInterEdge FindInterEdge(HPANode start, HPANode end)
    {
        foreach (var edge in start.Edges)
        {
            if (edge is HPAInterEdge interEdge && (interEdge.Node1 == end || interEdge.Node2 == end))
            {
                return interEdge;
            }
        }
        return null;
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