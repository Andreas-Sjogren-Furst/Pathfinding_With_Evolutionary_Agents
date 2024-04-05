using System.Collections.Generic;
using UnityEngine;

public class HPAPathfinding
{

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int end, int[,] tileMap)
    {

        HPAStarGraphConstruction hpaGraph = new HPAStarGraphConstruction(tileMap);
        hpaGraph.Preprocessing(1); // Specify the maximum level for preprocessing

        // Insert start and end nodes into the abstract graph



        // Perform HPA* search


        HPANode startNode = new HPANode(1251251251, null, start, 1);
        HPANode endNode = new HPANode(1251251253, null, end, 1);

        hpaGraph.insertNode(startNode, 1);
        hpaGraph.insertNode(endNode, 1);

        List<HPANode> abstractPath = hpaGraph.searchForPath(start, end, 1);

        // Refine the abstract path
        List<HPANode> detailedPath = hpaGraph.refinePath(abstractPath, 1);

        // Convert HPANodes back to tile map coordinates
        List<Vector2Int> tilePath = new List<Vector2Int>();
        foreach (HPANode node in detailedPath)
        {
            tilePath.Add(node.Position);
        }

        return tilePath;
    }
}