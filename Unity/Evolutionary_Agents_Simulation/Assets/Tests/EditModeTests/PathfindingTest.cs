using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class PathfindingTest
{



    // A Test behaves as an ordinary method
    [Test]
    public void PathfindingTestSimplePasses()
    {

        RunHPASimulation(
            iterations: 100,
            densityRange: (min: 45, max: 65),
            checkPointsRange: (min: 1, max: 5),
            cellularIterationsRange: (min: 10, max: 10),
            heightRange: (min: 100, max: 100),
            widthRange: (min: 100, max: 100),
            name: "density"
        );
        // Create instances of the necessary classes


        // InitCustomMaps mapModel = new InitCustomMaps(density: 0, numberOfCheckPoints: 0, cellularIterations: 0, height: 100, width: 100);


        // int[,] tileMap = CellularAutomata.Create2DMap(mapModel.height, mapModel.width, mapModel.density, mapModel.cellularIterations, 4);


        // Vector2Int start = new Vector2Int(0, 0);
        // Vector2Int end = new Vector2Int(mapModel.height - 1, mapModel.width - 1);



        // HPAPath hpaPath = EffiencyInNodeExploration(tileMap, start, end, 1, RefinePath: false);

        // Debug.Log("HPA STAR Abstract level 1 : ");

        // Debug.Log("Path Length: " + hpaPath.path.Count);
        // Debug.Log("Nodes Explored: " + hpaPath.NodesExplored);


        // HPAPath hpaPath3 = EffiencyInNodeExploration(tileMap, start, end, 2, RefinePath: false);

        // Debug.Log("HPA STAR Abstract level 2 : ");

        // Debug.Log("Path Length: " + hpaPath3.path.Count);
        // Debug.Log("Nodes Explored: " + hpaPath3.NodesExplored);



        // HPAPath hpaPath4 = EffiencyInNodeExploration(tileMap, start, end, 3, RefinePath: false);

        // Debug.Log("HPA STAR Abstract level 3 : ");

        // Debug.Log("Path Length: " + hpaPath4.path.Count);
        // Debug.Log("Nodes Explored: " + hpaPath4.NodesExplored);


        // HPAPath hpaPath2 = EffiencyInNodeExploration(tileMap, start, end, 3, RefinePath: true);
        // Debug.Log("HPA STAR: ");
        // Debug.Log("Path Length: " + hpaPath2.path.Count);
        // Debug.Log("Nodes Explored: " + hpaPath2.NodesExplored);







        // Debug.Log("A* : ");

        // (List<Vector2Int> Path, int NodesExplored) Apath = Astar.FindPath(start, end, tileMap);

        // Debug.Log("Path Length: " + Apath.Path.Count);
        // Debug.Log("Nodes Explored: " + Apath.NodesExplored);



        // for (int i = 0; i < hpaPath2.path.Count; i++)
        // {
        //     Debug.Log("HPA Path: " + hpaPath2.path[i].Position);
        //     if (Apath.Path.Count > i)
        //         Debug.Log("A* Path: " + Apath.Path[i]);
        // }



        // Effiency in node exploration. 


    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator PathfindingTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }


    private static void RunHPASimulation(int iterations, (int min, int max) densityRange, (int min, int max) checkPointsRange, (int min, int max) cellularIterationsRange, (int min, int max) heightRange, (int min, int max) widthRange, string name)
    {
        var random = new System.Random();
        var results = new List<string> { "Iteration,Density,CheckPoints,CellularIterations,Height,Width,PathLength_Level1,NodesExplored_Level1,PathLength_Level2,NodesExplored_Level2,PathLength_Level3,NodesExplored_Level3,PathLength_AStar,NodesExplored_AStar" };

        for (int i = 0; i < iterations; i++)
        {

            int density = random.Next(densityRange.min, densityRange.max);
            int numberOfCheckPoints = random.Next(checkPointsRange.min, checkPointsRange.max);
            int cellularIterations = random.Next(cellularIterationsRange.min, cellularIterationsRange.max);
            int height = random.Next(heightRange.min, heightRange.max);
            int width = random.Next(widthRange.min, widthRange.max);

            InitCustomMaps mapModel = new InitCustomMaps(density: density, numberOfCheckPoints: numberOfCheckPoints, cellularIterations: cellularIterations, height: height, width: width);
            int[,] tileMap = CellularAutomata.Create2DMap(mapModel.height, mapModel.width, mapModel.density, mapModel.cellularIterations, 4);
            Vector2Int start = new Vector2Int(10, 10);
            Vector2Int end = new Vector2Int(mapModel.height - 10, mapModel.width - 10);

            // HPA* Pathfinding at different abstraction levels
            HPAPath hpaPath1 = EffiencyInNodeExploration(tileMap, start, end, 1, RefinePath: false);
            HPAPath hpaPath2 = EffiencyInNodeExploration(tileMap, start, end, 2, RefinePath: false);
            HPAPath hpaPath3 = EffiencyInNodeExploration(tileMap, start, end, 3, RefinePath: false);

            // A* Pathfinding
            (List<Vector2Int> Path, int NodesExplored) Apath = Astar.FindPath(start, end, tileMap);




            string result = $"{i + 1}," +
                       $"{density}," +
                       $"{numberOfCheckPoints}," +
                       $"{cellularIterations}," +
                       $"{height}," +
                       $"{width}," +
                       $"{(hpaPath1?.path?.Count ?? 0)}," +
                       $"{(hpaPath1?.NodesExplored ?? 0)}," +
                       $"{(hpaPath2?.path?.Count ?? 0)}," +
                       $"{(hpaPath2?.NodesExplored ?? 0)}," +
                       $"{(hpaPath3?.path?.Count ?? 0)}," +
                       $"{(hpaPath3?.NodesExplored ?? 0)}," +
                       $"{(Apath.Path?.Count ?? 0)}," +
                       $"{(Apath.NodesExplored)}";

            results.Add(result);
        }
        name = "simulation_results_hpa_" + name + ".csv";

        string filePath = Path.Combine(Application.dataPath, name);
        File.WriteAllLines(filePath, results);
        Debug.Log($"Results saved to {filePath}");

    }


    public static HPAPath EffiencyInNodeExploration(int[,] tileMap, Vector2Int start, Vector2Int end, int maxLevel, Boolean RefinePath = false)
    {
        PathFinder _pathFinder = new PathFinder();
        GraphModel _graphModel = new GraphModel(tileMap);
        IEdgeManager edgeManager = new EdgeManager(_pathFinder);
        NodeManager _nodeManager = new NodeManager(_graphModel, edgeManager);
        IEntranceManager entranceManager = new EntranceManager(_graphModel, _nodeManager);
        IClusterManager clusterManager = new ClusterManager(_graphModel, _nodeManager, edgeManager, entranceManager);
        IHPAStar HPAStar = new HPAStar(_graphModel, clusterManager, _nodeManager, entranceManager, edgeManager, _pathFinder);

        HPAStar.Preprocessing(maxLevel);
        if (RefinePath)
        {
            HPAPath hpaPath = HPAStar.HierarchicalSearch(start, end, maxLevel);
            return hpaPath;
        }
        else
        {
            HPAPath hpaPath = HPAStar.HierarchicalAbstractSearch(start, end, maxLevel);
            return hpaPath;
        }

    }





}

