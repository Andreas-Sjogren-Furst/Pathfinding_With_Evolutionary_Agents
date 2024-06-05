using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

public class PathfindingTest
{



    // A Test behaves as an ordinary method
    [Test]
    public void PathfindingTestSimplePasses()
    {

        // RunHPASimulation(
        //     iterations: 100,
        //     densityRange: (min: 60, max: 60),
        //     checkPointsRange: (min: 1, max: 5),
        //     cellularIterationsRange: (min: 17, max: 20),
        //     heightRange: (min: 100, max: 100),
        //     widthRange: (min: 100, max: 100),
        //     refinePath: false,
        //     name: "general_maps_timer_refined_random"
        // );

        RunDynamicReplanningTests(
            iterations: 10,
            densityRange: (min: 60, max: 60),
            checkPointsRange: (min: 1, max: 5),
            cellularIterationsRange: (min: 17, max: 20),
            heightRange: (min: 100, max: 100),
            widthRange: (min: 100, max: 100),
            refinePath: true,
            name: "hpaDynamic_1_timer"
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


    private static void RunHPASimulation(int iterations, (int min, int max) densityRange, (int min, int max) checkPointsRange, (int min, int max) cellularIterationsRange, (int min, int max) heightRange, (int min, int max) widthRange, bool refinePath, string name)
    {
        var random = new System.Random();
        var results = new List<string> { "Iteration,Density,CheckPoints,CellularIterations,Height,Width,PathLength_Level1,NodesExplored_Level1,Time_Level1,PathLength_Level2,NodesExplored_Level2,Time_Level2,PathLength_Level3,NodesExplored_Level3,Time_Level3,PathLength_AStar,NodesExplored_AStar,Time_AStar" };

        for (int i = 0; i < iterations; i++)
        {
            int density = random.Next(densityRange.min, densityRange.max);
            int numberOfCheckPoints = random.Next(checkPointsRange.min, checkPointsRange.max);
            int cellularIterations = random.Next(cellularIterationsRange.min, cellularIterationsRange.max);
            int height = random.Next(heightRange.min, heightRange.max);
            int width = random.Next(widthRange.min, widthRange.max);

            int randomSeed = random.Next();
            MapModel mapModel = new MapModel(density: density, numberOfCheckPoints: numberOfCheckPoints, iterations: cellularIterations, mapSize: height, randomSeed: randomSeed);
            int[,] tileMap = CellularAutomata.Create2DMap(mapModel.height, mapModel.width, mapModel.density, mapModel.iterations, 4);
            MapObject[,] map = CellularAutomata.Convert2DTo3D(tileMap);
            int xc = UnityEngine.Random.Range(2, 100); // Generates a random integer between 1 and 100
            int yc = UnityEngine.Random.Range(2, 100);

            int xs = UnityEngine.Random.Range(2, 100); // Generates a random integer between 1 and 100
            int ys = UnityEngine.Random.Range(2, 100);
            Vector2Int start = new Vector2Int(xs, ys);
            Vector2Int end = new Vector2Int(xc, yc);

            // HPA* Pathfinding at different abstraction levels
            (HPAPath hpaPath1, var timeLevel1) = EffiencyInNodeExploration(map, start, end, 1, RefinePath: refinePath);

            (HPAPath hpaPath2, var timeLevel2) = EffiencyInNodeExploration(map, start, end, 2, RefinePath: refinePath);

            (HPAPath hpaPath3, var timeLevel3) = EffiencyInNodeExploration(map, start, end, 3, RefinePath: refinePath);

            // A* Pathfinding
            var stopwatch = Stopwatch.StartNew();
            (List<Vector2Int> Path, int NodesExplored) Apath = Astar.FindPath(start, end, tileMap);
            stopwatch.Stop();
            var timeAStar = stopwatch.ElapsedMilliseconds;

            string result = $"{i + 1}," +
                            $"{density}," +
                            $"{numberOfCheckPoints}," +
                            $"{cellularIterations}," +
                            $"{height}," +
                            $"{width}," +
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


    public static (HPAPath, long) EffiencyInNodeExploration(MapObject[,] tileMap, Vector2Int start, Vector2Int end, int maxLevel, Boolean RefinePath = false)
    {
        GraphModel _graphModel = new GraphModel(tileMap);
        PathFinder _pathFinder = new PathFinder(new GraphModel(tileMap));
        IEdgeManager edgeManager = new EdgeManager(_pathFinder);
        NodeManager _nodeManager = new NodeManager(_graphModel, edgeManager);
        IEntranceManager entranceManager = new EntranceManager(_graphModel, _nodeManager);
        IClusterManager clusterManager = new ClusterManager(_graphModel, _nodeManager, edgeManager, entranceManager);
        IHPAStar HPAStar = new HPAStar(_graphModel, clusterManager, _nodeManager, entranceManager, edgeManager, _pathFinder);

        HPAStar.Preprocessing(maxLevel);
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
        var results = new List<string> {
        "Iteration,Density,CheckPoints,CellularIterations,Height,Width,InitialTime_Level1,InitialTime_Level2,InitialTime_Level3,InitialTime_AStar,ReplanningTime_Level1,ReplanningTime_Level2,ReplanningTime_Level3,ReplanningTime_AStar,InitialPathLength_AStar,InitialPathLength_Level1,InitialPathLength_Level2,InitialPathLength_Level3,NewPathLength_AStar,NewPathLength_Level1,NewPathLength_Level2,NewPathLength_Level3,PathLengthChange_Level1,PathLengthChange_Level2,PathLengthChange_Level3,PathLengthChange_AStar"
        };
        for (int i = 0; i < iterations; i++)
        {
            int density = random.Next(densityRange.min, densityRange.max);
            int numberOfCheckPoints = random.Next(checkPointsRange.min, checkPointsRange.max);
            int cellularIterations = random.Next(cellularIterationsRange.min, cellularIterationsRange.max);
            int height = random.Next(heightRange.min, heightRange.max);
            int width = random.Next(widthRange.min, widthRange.max);

            MapModel mapModel = new MapModel(density: density, numberOfCheckPoints: numberOfCheckPoints, iterations: cellularIterations, mapSize: height, randomSeed: 0);
            int[,] tileMap = CellularAutomata.Create2DMap(mapModel.height, mapModel.width, mapModel.density, mapModel.iterations, 4);
            MapObject[,] map = CellularAutomata.Convert2DTo3D(tileMap);
            // Vector2Int start = new Vector2Int(10, 10);
            // Vector2Int end = new Vector2Int(mapModel.height - 10, mapModel.width - 10);


            Vector2Int start = new(26, 40);
            Vector2Int end = new(44, 28);

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
            var (hpaPath1, initialTimeLevel1) = MeasurePathfindingTime(hpaStar, start, end, 1, refinePath);
            var (hpaPath2, initialTimeLevel2) = MeasurePathfindingTime(hpaStar, start, end, 2, refinePath);
            var (hpaPath3, initialTimeLevel3) = MeasurePathfindingTime(hpaStar, start, end, 3, refinePath);
            var (Apath, nodesExplored, initialTimeAStar) = MeasureAStarTime(tileMap, start, end);

            // Define points to add and remove
            var pointsToAdd = GenerateRandomPoints(random, mapModel, start, end, 0);
            var pointsToRemove = GenerateRandomPoints(random, mapModel, start, end, 0);

            for (int j = start.x - 5; j < start.x + 5; j++)
            {
                for (int k = start.y - 5; k < start.y + 5; k++)
                {
                    if (j >= 0 && j < mapModel.height && k >= 0 && k < mapModel.width)
                    {
                        pointsToAdd.Add(new Vector2Int(j, k));

                    }
                }
            }


            if (pointsToRemove.Contains(start)) pointsToRemove.Remove(start);
            if (pointsToRemove.Contains(end)) pointsToRemove.Remove(end);



            // Apply changes and replan
            var (hpaPath1_after, initialTimeLevel1_after) = ApplyChangesAndReplan(hpaStar, start, end, 1, refinePath, pointsToAdd, pointsToRemove);
            var (hpaPath2_after, initialTimeLevel2_after) = ApplyChangesAndReplan(hpaStar, start, end, 2, refinePath, pointsToAdd, pointsToRemove);
            var (hpaPath3_after, initialTimeLevel3_after) = ApplyChangesAndReplan(hpaStar, start, end, 3, refinePath, pointsToAdd, pointsToRemove);
            var (Apath_after, nodesExplored_after, initialTimeAStar_after) = ApplyChangesAndReplanAStar(tileMap, start, end, pointsToAdd, pointsToRemove);

            // Calculate path length changes
            long pathLengthChangeLevel1 = CalculatePathLengthChange(hpaPath1_after, hpaPath1);
            long pathLengthChangeLevel2 = CalculatePathLengthChange(hpaPath2_after, hpaPath1);
            long pathLengthChangeLevel3 = CalculatePathLengthChange(hpaPath3_after, hpaPath3);
            long pathLengthChangeAStar = CalculatePathLengthChange(Apath_after, Apath);

            long replanningTimeLevel1 = initialTimeLevel1_after - initialTimeLevel1;
            long replanningTimeLevel2 = initialTimeLevel2_after - initialTimeLevel2;
            long replanningTimeLevel3 = initialTimeLevel3_after - initialTimeLevel3;
            long replanningTimeAStar = initialTimeAStar_after - initialTimeAStar;

            string result = $"{i + 1}," +
                            $"{density}," +
                            $"{numberOfCheckPoints}," +
                            $"{cellularIterations}," +
                            $"{height}," +
                            $"{width}," +
                            $"{initialTimeLevel1}," +
                            $"{initialTimeLevel2}," +
                            $"{initialTimeLevel3}," +
                            $"{initialTimeAStar}," +
                            $"{replanningTimeLevel1}," +
                            $"{replanningTimeLevel2}," +
                            $"{replanningTimeLevel3}," +
                            $"{replanningTimeAStar}," +
                            $"{Apath?.Count ?? 0}," +
                            $"{hpaPath1?.path?.Count ?? 0}," +
                            $"{hpaPath2?.path?.Count ?? 0}," +
                            $"{hpaPath3?.path?.Count ?? 0}," +
                            $"{Apath_after?.Count ?? 0}," +
                            $"{hpaPath1_after?.path?.Count ?? 0}," +
                            $"{hpaPath2_after?.path?.Count ?? 0}," +
                            $"{hpaPath3_after?.path?.Count ?? 0}," +
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

    private static (HPAPath, long) MeasurePathfindingTime(IHPAStar hpaStar, Vector2Int start, Vector2Int end, int level, bool refinePath)
    {
        var stopwatch = Stopwatch.StartNew();
        HPAPath hpaPath = ExecutePathfinding(hpaStar, start, end, level, refinePath);
        stopwatch.Stop();
        return (hpaPath, stopwatch.ElapsedMilliseconds);
    }

    private static (List<Vector2Int> Path, int NodesExplored, long Time) MeasureAStarTime(int[,] tileMap, Vector2Int start, Vector2Int end)
    {
        var stopwatch = Stopwatch.StartNew();
        var Apath = Astar.FindPath(start, end, tileMap);
        stopwatch.Stop();
        return (Apath.Path, Apath.NodesExplored, stopwatch.ElapsedMilliseconds);
    }

    private static List<Vector2Int> GenerateRandomPoints(System.Random random, MapModel mapModel, Vector2Int start, Vector2Int end, int count)
    {
        var points = new List<Vector2Int>();
        for (int j = 0; j < count; j++)
        {
            int x = random.Next(0, mapModel.height);
            int y = random.Next(0, mapModel.width);
            if (start.x == x && start.y == y) continue;
            if (end.x == x && end.y == y) continue;
            points.Add(new Vector2Int(x, y));
        }
        return points;
    }


    private static (HPAPath, long) ApplyChangesAndReplan(IHPAStar hpaStar, Vector2Int start, Vector2Int end, int level, bool RefinePath, List<Vector2Int> pointsToAdd, List<Vector2Int> pointsToRemove)
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

        var hpaPath = ExecutePathfinding(hpaStar, start, end, level, RefinePath);
        stopwatch.Stop();
        return (hpaPath, stopwatch.ElapsedMilliseconds);
    }

    private static (List<Vector2Int> Path, int NodesExplored, long) ApplyChangesAndReplanAStar(int[,] tileMap, Vector2Int start, Vector2Int end, List<Vector2Int> pointsToAdd, List<Vector2Int> pointsToRemove)
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

        (List<Vector2Int> path, int NodesExplored) = Astar.FindPath(start, end, tileMap);
        stopwatch.Stop();
        return (path, NodesExplored, stopwatch.ElapsedMilliseconds);
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

    private static long CalculatePathLengthChange(HPAPath initialPath, HPAPath newPath)
    {
        if (initialPath == null || newPath == null || initialPath.path == null || newPath.path == null)
        {
            return 0;
        }
        return newPath.path.Count - initialPath.path.Count;
    }

    private static long CalculatePathLengthChange(List<Vector2Int> initialPath, List<Vector2Int> newPath)
    {
        if (initialPath == null || newPath == null)
        {
            return 0;
        }
        return newPath.Count - initialPath.Count;
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

