using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MmasMapTest
{
    private List<MapModel> savedMaps;

    // A Test behaves as an ordinary method
    [Test]
    public void MmasMapTestSimplePasses()
    {
        int numRuns = 100;
        // Generate and save maps for simulations
        savedMaps = GenerateMaps(numRuns, (60, 60), (40, 40), (17, 20), (100, 100), (100, 100));
        RunSimulations("_experiment_two.csv", numRuns);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator MmasMapTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }

    public List<MapModel> GenerateMaps(int numMaps, (int min, int max) densityRange, (int min, int max) checkPointsRange, (int min, int max) cellularIterationsRange, (int min, int max) heightRange, (int min, int max) widthRange)
    {
        System.Random rand = new System.Random();

        List<MapModel> maps = new List<MapModel>();
        for (int i = 0; i < numMaps; i++)
        {
            MapModel mapModel = new MapModel(
          density: rand.Next(densityRange.min, densityRange.max),
            numberOfCheckPoints: rand.Next(checkPointsRange.min, checkPointsRange.max),
            iterations: rand.Next(cellularIterationsRange.min, cellularIterationsRange.max),
            mapSize: rand.Next(widthRange.min, widthRange.max),
            randomSeed: rand.Next(0, 100)
            );
            maps.Add(mapModel);
        }
        return maps;
    }

    public void RunSimulations(string csvFilePath, int numSimulations)
    {

        string staticCsvFilePath = "static" + csvFilePath;
        string dynamicCsvFilePath = "dynamic" + csvFilePath;
        string linearHeuristicFilePath = "linearHeuristic" + csvFilePath;
        string abstract1HeuristicFilePath = "abstract1" + csvFilePath;
        string abstract2HeuristicFilePath = "abstract2" + csvFilePath;
        string abstract3HeuristicFilePath = "abstract3" + csvFilePath;


        staticCsvFilePath = Path.Combine(Application.dataPath, staticCsvFilePath);
        dynamicCsvFilePath = Path.Combine(Application.dataPath, dynamicCsvFilePath);



        List<string> csvFiles = new List<string> { staticCsvFilePath, dynamicCsvFilePath, linearHeuristicFilePath, abstract1HeuristicFilePath, abstract2HeuristicFilePath, abstract3HeuristicFilePath };

        // Clear the previous results and add header once
        foreach (string file in csvFiles)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
            using (StreamWriter writer = new StreamWriter(file, true))
            {
                writer.WriteLine("Mode,SimulationRun,Checkpoints,MMASIterations,LocalIterations,BestTourLength,BestTourNodes");
            }
        }

        // List of test cases
        List<Action<int, MapModel, string>> testCases = new List<Action<int, MapModel, string>>
        {
            (run, map, filePath) => RunSimulation("RebuildWholeGraph", run, 10, map, filePath),
            (run, map, filePath) => RunSimulation("AddNodesDynamically", run, 10, map, filePath),
            (run, map, filePath) => RunHeurticsSimulation("AddNodesDynamically", run, 10, map, filePath, true),
            (run, map, filePath) => RunHeurticsSimulation("AddNodesDynamically", run, 10, map, filePath, false),
            (run, map, filePath) => RunHeurticsSimulation("AddNodesDynamically", run, 10, map, filePath, false),
            (run, map, filePath) => RunHeurticsSimulation("AddNodesDynamically", run, 10, map, filePath, false),







        };

        // Run each test case multiple times
        for (int run = 0; run < numSimulations; run++)
        {
            // Ensuring each test case runs once per map
            // testCases[0](run, savedMaps[run], staticCsvFilePath);
            // testCases[1](run, savedMaps[run], dynamicCsvFilePath);
            testCases[2](run, savedMaps[run], linearHeuristicFilePath);
            testCases[3](run, savedMaps[run], abstract1HeuristicFilePath);
            testCases[4](run, savedMaps[run], abstract2HeuristicFilePath);
            testCases[5](run, savedMaps[run], abstract3HeuristicFilePath);


        }
    }


    private void RunHeurticsSimulation(string mode, int simulationRun, int iterations, MapModel mapModel, string csvFilePath, bool LinearHeuristic = true, int heuristicsLevel = 1)
    {
        MyGameManager myGameManager = new MyGameManager(mapModel);
        int[,] map = CellularAutomata.Convert3DTo2D(mapModel.map);

        Graph graph = new();

        List<CheckPoint> checkpoints = mapModel.checkPoints;
        int MMASIterations = 0;

        Vector2Int centerCheckpoint = new Vector2Int(mapModel.map.GetLength(0) / 2, mapModel.map.GetLength(1) / 2);

        List<CheckPoint> validCheckpoints = new List<CheckPoint>();




        for (int i = 0; i < checkpoints.Count; i++)
        {
            (List<Vector2Int> path, int exploredNodes) = Astar.FindPath(centerCheckpoint, checkpoints[i].ArrayPosition, map);

            if (path?.Count != null && path.Count > 0 && path.Count < 10000)
            {
                validCheckpoints.Add(checkpoints[i]);
            }

        }

        // create the nodes :

        for (int i = 0; i < validCheckpoints.Count; i++)
        {
            graph.AddNode(new Node(graph.Nodes.Count, validCheckpoints[i].ArrayPosition.x, validCheckpoints[i].ArrayPosition.y));
        }

        if (validCheckpoints.Count < 3)
        {
            return;
        }

        // connect the valid checkpoints.

        for (int i = 0; i < validCheckpoints.Count; i++)
        {
            for (int j = 0; j < validCheckpoints.Count; j++)
            {
                double distance;
                if (LinearHeuristic) // for faster speed in tests. 
                {
                    distance = Vector2Int.Distance(validCheckpoints[i].ArrayPosition, validCheckpoints[j].ArrayPosition);
                }
                else
                {
                    HPAPath path = myGameManager.HPAGraphController.HierarchicalAbstractSearch(validCheckpoints[i].ArrayPosition, validCheckpoints[j].ArrayPosition, heuristicsLevel);

                    if (path?.Length != null && path.Length > 0)
                    {
                        distance = path.Length;
                    }
                    else
                    { // fallback to linear distance since HPA Path is an approximation.
                        distance = Vector2Int.Distance(validCheckpoints[i].ArrayPosition, validCheckpoints[j].ArrayPosition);

                    }
                }
                graph.AddEdge(graph.Nodes[i], graph.Nodes[j], distance);
                graph.AddEdge(graph.Nodes[j], graph.Nodes[i], distance);

            }
        }

        myGameManager.mmasGraphController.SetGraph(graph);
        MMASIterations = myGameManager.mmasGraphController.Run(500);
        Node[] nodes = myGameManager.mmasGraphController.GetBestTour();
        int totalDistance = 0;
        for (int i = 0; i < nodes.Length - 1; i++)
        {
            (List<Vector2Int> path, int exploredNodes) = Astar.FindPath(centerCheckpoint, new Vector2Int((int)nodes[i].X, (int)nodes[i].Y), map);
            totalDistance += path.Count;
        }


        Debug.Log($"Valid Checkpoints: {validCheckpoints.Count}");
        Debug.Log($"Total Distance: {totalDistance}");


        // Collect and save results
        SaveResults(mode, simulationRun, graph.Nodes.Count, MMASIterations, MMASIterations, totalDistance, csvFilePath);
    }










    private void RunSimulation(string mode, int simulationRun, int iterations, MapModel mapModel, string csvFilePath, bool LinearHeuristic = true, int heuristicsLevel = 1)
    {


        // Vector2Int centerCheckpoint = new Vector2Int(mapModel.map.GetLength(0) / 2, mapModel.map.GetLength(1) / 2);


        MyGameManager myGameManager = new MyGameManager(mapModel);
        Graph graph = new();

        List<CheckPoint> checkpoints = mapModel.checkPoints;
        int MMASIterations = 0;





        for (int i = 0; i < checkpoints.Count; i++)
        {
            if (i < 4)
            {
                graph.AddNode(new Node(graph.Nodes.Count, checkpoints[i].ArrayPosition.x, checkpoints[i].ArrayPosition.y));

                if (i == 3)
                {
                    foreach (Node node in graph.Nodes)
                    {
                        foreach (Node node2 in graph.Nodes)
                        {
                            double distance = double.PositiveInfinity;
                            if (LinearHeuristic) // for faster speed in tests. 
                            {
                                distance = Vector2Int.Distance(new Vector2Int((int)node.X, (int)node.Y), new Vector2Int((int)node2.X, (int)node2.Y));
                            }
                            else
                            {
                                HPAPath tpath = myGameManager.HPAGraphController.HierarchicalAbstractSearch(new Vector2Int((int)node.X, (int)node.Y), new Vector2Int((int)node2.X, (int)node2.Y), heuristicsLevel);

                                if (tpath != null)
                                {
                                    distance = tpath.Length;
                                }
                            }
                            graph.AddEdge(node, node2, distance);
                            graph.AddEdge(node2, node, distance);
                        }
                    }
                    myGameManager.mmasGraphController.SetGraph(graph);
                }
                continue;
            }

            if (mode == "RebuildWholeGraph")
            {

                myGameManager.MmasAddCheckpoint(checkpoints[i].ArrayPosition, heuristicsLevel, linearHeuristic: LinearHeuristic);
                myGameManager.mmasGraphController.SetGraph(graph);
            }
            else if (mode == "AddNodesDynamically")
            {
                // Add nodes dynamically
                myGameManager.MmasAddCheckpoint(checkpoints[i].ArrayPosition, 1);
            }

            // Run the MMAS algorithm to get the optimal path
            int localIterations = myGameManager.mmasGraphController.Run(500);
            MMASIterations = MMASIterations + localIterations;
            // Collect and save results
            SaveResults(mode, simulationRun, i, MMASIterations, localIterations, myGameManager.mmasGraphController, csvFilePath);
        }
    }

    private void SaveResults(string mode, int simulationRun, int iteration, int MMASIterations, int localIterations, MMAS mmas, string csvFilePath)
    {
        var bestTourLength = mmas.GetBestTourLength();
        var bestTour = mmas.GetBestTour();


        using (StreamWriter writer = new StreamWriter(csvFilePath, true))
        {
            writer.WriteLine($"{mode},{simulationRun},{iteration},{MMASIterations},{localIterations},{bestTourLength},{bestTour.Length}");
        }
    }

    private void SaveResults(string mode, int simulationRun, int iteration, int MMASIterations, int localIterations, int bestTourLength, string csvFilePath)
    {
        // var bestTourLength = mmas.GetBestTourLength();
        // var bestTour = mmas.GetBestTour();


        using (StreamWriter writer = new StreamWriter(csvFilePath, true))
        {
            writer.WriteLine($"{mode},{simulationRun},{iteration},{MMASIterations},{localIterations},{bestTourLength},{0}");
        }
    }
}
