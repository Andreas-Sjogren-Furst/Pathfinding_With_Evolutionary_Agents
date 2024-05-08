using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class MMASController
{

    // public static Graph buildGraphWithAStar(Vector2Int[] checkpointCoordinates, int[,] map)
    // {
    //     Graph graph = new Graph(checkpointCoordinates.Length);



    //     for (int i = 0; i < checkpointCoordinates.Length; i++)
    //     {
    //         Vector2Int checkPoint1 = checkpointCoordinates[i];
    //         Node node = new Node(i, checkPoint1.x, checkPoint1.y);
    //         graph.AddNode(node);

    //         for (int j = 0; j < checkpointCoordinates.Length; j++)
    //         {
    //             Vector2Int checkPoint2 = checkpointCoordinates[j];
    //             //      List<Vector2Int> optPath = Astar.FindPath(checkPoint1, checkPoint2);
    //             //    graph.AddEdge(graph.Nodes[i], graph.Nodes[j], optPath.Count); //TODO: ADD HPA* implementation

    //         }

    //     }

    //     return graph;


    // }
    // public static int[] calculateShortestPathBetweenCheckpoints(Vector2Int[] checkpointCoordinates, int[,] map)
    // {

    //     Graph graph = buildGraphWithAStar(checkpointCoordinates, map);

    //     int numAnts = 52;
    //     double alpha = 1.5;
    //     double beta = 4.5;
    //     double rho = 0.90;
    //     double q = 100.0;
    //     int maxIterations = 500;

    //     MMAS mmas = new MMAS(numAnts, alpha, beta, rho, q);
    //     mmas.SetGraph(graph);
    //     mmas.Run(maxIterations);

    //     int[] bestTour = mmas.GetBestTour();
    //     double bestTourLength = mmas.GetBestTourLength();

    //     UnityEngine.Debug.Log("Best Tour of checkpoints: " + string.Join(" -> ", bestTour));
    //     UnityEngine.Debug.Log("Best Tour Length: " + bestTourLength);

    //     return bestTour; // returns the best tour of checkpoints to visit with their indexes in the checkpointCoordinates array.

    // }
}