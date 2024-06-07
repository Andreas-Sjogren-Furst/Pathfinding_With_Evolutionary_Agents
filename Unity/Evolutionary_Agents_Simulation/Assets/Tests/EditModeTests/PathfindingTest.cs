using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Debug = UnityEngine.Debug;

public class PathfindingTest
{



    // A Test behaves as an ordinary method
    [Test]
    public void PathfindingTestSimplePasses()
    {

        // specific map test as seen in the project. Where we know there is a path
        // / myGameManager.graphController.Preprocessing(3);

        MapModel mapModel = new(60, 17, 100, 20, 1, 0); // 6
        MyGameManager myGameManager = new MyGameManager(mapModel);


        Vector2Int start = new(26, 40);
        Vector2Int end = new(44, 28);


        List<Vector2Int> path = Astar.FindPath(start, end, CellularAutomata.Convert3DTo2D(mapModel.map)).Path;
        HPAPath path1 = myGameManager.HPAGraphController.HierarchicalSearch(start, end, 1);
        HPAPath path2 = myGameManager.HPAGraphController.HierarchicalSearch(start, end, 2);
        HPAPath path3 = myGameManager.HPAGraphController.HierarchicalSearch(start, end, 3);


        Assert.IsNotNull(path);
        Assert.IsNotNull(path1);
        Assert.IsNotNull(path2);
        Assert.IsNotNull(path3);

        Assert.Greater(path.Count, 5);
        Assert.Greater(path1.path.Count, 5);
        Assert.Greater(path2.path.Count, 5);
        Assert.Greater(path3.path.Count, 5);

        // Print results to csv. 
        int iterations = 1;
        List<(int[,], Vector2Int, Vector2Int)> maps = new List<(int[,], Vector2Int, Vector2Int)>();
        for (int i = 0; i < iterations; i++)
        {
            maps.Add(GenerateMapForTests((min: 60, max: 60), (min: 5, max: 20), (min: 17, max: 20), (min: 100, max: 100), (min: 100, max: 100), 0));

        }

        // results will be logged in .csv format. 

        RunHPASimulation(
            mapObjects: maps,
            refinePath: true,
            name: "ClusterSize10_exp3",
            cluserSize: 10,
            randomNumberSeed: 42
        );

        RunHPASimulation(
          mapObjects: maps,
          refinePath: true,
          name: "ClusterSize5_exp3",
          cluserSize: 5,
          randomNumberSeed: 42
      );

        RunDynamicReplanningTests(
            iterations: iterations,
            densityRange: (min: 45, max: 65),
            checkPointsRange: (min: 1, max: 5),
            cellularIterationsRange: (min: 5, max: 10),
            heightRange: (min: 100, max: 100),
            widthRange: (min: 100, max: 100),
            refinePath: true,
            name: "dynamic_maps_timer3"
        );
        // Create instances of the necessary classes


        // InitCustomMaps mapModel = new InitCustomMaps(density: 0, numberOfCheckPoints: 0, cellularIterations: 0, height: 100, width: 100);


        // int[,] tileMap = CellularAutomata.Create2DMap(mapModel.height, mapModel.width, mapModel.density, mapModel.cellularIterations, 4);
        // CellularAutomata.Convert2DTo3D(tileMap);


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


    private static void RunHPASimulation(List<(int[,], Vector2Int, Vector2Int)> mapObjects, bool refinePath, string name, int cluserSize, int randomNumberSeed = 0)
    {
        var random = new System.Random(randomNumberSeed);
        var results = new List<string> { "Iteration,Density,CheckPoints,CellularIterations,Height,Width,PathLength_Level1,NodesExplored_Level1,Time_Level1,PathLength_Level2,NodesExplored_Level2,Time_Level2,PathLength_Level3,NodesExplored_Level3,Time_Level3,PathLength_AStar,NodesExplored_AStar,Time_AStar" };


        for (int i = 0; i < mapObjects.Count; i++)
        {
            MapObject[,] map = CellularAutomata.Convert2DTo3D(mapObjects[i].Item1);
            Vector2Int start = mapObjects[i].Item2;
            Vector2Int end = mapObjects[i].Item3;

            // HPA* Pathfinding at different abstraction levels
            (HPAPath hpaPath1, var timeLevel1) = EffiencyInNodeExploration(map, start, end, 1, RefinePath: refinePath, clusterSize: cluserSize);

            (HPAPath hpaPath2, var timeLevel2) = EffiencyInNodeExploration(map, start, end, 2, RefinePath: refinePath, cluserSize);

            (HPAPath hpaPath3, var timeLevel3) = EffiencyInNodeExploration(map, start, end, 3, RefinePath: refinePath, cluserSize);

            // A* Pathfinding
            var stopwatch = Stopwatch.StartNew();
            (List<Vector2Int> Path, int NodesExplored) Apath = Astar.FindPath(start, end, mapObjects[i].Item1);
            stopwatch.Stop();
            var timeAStar = stopwatch.ElapsedMilliseconds;

            string result = $"{i + 1}," +
                            $"{"density"}," +
                            $"{"numberOfCheckPoints"}," +
                            $"{"cellularIterations"}," +
                            $"{"height"}," +
                            $"{"width"}," +
                            $"{hpaPath1?.path?.Count ?? 0}," +
                            $"{hpaPath1?.NodesExplored ?? 0}," +
                            $"{timeLevel1}," +
                            $"{hpaPath2?.path?.Count ?? 0}," +
                            $"{hpaPath2?.NodesExplored ?? 0}," +
                            $"{timeLevel2}," +
                            $"{hpaPath3?.path?.Count ?? 0}," +
                            $"{hpaPath3?.NodesExplored ?? 0}," +
                            $"{timeLevel3}," +
                            $"{Apath.Path?.Count ?? 0}," +
                            $"{Apath.NodesExplored}," +
                            $"{timeAStar}";

            results.Add(result);
        }
        name = "simulation_results_hpa_" + name + ".csv";

        string filePath = Path.Combine(Application.dataPath, name);
        File.WriteAllLines(filePath, results);
        UnityEngine.Debug.Log($"Results saved to {filePath}");
    }

    private static (int[,], Vector2Int, Vector2Int) GenerateMapForTests((int min, int max) densityRange, (int min, int max) checkPointsRange, (int min, int max) cellularIterationsRange, (int min, int max) heightRange, (int min, int max) widthRange, int seed = 42)
    {
        System.Random random = new System.Random(seed);
        int density = random.Next(densityRange.min, densityRange.max);
        int numberOfCheckPoints = random.Next(checkPointsRange.min, checkPointsRange.max);
        int cellularIterations = random.Next(cellularIterationsRange.min, cellularIterationsRange.max);
        int height = random.Next(heightRange.min, heightRange.max);
        int width = random.Next(widthRange.min, widthRange.max);
        int amountOfAgents = 1;

        int randomSeed = 42;
        MapModel mapModel = new MapModel(density: density, numberOfCheckPoints: numberOfCheckPoints, iterations: cellularIterations, mapSize: height, randomSeed: randomSeed, amountOfAgents: amountOfAgents);
        int[,] tileMap = CellularAutomata.Create2DMap(mapModel.height, mapModel.width, mapModel.density, mapModel.iterations, 4);
        // map = CellularAutomata.Convert2DTo3D(tileMap);
        int xc = random.Next(2, 100); // Generates a random integer between 1 and 100
        int yc = random.Next(2, 100);

        int xs = random.Next(2, 100); // Generates a random integer between 1 and 100
        int ys = random.Next(2, 100);
        Vector2Int start = new Vector2Int(xs, ys);
        Vector2Int end = new Vector2Int(xc, yc);

        return (tileMap, start, end);
    }

    public static (HPAPath, long) EffiencyInNodeExploration(MapObject[,] tileMap, Vector2Int start, Vector2Int end, int maxLevel, Boolean RefinePath = false, int clusterSize = 10)
    {
        GraphModel _graphModel = new GraphModel(tileMap);
        PathFinder _pathFinder = new PathFinder(_graphModel);
        IEdgeManager edgeManager = new EdgeManager(_pathFinder);
        NodeManager _nodeManager = new NodeManager(_graphModel, edgeManager);
        IEntranceManager entranceManager = new EntranceManager(_graphModel, _nodeManager);
        IClusterManager clusterManager = new ClusterManager(_graphModel, _nodeManager, edgeManager, entranceManager);
        IHPAStar HPAStar = new HPAStar(_graphModel, clusterManager, _nodeManager, entranceManager, edgeManager, _pathFinder);

        HPAStar.Preprocessing(maxLevel, clusterSize);
        if (RefinePath)
        {

            Stopwatch stopwatch = Stopwatch.StartNew();
            HPAPath hpaPath = HPAStar.HierarchicalSearch(start, end, maxLevel);
            stopwatch.Stop();
            return (hpaPath, stopwatch.ElapsedMilliseconds);
        }
        else
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            HPAPath hpaPath = HPAStar.HierarchicalAbstractSearch(start, end, maxLevel);
            stopwatch.Stop();
            return (hpaPath, stopwatch.ElapsedMilliseconds);
        }

    }
    private static void RunDynamicReplanningTests(int iterations, (int min, int max) densityRange, (int min, int max) checkPointsRange, (int min, int max) cellularIterationsRange, (int min, int max) heightRange, (int min, int max) widthRange, bool refinePath, string name)
    {
        var random = new System.Random();
        var results = new List<string> { "Iteration,Density,CheckPoints,CellularIterations,Height,Width,ReplanningTime_Level1,ReplanningTime_AStar,PathLengthChange_Level1,PathLengthChange_AStar" };

        for (int i = 0; i < iterations; i++)
        {
            int density = random.Next(densityRange.min, densityRange.max);
            int numberOfCheckPoints = random.Next(checkPointsRange.min, checkPointsRange.max);
            int cellularIterations = random.Next(cellularIterationsRange.min, cellularIterationsRange.max);
            int height = random.Next(heightRange.min, heightRange.max);
            int width = random.Next(widthRange.min, widthRange.max);
            int amountOfAgents = 1;

            MapModel mapModel = new MapModel(density: density, numberOfCheckPoints: numberOfCheckPoints, iterations: cellularIterations, mapSize: height, randomSeed: random.Next(), amountOfAgents: amountOfAgents);

            int[,] tileMap = CellularAutomata.Create2DMap(mapModel.height, mapModel.width, mapModel.density, mapModel.iterations, 4);
            MapObject[,] map = CellularAutomata.Convert2DTo3D(tileMap);
            Vector2Int start = new Vector2Int(10, 10);
            Vector2Int end = new Vector2Int(mapModel.height - 10, mapModel.width - 10);

            // Build graph once
            var graphModel = new GraphModel(map);
            var pathFinder = new PathFinder(graphModel);
            var edgeManager = new EdgeManager(pathFinder);
            var nodeManager = new NodeManager(graphModel, edgeManager);
            var entranceManager = new EntranceManager(graphModel, nodeManager);
            var clusterManager = new ClusterManager(graphModel, nodeManager, edgeManager, entranceManager);
            var hpaStar = new HPAStar(graphModel, clusterManager, nodeManager, entranceManager, edgeManager, pathFinder);

            hpaStar.Preprocessing(3);

            // Initial Pathfinding
            HPAPath hpaPath1 = ExecutePathfinding(hpaStar, start, end, 1, RefinePath: refinePath);
            HPAPath hpaPath2 = ExecutePathfinding(hpaStar, start, end, 2, RefinePath: refinePath);
            HPAPath hpaPath3 = ExecutePathfinding(hpaStar, start, end, 3, RefinePath: refinePath);
            (List<Vector2Int> Path, int NodesExplored) Apath = Astar.FindPath(start, end, tileMap);

            // Define points to add and remove
            List<Vector2Int> pointsToAdd = new List<Vector2Int> { new Vector2Int(15, 15), new Vector2Int(16, 16) };
            List<Vector2Int> pointsToRemove = new List<Vector2Int> { new Vector2Int(20, 20), new Vector2Int(21, 21) };

            for (int j = 0; j < 500; j++)
            {
                int x = random.Next(0, mapModel.height);
                int y = random.Next(0, mapModel.width);
                if (start.x == x && start.y == y) continue;
                if (end.x == x && end.y == y) continue;

                pointsToAdd.Add(new Vector2Int(x, y));


            }

            for (int j = 0; j < 500; j++)
            {
                int x = random.Next(0, mapModel.height);
                int y = random.Next(0, mapModel.width);
                if (start.x == x && start.y == y) continue;
                if (end.x == x && end.y == y) continue;

                pointsToAdd.Remove(new Vector2Int(x, y));


            }



            // Filter points based on their existence in the graph
            List<Vector2Int> validPointsToAdd = pointsToAdd.Where(p => !graphModel.NodesByLevel[1].ContainsKey(p)).ToList(); // Change 1 to appropriate level
            List<Vector2Int> validPointsToRemove = pointsToRemove.Where(p => graphModel.NodesByLevel[1].ContainsKey(p)).ToList(); // Change 1 to appropriate level

            // Simulate dynamic changes and replanning
            long replanningTimeLevel1 = ApplyChangesAndReplan(hpaStar, start, end, 1, RefinePath: refinePath, validPointsToAdd, validPointsToRemove);
            long replanningTimeLevel2 = ApplyChangesAndReplan(hpaStar, start, end, 2, RefinePath: true, validPointsToAdd, validPointsToRemove);
            long replanningTimeLevel3 = ApplyChangesAndReplan(hpaStar, start, end, 3, RefinePath: true, validPointsToAdd, validPointsToRemove);
            long replanningTimeAStar = ApplyChangesAndReplanAStar(tileMap, start, end, validPointsToAdd, validPointsToRemove);

            // Calculate path length changes
            long pathLengthChangeLevel1 = CalculatePathLengthChange(hpaPath1);
            long pathLengthChangeLevel2 = CalculatePathLengthChange(hpaPath2);
            long pathLengthChangeLevel3 = CalculatePathLengthChange(hpaPath3);
            long pathLengthChangeAStar = CalculatePathLengthChange(Apath);

            string result = $"{i + 1}," +
                            $"{density}," +
                            $"{numberOfCheckPoints}," +
                            $"{cellularIterations}," +
                            $"{height}," +
                            $"{width}," +
                            $"{replanningTimeLevel1}," +
                            $"{replanningTimeLevel2}," +
                            $"{replanningTimeLevel3}," +
                            $"{replanningTimeAStar}," +
                            $"{pathLengthChangeLevel1}," +
                            $"{pathLengthChangeLevel2}," +
                            $"{pathLengthChangeLevel3}," +
                            $"{pathLengthChangeAStar}";

            results.Add(result);
        }
        name = "dynamic_replanning_results_" + name + ".csv";

        string filePath = Path.Combine(Application.dataPath, name);
        File.WriteAllLines(filePath, results);
        Debug.Log($"Results saved to {filePath}");
    }

    private static long ApplyChangesAndReplan(IHPAStar hpaStar, Vector2Int start, Vector2Int end, int level, bool RefinePath, List<Vector2Int> pointsToAdd, List<Vector2Int> pointsToRemove)
    {
        var stopwatch = Stopwatch.StartNew();

        // Add points if they do not already exist
        foreach (var point in pointsToAdd)
        {
            hpaStar.DynamicallyAddHPANode(point);
        }

        // Remove points if they exist
        foreach (var point in pointsToRemove)
        {
            hpaStar.DynamicallyRemoveHPANode(point);
        }

        ExecutePathfinding(hpaStar, start, end, level, RefinePath);
        stopwatch.Stop();
        return stopwatch.ElapsedMilliseconds;
    }

    private static long ApplyChangesAndReplanAStar(int[,] tileMap, Vector2Int start, Vector2Int end, List<Vector2Int> pointsToAdd, List<Vector2Int> pointsToRemove)
    {
        var stopwatch = Stopwatch.StartNew();

        // Add points if they do not already exist
        foreach (var point in pointsToAdd)
        {
            if (tileMap[point.x, point.y] == 0) // Assuming 0 means no obstacle
            {
                tileMap[point.x, point.y] = 1; // Adding an obstacle
            }
        }

        // Remove points if they exist
        foreach (var point in pointsToRemove)
        {
            if (tileMap[point.x, point.y] == 1) // Assuming 1 means obstacle exists
            {
                tileMap[point.x, point.y] = 0; // Removing an obstacle
            }
        }

        (List<Vector2Int> Path, int NodesExplored) Apath = Astar.FindPath(start, end, tileMap);
        stopwatch.Stop();
        return stopwatch.ElapsedMilliseconds;
    }

    private static long CalculatePathLengthChange(HPAPath hpaPath)
    {
        if (hpaPath == null)
        {
            return 0;
        }
        // This method should compare the new path length with the initial path length
        // For simplicity, assume we have a way to get the initial path length
        int initialPathLength = 0; // Placeholder
        return (hpaPath?.path?.Count ?? 0) - initialPathLength;
    }

    private static long CalculatePathLengthChange((List<Vector2Int> Path, int NodesExplored) Apath)
    {
        if (Apath.Path == null) return 0;
        // This method should compare the new path length with the initial path length
        // For simplicity, assume we have a way to get the initial path length
        int initialPathLength = 0; // Placeholder
        return Apath.Path.Count - initialPathLength;
    }

    private static HPAPath ExecutePathfinding(IHPAStar hpaStar, Vector2Int start, Vector2Int end, int maxLevel, bool RefinePath)
    {
        if (RefinePath)
        {
            return hpaStar.HierarchicalSearch(start, end, maxLevel);
        }
        else
        {
            return hpaStar.HierarchicalAbstractSearch(start, end, maxLevel);
        }
    }


}

