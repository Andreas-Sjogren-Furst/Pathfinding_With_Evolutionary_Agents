// written by: Gustav Clausen s214940

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MMASTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void MMASTestSimplePasses()
    {




        string tspFilePath = "/Users/gustavsiphone/Documents/GitHub/Pathfinding_With_Evolutionary_Agents/Unity/Evolutionary_Agents_Simulation/Assets/Tests/EditModeTests/berlin52.tsp";
        string optimalTourFilePath = "/Users/gustavsiphone/Documents/GitHub/Pathfinding_With_Evolutionary_Agents/Unity/Evolutionary_Agents_Simulation/Assets/Tests/EditModeTests/berlin52.opt.tour";

        Graph graph = ReadTSPFile(tspFilePath);
        int[] optimalTour = ReadOptimalTour(optimalTourFilePath);
        double optimalTourLength = CalculateTourLength(optimalTour, graph);

        int numAnts = 52;
        double alpha = 1;
        double beta = 5;
        double rho = 0.90;
        double q = 100.0;
        int maxIterations = 500;

        double totalElapsedTime = 0.0;
        int successCount = 0;
        int iterations = 0;

        int numberofExperiments = 100;
        int experimentUID = 3;

        for (int i = 0; i < numberofExperiments; i++)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            MMAS mmas = new MMAS(numAnts, alpha, beta, rho, q, graph);
            mmas.SetGraph(graph);
            iterations += mmas.Run(maxIterations);

            stopwatch.Stop();
            totalElapsedTime += stopwatch.Elapsed.TotalMilliseconds;

            Node[] bestTour = mmas.GetBestTour();
            double bestTourLength = mmas.GetBestTourLength();

            // UnityEngine.Debug.Log("Best Tour: " + string.Join(" -> ", bestTour));
            UnityEngine.Debug.Log("Best Tour Length: " + bestTourLength + " (calculated again: " + optimalTourLength + ")");

            if (System.Math.Abs(bestTourLength - optimalTourLength) < 1e-6)
            {
                successCount++;
            }

            // Assert the results
            // Assert.AreEqual(optimalTour.Length, bestTour.Length);
            // Assert.LessOrEqual(bestTourLength, optimalTourLength);
        }

        double averageTime = totalElapsedTime / numberofExperiments;

        UnityEngine.Debug.Log("\n *** MMAS Test Results *** experiments: " + numberofExperiments + " ***" + "UID = " + experimentUID);
        UnityEngine.Debug.Log("Average Time for Success: " + averageTime + " ms");
        UnityEngine.Debug.Log("Success Count: " + successCount);
        UnityEngine.Debug.Log("Average Iterations: " + iterations / numberofExperiments);



    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator MMASTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }

    private Graph ReadTSPFile(string filePath)
    {
        UnityEngine.Debug.Log("Reading Nodes from TSP file");
        Graph graph = new Graph();
        string[] lines = File.ReadAllLines(filePath);
        int dimension = 0;
        bool readingNodes = false;

        foreach (string line in lines)
        {
            if (line.StartsWith("DIMENSION:"))
            {
                dimension = int.Parse(line.Split(':')[1].Trim());
            }
            else if (line.StartsWith("NODE_COORD_SECTION"))
            {
                readingNodes = true;
                continue;
            }
            else if (line.StartsWith("EOF"))
            {
                break;
            }

            if (readingNodes)
            {
                string[] parts = line.Split(new[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 3)
                {
                    int id = int.Parse(parts[0]) - 1;
                    double x = double.Parse(parts[1]);
                    double y = double.Parse(parts[2]);
                    // UnityEngine.Debug.Log("Node: " + id + " (" + x + ", " + y + ")");
                    graph.AddNode(new Node(id, x, y));
                }
            }
        }

        for (int i = 0; i < graph.Nodes.Count; i++)
        {
            for (int j = i + 1; j < graph.Nodes.Count; j++)
            {
                double distance = CalculateDistance(graph.Nodes[i], graph.Nodes[j]);
                graph.AddEdge(graph.Nodes[i], graph.Nodes[j], distance);
                graph.AddEdge(graph.Nodes[j], graph.Nodes[i], distance);
            }
        }

        return graph;
    }


    private double CalculateDistance(Node node1, Node node2)
    {
        double dx = node1.X - node2.X;
        double dy = node1.Y - node2.Y;
        return System.Math.Sqrt(dx * dx + dy * dy);
    }

    private int[] ReadOptimalTour(string filePath)
    {
        List<int> tour = new List<int>();
        string[] lines = File.ReadAllLines(filePath);
        bool readingTour = false;

        foreach (string line in lines)
        {
            if (line.StartsWith("TOUR_SECTION"))
            {
                readingTour = true;
                continue;
            }

            if (readingTour)
            {
                if (line.StartsWith("-1"))
                {
                    break;
                }

                string[] parts = line.Split(' ');
                foreach (string part in parts)
                {
                    if (!string.IsNullOrWhiteSpace(part))
                    {
                        int nodeId = int.Parse(part) - 1;
                        tour.Add(nodeId);
                        //    UnityEngine.Debug.Log("Opt tour node " + nodeId);
                    }
                }
            }
        }

        return tour.ToArray();
    }

    private double CalculateTourLength(int[] tour, Graph graph)
    {
        double length = 0.0;
        for (int i = 0; i < tour.Length - 1; i++)
        {
            Node node1 = graph.Nodes[tour[i]];
            Node node2 = graph.Nodes[tour[i + 1]];
            // Access the distance between consecutive nodes directly from the adjacency matrix
            double distance = graph.getEdge(node1, node2);
            if (distance < System.Double.MaxValue)
            {
                length += distance;
            }
            else
            {
                // Handle the case when there is no direct path between nodes
                // This scenario should not typically occur in a well-defined TSP
                throw new System.InvalidOperationException($"No direct path between nodes {tour[i]} and {tour[i + 1]}.");
            }
        }

        Node lastNode = graph.Nodes[tour[tour.Length - 1]];
        Node firstNode = graph.Nodes[tour[0]];

        // Add the distance from the last node back to the first node to complete the tour
        double distanceToFirst = graph.getEdge(lastNode, firstNode);
        if (distanceToFirst < System.Double.MaxValue)
        {
            length += distanceToFirst;
        }
        else
        {
            // Handle the case when there is no direct path from the last node back to the first
            throw new System.InvalidOperationException($"No direct path between nodes {tour[tour.Length - 1]} and {tour[0]}.");
        }

        return length;
    }



}
