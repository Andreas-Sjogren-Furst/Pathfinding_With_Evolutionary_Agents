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
        int numRuns = 10;
        // Generate and save maps for simulations
        savedMaps = GenerateMaps(numRuns, (60, 60), (10, 10), (17, 20), (100, 100), (100, 100));
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
        List<MapModel> maps = new List<MapModel>();
        for (int i = 0; i < numMaps; i++)
        {
            MapModel mapModel = new MapModel(
                density: UnityEngine.Random.Range(densityRange.min, densityRange.max),
                numberOfCheckPoints: UnityEngine.Random.Range(checkPointsRange.min, checkPointsRange.max),
                iterations: UnityEngine.Random.Range(cellularIterationsRange.min, cellularIterationsRange.max),
                mapSize: UnityEngine.Random.Range(widthRange.min, widthRange.max),
                randomSeed: UnityEngine.Random.Range(0, 100)
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

        Dictionary<Vector2Int, Node> nodeDictionary = new Dictionary<Vector2Int, Node>();

        for (int i = 0; i < checkpoints.Count; i++)
        {
            Vector2Int pos1 = checkpoints[i].ArrayPosition;
            Node node1;
            if (!nodeDictionary.TryGetValue(pos1, out node1))
            {
                node1 = new Node(graph.Nodes.Count, pos1.x, pos1.y);
                nodeDictionary[pos1] = node1;
                graph.AddNode(node1);  // Add node to graph and assign ID
            }

            for (int j = 0; j < checkpoints.Count; j++)
            {
                if (i == j) continue;

                Vector2Int pos2 = checkpoints[j].ArrayPosition;
                Node node2;
                if (!nodeDictionary.TryGetValue(pos2, out node2))
                {
                    node2 = new Node(graph.Nodes.Count, pos2.x, pos2.y);
                    nodeDictionary[pos2] = node2;
                    graph.AddNode(node2);  // Add node to graph and assign ID
                }

                // First check if there's a direct path between the nodes using A*
                (List<Vector2Int> apath, int nodesExplored) = Astar.FindPath(pos1, pos2, map);
                if (apath == null || apath.Count == 0)
                {
                    // Debug.Log("No path found between " + pos1 + " and " + pos2);
                    continue;
                }

                double distance = apath.Count;
                // Then check the hierarchical abstract search path
                if (LinearHeuristic)
                {
                    distance = Vector2Int.Distance(pos1, pos2);
                    graph.AddEdge(node1, node2, distance);
                    graph.AddEdge(node2, node1, distance);
                    continue;
                }
                HPAPath tpath = myGameManager.HPAGraphController.HierarchicalAbstractSearch(pos1, pos2, heuristicsLevel);
                if (tpath != null && tpath.Length > 0 && tpath.Length < double.PositiveInfinity)
                {
                    distance = tpath.Length;
                }

                // Add edges if the path is valid
                graph.AddEdge(node1, node2, distance);
                graph.AddEdge(node2, node1, distance);
            }
        }

        // Ensure the graph is fully connected
        if (!graph.IsFullyConnected())
        {
            // Debug.LogError("The graph is not fully connected.");
            return; // Exit the function if the graph is not fully connected
        }

        // Reassign IDs to the remaining nodes
        for (int i = 0; i < graph.Nodes.Count; i++)
        {
            graph.Nodes[i].Id = i;
        }

        Debug.Log("Total checkpoints added: " + graph.Nodes.Count);

        myGameManager.mmasGraphController.SetGraph(graph);
        int localIterations = myGameManager.mmasGraphController.Run(500);

        // Calculate the actual length of the path using A*
        Node[] bestTour = myGameManager.mmasGraphController.GetBestTour();
        int totalDistance = 0;
        for (int i = 0; i < bestTour.Length - 1; i++)
        {
            Vector2Int start = new Vector2Int((int)bestTour[i].X, (int)bestTour[i].Y);
            Vector2Int end = new Vector2Int((int)bestTour[i + 1].X, (int)bestTour[i + 1].Y);

            (List<Vector2Int> apath, int nodesExplored) = Astar.FindPath(start, end, map);
            totalDistance += apath.Count;
        }

        MMASIterations += localIterations;
        // Collect and save results
        SaveResults(mode, simulationRun, graph.Nodes.Count, MMASIterations, localIterations, totalDistance, csvFilePath);
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
