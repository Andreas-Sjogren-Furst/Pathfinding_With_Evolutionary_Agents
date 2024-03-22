using System;
using System.Collections.Generic;
using System.Linq;

public class Node
{
    public int Id { get; set; }
    public double X { get; set; }
    public double Y { get; set; }

    public Node(int id, double x, double y)
    {
        Id = id;
        X = x;
        Y = y;
    }
}

public class Edge
{
    public Node Source { get; set; }
    public Node Destination { get; set; }
    public double Distance { get; set; }

    public Edge(Node source, Node destination, double distance)
    {
        Source = source;
        Destination = destination;
        Distance = distance;
    }
}

public class Graph
{
    public List<Node> Nodes { get; set; }
    // Removed the Edges list as it's replaced by the adjacency matrix.
    public double[,] AdjacencyMatrix { get; private set; } // Edges. 

    public Graph(int numberOfNodes)
    {
        Nodes = new List<Node>(numberOfNodes);
        AdjacencyMatrix = new double[numberOfNodes, numberOfNodes];

        // Initialize the adjacency matrix with Double.MaxValue to indicate no direct connection.
        for (int i = 0; i < numberOfNodes; i++)
        {
            for (int j = 0; j < numberOfNodes; j++)
            {
                if (i == j)
                    AdjacencyMatrix[i, j] = 0; // Distance to itself is 0.
                else
                    AdjacencyMatrix[i, j] = Double.MaxValue;
            }
        }
    }

    public void AddNode(Node node)
    {
        Nodes.Add(node);
    }

    public void AddEdge(Node source, Node destination, double distance)
    {
        int sourceIndex = Nodes.IndexOf(source);
        int destinationIndex = Nodes.IndexOf(destination);

        // Update the adjacency matrix to reflect the new edge.
        AdjacencyMatrix[sourceIndex, destinationIndex] = distance;
        AdjacencyMatrix[destinationIndex, sourceIndex] = distance; // For undirected graph. Remove this line for directed graph.
    }
}

public class Ant
{
    public int CurrentNode { get; set; }
    public HashSet<int> TabuList { get; set; }
    public double TourLength { get; set; }

    public Ant()
    {
        CurrentNode = -1;
        TabuList = new HashSet<int>(); // hashset offers o(1) for checking if it contains an element. 
        TourLength = 0.0;
    }
}

public class MMAS
{
    private readonly int _numAnts;
    private readonly double _alpha;
    private readonly double _beta;
    private readonly double _rho;
    private readonly double _q;
    private double _tauMax;
    private double _tauMin;

    private Graph _graph;
    private double[,] _pheromones;
    private int[] _bestTour;
    private double _bestTourLength;

    public MMAS(int numAnts, double alpha, double beta, double rho, double q)
    {
        _numAnts = numAnts;
        _alpha = alpha;
        _beta = beta;
        _rho = rho; // pheromone evaporation
        _q = q; // 

        _graph = null;
        _pheromones = null;
        _bestTour = null;
        _bestTourLength = double.MaxValue;
    }

    public void SetGraph(Graph graph)
    {
        _graph = graph;
        int numNodes = _graph.Nodes.Count;
        _tauMax = 1.0 / (_rho * GetNearestNeighborTourLength());
        _tauMin = _tauMax / (2.0 * numNodes);
        _pheromones = new double[numNodes, numNodes];
        InitializePheromones();
        _bestTour = new int[numNodes];
    }

    public int[] GetBestTour()
    {
        return _bestTour;
    }

    public double GetBestTourLength()
    {
        return _bestTourLength;
    }

    public void Run(int maxIterations) // The ants construct solutions concurrently. Avg. computation time of Berlin52 = 2.7 seconds on M1 Pro. 
    {
        for (int iteration = 0; iteration < maxIterations; iteration++)
        {
            Ant[] ants = new Ant[_numAnts];
            int[][] antTours = new int[_numAnts][];
            double[] antTourLengths = new double[_numAnts];

            System.Threading.Tasks.Parallel.For(0, _numAnts, i =>
            {
                ants[i] = new Ant();
                antTours[i] = BuildTour(ants[i], i);
                antTourLengths[i] = CalculateTourLength(antTours[i]);
            });

            // Find the best tour after all ants have finished their tours
            int bestIndex = Array.IndexOf(antTourLengths, antTourLengths.Min());
            if (antTourLengths[bestIndex] < _bestTourLength)
            {
                _bestTour = antTours[bestIndex];
                _bestTourLength = antTourLengths[bestIndex];
            }

            UpdatePheromones(antTours, antTourLengths);
            ApplyPheromoneTrailLimits();
        }
    }


    private void InitializePheromones()
    {
        for (int i = 0; i < _graph.Nodes.Count; i++)
        {
            for (int j = 0; j < _graph.Nodes.Count; j++)
            {
                _pheromones[i, j] = _tauMax;
            }
        }
    }

    private int[] BuildTour(Ant ant, int startNode)
    {
        int numNodes = _graph.Nodes.Count;
        int[] tour = new int[numNodes];
        ant.TabuList.Clear();
        // This will generate a random integer between 1 and 10 (1 inclusive, 10 exclusive)
        // int randomNumber = UnityEngine.Random.Range(0, numNodes);
        // UnityEngine.Debug.Log("random number " + randomNumber);

        ant.CurrentNode = _graph.Nodes[startNode].Id; // Start at a random node. 
        ant.TabuList.Add(ant.CurrentNode);
        tour[0] = ant.CurrentNode;

        for (int i = 1; i < numNodes; i++)
        {
            ant.CurrentNode = SelectNextNode(ant);
            ant.TabuList.Add(ant.CurrentNode);
            tour[i] = ant.CurrentNode;
        }

        return tour;
    }

    private int SelectNextNode(Ant ant)
    {
        int currentNode = ant.CurrentNode;
        double[] probabilities = new double[_graph.Nodes.Count];
        double sum = 0.0;

        for (int i = 0; i < _graph.Nodes.Count; i++)
        {
            if (!ant.TabuList.Contains(i) && _graph.AdjacencyMatrix[currentNode, i] < Double.MaxValue)
            {
                // Directly access the distance from the adjacency matrix
                double distance = _graph.AdjacencyMatrix[currentNode, i];
                // Calculate the probability of moving to node i
                probabilities[i] = Math.Pow(_pheromones[currentNode, i], _alpha) * Math.Pow(1.0 / distance, _beta); // the heuritct value for Nij is 1/Jij where J is the distance. Since shorter distance, will give higher heuristic. 
                sum += probabilities[i];
            }
        }

        double random = new Random().NextDouble() * sum;
        double cumulativeProbability = 0.0;

        for (int i = 0; i < _graph.Nodes.Count; i++)
        {
            if (!ant.TabuList.Contains(i))
            {
                cumulativeProbability += probabilities[i];
                if (cumulativeProbability >= random)
                {
                    return i;
                }
            }
        }

        return -1;
    }

    private double CalculateTourLength(int[] tour)
    {
        double length = 0.0;
        for (int i = 0; i < tour.Length - 1; i++)
        {
            // Directly access the distance between consecutive nodes in the tour from the adjacency matrix
            double distance = _graph.AdjacencyMatrix[tour[i], tour[i + 1]];
            if (distance < Double.MaxValue)
            {
                length += distance;
            }
            else
            {
                // Handle the case where there is no direct path between consecutive nodes in the tour
                // This case should theoretically not occur in a valid tour, but the check is here for completeness
                throw new InvalidOperationException($"No direct path between nodes {tour[i]} and {tour[i + 1]}.");
            }
        }
        // Add the distance from the last node back to the first to complete the tour
        double lastDistance = _graph.AdjacencyMatrix[tour[tour.Length - 1], tour[0]];
        if (lastDistance < Double.MaxValue)
        {
            length += lastDistance;
        }
        else
        {
            // Handle the case where there is no direct path from the last node back to the first
            throw new InvalidOperationException($"No direct path between nodes {tour[tour.Length - 1]} and {tour[0]}.");
        }
        return length;
    }


    private void UpdatePheromones(int[][] antTours, double[] antTourLengths)
    {
        for (int i = 0; i < _graph.Nodes.Count; i++)
        {
            for (int j = 0; j < _graph.Nodes.Count; j++)
            {
                _pheromones[i, j] *= _rho;
            }
        }

        int[] bestTour = antTours[Array.IndexOf(antTourLengths, antTourLengths.Min())];
        double bestTourLength = antTourLengths.Min();

        for (int i = 0; i < _graph.Nodes.Count - 1; i++)
        {
            _pheromones[bestTour[i], bestTour[i + 1]] += _q / bestTourLength;
            _pheromones[bestTour[i + 1], bestTour[i]] += _q / bestTourLength;
        }

        _pheromones[bestTour[_graph.Nodes.Count - 1], bestTour[0]] += _q / bestTourLength;
        _pheromones[bestTour[0], bestTour[_graph.Nodes.Count - 1]] += _q / bestTourLength;
    }

    private void ApplyPheromoneTrailLimits()
    {
        for (int i = 0; i < _graph.Nodes.Count; i++)
        {
            for (int j = 0; j < _graph.Nodes.Count; j++)
            {
                _pheromones[i, j] = Math.Max(_tauMin, Math.Min(_pheromones[i, j], _tauMax));
            }
        }
    }

    private double GetNearestNeighborTourLength()
    {
        bool[] visited = new bool[_graph.Nodes.Count];
        int currentNode = 0; // Starting from the first node
        double tourLength = 0.0;
        visited[currentNode] = true;

        for (int i = 1; i < _graph.Nodes.Count; i++)
        {
            int nearestNode = -1;
            double minDistance = double.MaxValue;

            for (int j = 0; j < _graph.Nodes.Count; j++)
            {
                // Directly use the adjacency matrix to get the distance
                double distance = _graph.AdjacencyMatrix[currentNode, j];
                if (!visited[j] && distance < minDistance)
                {
                    nearestNode = j;
                    minDistance = distance;
                }
            }

            if (nearestNode != -1)
            {
                visited[nearestNode] = true;
                tourLength += minDistance;
                currentNode = nearestNode;
            }
            else
            {
                // If no unvisited node is found (which should not happen in a well-defined TSP)
                break;
            }
        }

        // Add the distance from the last node back to the first to complete the tour
        if (_graph.AdjacencyMatrix[currentNode, 0] < Double.MaxValue)
        {
            tourLength += _graph.AdjacencyMatrix[currentNode, 0];
        }
        else
        {
            // Handle the case where returning to the start node is not possible
            // This should theoretically not happen in a complete graph used for TSP
            throw new InvalidOperationException("Cannot return to start node from last node.");
        }

        return tourLength;
    }



}