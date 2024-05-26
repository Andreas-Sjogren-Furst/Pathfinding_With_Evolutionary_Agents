using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        int numRuns = 30;
        // Generate and save maps for simulations
        savedMaps = GenerateMaps(numRuns, (0, 0), (40, 40), (0, 0), (100, 100), (100, 100));
        RunSimulations("euclidean30Runs.csv", numRuns);
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

        string staticCsvFilePath = "static_" + csvFilePath;
        string dynamicCsvFilePath = "dynamic_" + csvFilePath;

        staticCsvFilePath = Path.Combine(Application.dataPath, staticCsvFilePath);
        dynamicCsvFilePath = Path.Combine(Application.dataPath, dynamicCsvFilePath);



        List<string> csvFiles = new List<string> { staticCsvFilePath, dynamicCsvFilePath };

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
            (run, map, filePath) => RunSimulation("AddNodesDynamically", run, 10, map, filePath)
        };

        // Run each test case multiple times
        for (int run = 0; run < numSimulations; run++)
        {
            // Ensuring each test case runs once per map
            testCases[0](run, savedMaps[run], staticCsvFilePath);
            testCases[1](run, savedMaps[run], dynamicCsvFilePath);
        }
    }

    private void RunSimulation(string mode, int simulationRun, int iterations, MapModel mapModel, string csvFilePath, bool LinearHeuristic = true)
    {
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
                                HPAPath tpath = myGameManager.HPAGraphController.HierarchicalAbstractSearch(new Vector2Int((int)node.X, (int)node.Y), new Vector2Int((int)node2.X, (int)node2.Y), 1);

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

                myGameManager.MmasAddCheckpoint(checkpoints[i].ArrayPosition, 1);
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
}
