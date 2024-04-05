
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cluster
{
    public Guid Id { get; set; }
    public int Level { get; set; }

    public Vector2Int bottomLeftPos { get; set; }
    public Vector2Int topRightPos { get; set; }
    public HashSet<HPANode> Nodes { get; set; }


    public HashSet<Entrance> Entrances { get; set; }

    public Cluster(int level, HashSet<HPANode> hPANodes, HashSet<Entrance> entrances, Vector2Int bottomLeftPos, Vector2Int topRightPos)
    {


        Id = Guid.NewGuid();
        Level = level;
        Nodes = hPANodes;
        Entrances = entrances;

        this.bottomLeftPos = bottomLeftPos;
        this.topRightPos = topRightPos;
    }

    public bool Contains(Vector2Int position)
    {
        return position.x >= bottomLeftPos.x && position.x <= topRightPos.x && position.y >= bottomLeftPos.y && position.y <= topRightPos.y;

    }

    public static HPAEdge[,] ConvertGridToAdjacencyMatrix(int[,] map, Dictionary<Vector2Int, HPANode> tileToHPANode)
    {
        int size = tileToHPANode.Count;
        // Assuming tileToHPANode contains all walkable nodes, the matrix size should match the number of nodes
        HPAEdge[,] adjacencyMatrix = new HPAEdge[size, size];

        foreach (var entry in tileToHPANode)
        {
            Vector2Int currentPosition = entry.Key;
            HPANode currentNode = entry.Value;

            // Directions for N, S, E, W
            Vector2Int[] directions = {
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0)
            // Include diagonals if desired
        };

            foreach (Vector2Int direction in directions)
            {
                Vector2Int neighborPosition = currentPosition + direction;
                if (tileToHPANode.TryGetValue(neighborPosition, out HPANode neighborNode))
                {
                    // Assuming index mapping is sequential and based on the order of insertion into tileToHPANode
                    int currentIndex = GetIndexFromPosition(currentPosition, tileToHPANode);
                    int neighborIndex = GetIndexFromPosition(neighborPosition, tileToHPANode);

                    // Create edge only if both nodes exist and are walkable
                    adjacencyMatrix[currentIndex, neighborIndex] = new HPAEdge(node1: currentNode, node2: neighborNode, weight: 1, level: 0, type: HPAEdgeType.INTRA);
                }
            }
        }

        return adjacencyMatrix;
    }

    private static int GetIndexFromPosition(Vector2Int position, Dictionary<Vector2Int, HPANode> tileToHPANode)
    {
        // Assuming tileToHPANode.Values is ordered or you have a mechanism to map positions to sequential indices
        // You might need to adjust this based on how you manage node indices
        return Array.IndexOf(tileToHPANode.Values.ToArray(), tileToHPANode[position]);
    }


}