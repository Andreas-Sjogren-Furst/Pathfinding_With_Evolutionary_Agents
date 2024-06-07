// written by: Gustav Clausen s214940

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
        // savedMaps = GenerateMaps(numRuns, (60, 60), (15, 15), (17, 20), (100, 100), (100, 100));
        savedMaps = GenerateMaps(numRuns, (0, 0), (40, 40), (0, 0), (100, 100), (100, 100));

        RunSimulations("Experiment_10_remove_node_30runs", numRuns, heuristicSimulation: false, scaleGraphEdges: false, plotIterations_stagnation: false);
        // RunSimulations("30_experiment_NEW2_parameters_trail_limits_non_scaled", numRuns, heuristicSimulation: true, scaleGraphEdges: false, plotIterations_stagnation: false);
        // RunSimulations("30_experiment_NEW2_parameters_trail_limits_non_scaled", numRuns, heuristicSimulation: true, scaleGraphEdges: false, plotIterations_stagnation: false);

        // 

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
        System.Random rand = new System.Random(42);

        List<MapModel> maps = new List<MapModel>();
        for (int i = 0; i < numMaps; i++)
        {
            // Debug.Log($"Generating map {i} + {rand.Next(0, 100)}");
            MapModel mapModel = new MapModel(
          density: rand.Next(densityRange.min, densityRange.max),
            numberOfCheckPoints: rand.Next(checkPointsRange.min, checkPointsRange.max),
            iterations: rand.Next(cellularIterationsRange.min, cellularIterationsRange.max),
            mapSize: rand.Next(widthRange.min, widthRange.max),
            randomSeed: rand.Next(0, 100),
            amountOfAgents: 1
            );
            maps.Add(mapModel);
        }
        return maps;
    }

    public void RunSimulations(string baseFolder, int numSimulations, bool heuristicSimulation, bool scaleGraphEdges, bool plotIterations_stagnation)
    {
        string baseFolderPath = Path.Combine(Application.dataPath, baseFolder);
        if (!Directory.Exists(baseFolderPath))
        {
            Directory.CreateDirectory(baseFolderPath);
        }

        string[] fileNames = { "static.csv", "dynamic.csv", "manhattenHeuristic.csv", "linearHeuristic.csv", "astar.csv", "abstract1.csv", "abstract2.csv", "abstract3.csv", "normalize.csv", "RemoveRebuild.csv", "RemoveDynamic.csv", "RemoveDynamicNormalize.csv" };
        string[] csvFilePaths = new string[fileNames.Length];

        for (int i = 0; i < fileNames.Length; i++)
        {
            csvFilePaths[i] = Path.Combine(baseFolderPath, fileNames[i]);
            if (File.Exists(csvFilePaths[i]))
            {
                File.Delete(csvFilePaths[i]);
            }
            using (StreamWriter writer = new StreamWriter(csvFilePaths[i], true))
            {
                writer.WriteLine("Mode,SimulationRun,Checkpoints,MMASIterations,LocalIterations,BestTourLength,BestTourNodes");
            }
        }

        // Run each test case multiple times
        for (int run = 0; run < numSimulations; run++)
        {
            if (heuristicSimulation)
            {
                RunHeurticsSimulation("RebuildWholeGraph", run, 10, savedMaps[run], csvFilePaths[2], false, -1);
                RunHeurticsSimulation("RebuildWholeGraph", run, 10, savedMaps[run], csvFilePaths[3], true);
                RunHeurticsSimulation("RebuildWholeGraph", run, 10, savedMaps[run], csvFilePaths[4], false, 0);
                RunHeurticsSimulation("RebuildWholeGraph", run, 10, savedMaps[run], csvFilePaths[5], false, 1);
                RunHeurticsSimulation("RebuildWholeGraph", run, 10, savedMaps[run], csvFilePaths[6], false, 2);
                RunHeurticsSimulation("RebuildWholeGraph", run, 10, savedMaps[run], csvFilePaths[7], false, 3);
            }
            else
            {
                RunSimulation("RebuildWholeGraph", run, 10, savedMaps[run], csvFilePaths[0]);
                RunSimulation("AddNodesDynamically", run, 10, savedMaps[run], csvFilePaths[1]);
                RunSimulation("AddNodesDynamically", run, 10, savedMaps[run], csvFilePaths[8], normalize: true);
                RunSimulationRemovalOfCheckpoints("RebuildWholeGraph", run, 10, savedMaps[run], csvFilePaths[9]);
                RunSimulationRemovalOfCheckpoints("RemoveNodesDynamically", run, 10, savedMaps[run], csvFilePaths[10]);
                RunSimulationRemovalOfCheckpoints("RemoveNodesDynamically", run, 10, savedMaps[run], csvFilePaths[11], normalize: true);


            }
        }
    }





    private void RunHeurticsSimulation(string mode, int simulationRun, int iterations, MapModel mapModel, string csvFilePath, bool LinearHeuristic = true, int heuristicsLevel = 1, bool scaleGraphEdges = false, bool plotIterations_stagnation = false)
    {
        MyGameManager myGameManager = new MyGameManager(mapModel);
        int[,] map = CellularAutomata.Convert3DTo2D(mapModel.map);

        Graph graph = new();

        List<CheckPoint> checkpoints = mapModel.checkPoints;

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
                if (i == j)
                {
                    continue;
                }
                double distance;
                if (LinearHeuristic) // for faster speed in tests. 
                {
                    distance = Vector2Int.Distance(validCheckpoints[i].ArrayPosition, validCheckpoints[j].ArrayPosition);
                }
                else if (heuristicsLevel == -1)
                { // manhatten distance

                    distance = Math.Abs(validCheckpoints[i].ArrayPosition.x - validCheckpoints[j].ArrayPosition.x) + Math.Abs(validCheckpoints[i].ArrayPosition.y - validCheckpoints[j].ArrayPosition.y);

                }
                else if (heuristicsLevel == 0)
                {
                    (List<Vector2Int> path, int exploredNodes) = Astar.FindPath(validCheckpoints[i].ArrayPosition, validCheckpoints[j].ArrayPosition, map);
                    if (path?.Count != null && path.Count > 0)
                    {
                        distance = path.Count;

                    }
                    else
                    { // fall back to linear distance should not happen, since A* guranntees to find the path. 
                        Debug.Log("A* failed to find the path");
                        distance = Math.Abs(validCheckpoints[i].ArrayPosition.x - validCheckpoints[j].ArrayPosition.x) + Math.Abs(validCheckpoints[i].ArrayPosition.y - validCheckpoints[j].ArrayPosition.y);
                    }

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
                        Debug.Log("HPA failed to find the path at level " + heuristicsLevel + " from " + validCheckpoints[i].ArrayPosition + " to " + validCheckpoints[j].ArrayPosition + " in simulation " + simulationRun);
                        distance = Math.Abs(validCheckpoints[i].ArrayPosition.x - validCheckpoints[j].ArrayPosition.x) + Math.Abs(validCheckpoints[i].ArrayPosition.y - validCheckpoints[j].ArrayPosition.y);




                    }
                }
                graph.AddEdge(graph.Nodes[i], graph.Nodes[j], distance);
                graph.AddEdge(graph.Nodes[j], graph.Nodes[i], distance);

            }
        }

        if (scaleGraphEdges)
        {
            graph.ScaleGraphEdges(0, 10000);
        }

        for (int runs = 0; runs < 10; runs++)
        {
            Node[] nodes = new Node[graph.Nodes.Count];
            int MMASIterations = 0;
            double localTourLength = 0.0;


            if (plotIterations_stagnation)
            {
                for (int i = 0; i < 1000; i++)
                {
                    myGameManager.mmasGraphController.SetGraph(graph);
                    MMASIterations = myGameManager.mmasGraphController.Run(1);
                    nodes = myGameManager.mmasGraphController.GetBestTour();
                    localTourLength = myGameManager.mmasGraphController.GetBestTourLength();
                    SaveResults(mode, simulationRun, i, MMASIterations, MMASIterations, 0, localTourLength, csvFilePath + "_stagnation.csv");
                }
            }
            else
            {
                myGameManager.mmasGraphController.SetGraph(graph);
                MMASIterations = myGameManager.mmasGraphController.Run(1000);
                nodes = myGameManager.mmasGraphController.GetBestTour();
                localTourLength = myGameManager.mmasGraphController.GetBestTourLength();
            }

            int totalDistance = 0;
            for (int i = 0; i < nodes.Length - 1; i++)
            {
                Node node1 = nodes[i];
                Node node2 = nodes[i + 1];
                Vector2Int v1 = new Vector2Int((int)node1.X, (int)node1.Y);
                Vector2Int v2 = new Vector2Int((int)node2.X, (int)node2.Y);
                (List<Vector2Int> path, int exploredNodes) = Astar.FindPath(v1, v2, map);
                totalDistance += path.Count;
            }




            // Collect and save results
            SaveResults(mode, simulationRun, graph.Nodes.Count, MMASIterations, MMASIterations, totalDistance, localTourLength, csvFilePath);

        }


    }










    private void RunSimulation(string mode, int simulationRun, int iterations, MapModel mapModel, string csvFilePath, bool LinearHeuristic = true, int heuristicsLevel = 1, bool normalize = false)
    {


        // Vector2Int centerCheckpoint = new Vector2Int(mapModel.map.GetLength(0) / 2, mapModel.map.GetLength(1) / 2);


        MyGameManager myGameManager = new MyGameManager(mapModel);
        Graph graph = new();

        List<CheckPoint> checkpoints = mapModel.checkPoints;
        int MMASIterations = 0;

        Debug.Log($"Checkpoints: {checkpoints.Count}");





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

                DynamicGraphoperations.MmasAddCheckpoint(ref myGameManager.mmasGraphController, ref myGameManager.HPAGraphController, checkpoints[i].ArrayPosition, heuristicsLevel, linearHeuristic: LinearHeuristic);
                myGameManager.mmasGraphController.SetGraph(graph);
            }
            else if (mode == "AddNodesDynamically")
            {
                // Add nodes dynamically
                DynamicGraphoperations.MmasAddCheckpoint(ref myGameManager.mmasGraphController, ref myGameManager.HPAGraphController, checkpoints[i].ArrayPosition, 1, normalize: normalize);
            }

            // Run the MMAS algorithm to get the optimal path
            int localIterations = myGameManager.mmasGraphController.Run(1000);
            MMASIterations = MMASIterations + localIterations;
            // Collect and save results
            SaveResults(mode, simulationRun, i, MMASIterations, localIterations, myGameManager.mmasGraphController, csvFilePath);
        }
    }



    private void RunSimulationRemovalOfCheckpoints(string mode, int simulationRun, int iterations, MapModel mapModel, string csvFilePath, bool LinearHeuristic = true, int heuristicsLevel = 1, bool normalize = false)
    {



        MyGameManager myGameManager = new MyGameManager(mapModel);



        List<CheckPoint> checkpoints = mapModel.checkPoints;
        int MMASIterations = 0;
        Graph graph = new();
        MMAS mmas = new MMAS(checkpoints.Count, 1.5, 4.5, 0.9, 100, graph);

        Debug.Log($"Checkpoints: {checkpoints.Count}");

        for (int j = 0; j < checkpoints.Count; j++)
        {
            DynamicGraphoperations.MmasAddCheckpoint(ref mmas, ref myGameManager.HPAGraphController, checkpoints[j].ArrayPosition, 1);
        }
        mmas.SetGraph(graph);
        mmas.Run(5000);


        for (int i = checkpoints.Count - 1; i > 4; i--)
        {

            if (mode == "RebuildWholeGraph")
            {

                DynamicGraphoperations.MmasRemoveCheckpoint(ref mmas, checkpoints[i].ArrayPosition);
                // Debug.Log("Graph nodes " + graph.Nodes.Count);
                mmas.SetGraph(mmas._graph);
            }
            else if (mode == "RemoveNodesDynamically")
            {
                // Remove nodes dynamically
                DynamicGraphoperations.MmasRemoveCheckpoint(ref mmas, checkpoints[i].ArrayPosition, normalize: normalize);
            }

            // Run the MMAS algorithm to get the optimal path
            int localIterations = mmas.Run(1000);
            MMASIterations = MMASIterations + localIterations;
            // Collect and save results
            SaveResults(mode, simulationRun, i, MMASIterations, localIterations, mmas, csvFilePath);
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

    private void SaveResults(string mode, int simulationRun, int iteration, int MMASIterations, int localIterations, int bestTourLength, double localTourLength, string csvFilePath)
    {
        // var bestTourLength = mmas.GetBestTourLength();
        // var bestTour = mmas.GetBestTour();
        if (!File.Exists(csvFilePath))
        {
            using (StreamWriter writer = new StreamWriter(csvFilePath, true))
            {
                writer.WriteLine("Mode,SimulationRun,Checkpoints,MMASIterations,LocalIterations,BestTourLength,BestTourNodes");
            }
        }




        using (StreamWriter writer = new StreamWriter(csvFilePath, true))
        {
            writer.WriteLine($"{mode},{simulationRun},{iteration},{MMASIterations},{localIterations},{bestTourLength},{localTourLength}");
        }
    }
}
