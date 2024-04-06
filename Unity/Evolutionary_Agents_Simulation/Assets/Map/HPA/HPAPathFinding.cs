using System;
using System.Collections.Generic;
using UnityEngine;

public class HPAPathfinding
{

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int end, int[,] tileMap)
    {

        HPAStarGraphConstruction hpaGraph = new HPAStarGraphConstruction(tileMap);
        hpaGraph.Preprocessing(1); // Specify the maximum level for preprocessing

        Debug.Log("end graph build ");

        Debug.Log(hpaGraph.ClusterByLevel[1].Count);




        // Insert start and end nodes into the abstract graph



        // Perform HPA* search


        HPANode startNode = new HPANode(1251251251, null, start, 1);
        HPANode endNode = new HPANode(1251251253, null, end, 1);

        hpaGraph.insertNode(startNode, 1);
        hpaGraph.insertNode(endNode, 1);

        List<HPANode> abstractPath = hpaGraph.searchForPath(start, end, 1);

        if (abstractPath == null)
        {

            throw new Exception("No path found in hierarchicalSearch");

        }
        else
        {
            Debug.Log("Path found");

        }

        // Refine the abstract path
        List<HPANode> detailedPath = hpaGraph.refinePath(abstractPath, 1);

        // Convert HPANodes back to tile map coordinates
        List<Vector2Int> tilePath = new List<Vector2Int>();
        foreach (HPANode node in detailedPath)
        {
            tilePath.Add(node.Position);


        }

        if (detailedPath == null)
        {
            throw new Exception("No detailed path found in hierarchicalSearch");
        }
        else
        {
            foreach (HPANode node in detailedPath)
            {
                Debug.Log("refined path: " + node.Position);
            }
            Debug.Log("Detailed path found");

        }


        return abstractPath.ConvertAll(node => node.Position);
    }
}