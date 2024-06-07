// written by: Gustav Clausen s214940
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class PathFinder : IPathFinder
{
    private readonly IGraphModel _graphModel;

    public PathFinder(IGraphModel graphModel)
    {
        _graphModel = graphModel;
    }






    public HPAPath FindAbstractPath(HPANode start, HPANode goal, int level)
    {
        HPAPath abstractPath = Astar.FindPath(start, goal, HPAEdgeType.INTER);
        return abstractPath;
    }


    public HPAPath RefinePath(HPAPath abstractPath, int initialLevel)
    {
        if (abstractPath == null || abstractPath.path.Count == 0)
        {
            Debug.LogError("Abstract path is null or empty.");
            return new HPAPath(new List<HPANode>());
        }

        HPAPath refinedPath = new HPAPath(new List<HPANode>());
        Stack<(HPANode, HPANode, int)> stack = new Stack<(HPANode, HPANode, int)>();

        // Initialize the stack with the abstract path nodes and initial level
        for (int i = abstractPath.path.Count - 1; i > 0; i--)
        {
            stack.Push((abstractPath.path[i - 1], abstractPath.path[i], initialLevel));
        }

        while (stack.Count > 0)
        {
            var (startNode, endNode, level) = stack.Pop();

            if (startNode == null || endNode == null)
            {
                Debug.LogError("Null node encountered in stack.");
                continue;
            }

            if (NeighborCheck(startNode, endNode))
            {
                refinedPath.path.Add(startNode);
                continue;
            }

            if (level > 1)
            {
                // Finding paths at a lower level. 
                if (_graphModel.NodesByLevel[level - 1].TryGetValue(startNode.Position, out HPANode lowerStartNode) &&
                    _graphModel.NodesByLevel[level - 1].TryGetValue(endNode.Position, out HPANode lowerEndNode))
                {
                    Debug.Log($"Refining path between {startNode.Position} and {endNode.Position} at level {level - 1}");

                    HPAPath lowerLevelPath = FindAbstractPath(lowerStartNode, lowerEndNode, level - 1);
                    if (lowerLevelPath != null && lowerLevelPath.path.Count > 0)
                    {
                        for (int i = lowerLevelPath.path.Count - 1; i > 0; i--)
                        {
                            stack.Push((lowerLevelPath.path[i - 1], lowerLevelPath.path[i], level - 1));
                        }
                    }
                    else
                    {
                        Debug.LogError($"Failed to find abstract path between {lowerStartNode.Position} and {lowerEndNode.Position} at level {level - 1}");
                    }
                }
                else
                {
                    Debug.LogError($"Failed to find nodes at level {level - 1} for positions {startNode.Position} and {endNode.Position}");
                }
            }
            else
            {
                // Find local path within the same cluster at level 1
                HPAPath localPath = FindLocalPath(startNode, endNode, startNode.Cluster);
                if (localPath != null && localPath.path.Count > 0)
                {
                    Debug.Log($"Adding local path directly between {startNode.Position} and {endNode.Position} at level 1");
                    refinedPath.AddRange(localPath);
                }
                else
                {
                    Debug.LogError($"Local path not found between {startNode.Position} and {endNode.Position} at level 1");
                }
            }
        }

        // Add the last node of the abstract path to the refined path
        if (abstractPath.path.Count > 0)
        {
            refinedPath.path.Add(abstractPath.path.Last());
        }

        return refinedPath;
    }



    private static bool NeighborCheck(HPANode startNode, HPANode endNode)
    {
        if (startNode.Position - endNode.Position == Vector2Int.down || startNode.Position - endNode.Position == Vector2Int.up || startNode.Position - endNode.Position == Vector2Int.left || startNode.Position - endNode.Position == Vector2Int.right)
        {
            return true;
        }
        return false;
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

    private HPAPath FindInterEdge(HPANode start, HPANode end)
    {
        foreach (var edge in start.Edges)
        {
            if (edge.Type == HPAEdgeType.INTER && (edge.Node1 == end || edge.Node2 == end))
            {
                return edge.IntraPaths;
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