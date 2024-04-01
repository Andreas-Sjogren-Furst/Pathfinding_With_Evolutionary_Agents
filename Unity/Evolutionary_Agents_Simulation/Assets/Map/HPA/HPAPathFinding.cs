using System.Collections.Generic;
using UnityEngine;

public class HPAPathfinding
{
    private HPAStarGraphConstruction hpaGraph;
    private Dictionary<Vector2Int, HPANode> tileToHPANode;

    public void Initialize(int[,] tileMap)
    {
        hpaGraph = new HPAStarGraphConstruction();
        tileToHPANode = new Dictionary<Vector2Int, HPANode>();

        // Create HPANodes for each walkable tile
        for (int x = 0; x < tileMap.GetLength(0); x++)
        {
            for (int y = 0; y < tileMap.GetLength(1); y++)
            {
                if (tileMap[x, y] == 0) // Walkable tile
                {
                    HPANode node = new HPANode();
                    node.Position = new Vector2Int(x, y);
                    tileToHPANode[new Vector2Int(x, y)] = node;
                }
            }
        }

        // Create HPAEdges between adjacent HPANodes
        foreach (HPANode node in tileToHPANode.Values)
        {
            Vector2Int[] directions = new Vector2Int[]
            {
                new Vector2Int(-1, 0), // Left
                new Vector2Int(1, 0),  // Right
                new Vector2Int(0, -1), // Down
                new Vector2Int(0, 1)   // Up
            };

            foreach (Vector2Int direction in directions)
            {
                Vector2Int neighborPos = node.Position + direction;
                if (tileToHPANode.ContainsKey(neighborPos))
                {
                    HPANode neighbor = tileToHPANode[neighborPos];
                    // Create an edge between the nodes
                    // You can assign appropriate weights based on your requirements
                    // For simplicity, assuming a weight of 1 for all edges
                    hpaGraph.AddHPAEdge(node, neighbor, 1, 1, HPAEdgeType.INTRA);
                }
            }
        }

        // Perform HPA* preprocessing
        hpaGraph.AbstractMaze();
        hpaGraph.BuildGraph();
        hpaGraph.Preprocessing(1); // Specify the maximum level for preprocessing
    }

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int end)
    {
        HPANode startNode = tileToHPANode[start];
        HPANode endNode = tileToHPANode[end];

        // Insert start and end nodes into the abstract graph
        hpaGraph.insertNode(startNode, 1);
        hpaGraph.insertNode(endNode, 1);

        // Perform HPA* search
        List<HPANode> abstractPath = hpaGraph.searchForPath(startNode, endNode, 1);

        // Refine the abstract path
        List<Vector2Int> detailedPath = hpaGraph.refinePath(abstractPath, 1);

        // Convert HPANodes back to tile map coordinates
        List<Vector2Int> tilePath = new List<Vector2Int>();
        foreach (Vector2Int position in detailedPath)
        {
            tilePath.Add(position);
        }

        return tilePath;
    }
}