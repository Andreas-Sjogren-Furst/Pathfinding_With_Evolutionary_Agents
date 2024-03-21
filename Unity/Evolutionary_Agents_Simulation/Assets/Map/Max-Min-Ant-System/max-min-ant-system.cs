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
    public List<Edge> Edges { get; set; }

    public Graph()
    {
        Nodes = new List<Node>();
        Edges = new List<Edge>();
    }

    public void AddNode(Node node)
    {
        Nodes.Add(node);
    }

    public void AddEdge(Edge edge)
    {
        Edges.Add(edge);
    }
}

public class Ant
{
    public int CurrentNode { get; set; }
    public List<int> TabuList { get; set; }
    public double TourLength { get; set; }

    public Ant()
    {
        CurrentNode = -1;
        TabuList = new List<int>();
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

        _graph = new Graph();
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

    public void Run(int maxIterations)
    {
        for (int iteration = 0; iteration < maxIterations; iteration++)
        {
            List<Ant> ants = new List<Ant>();
            for (int i = 0; i < _numAnts; i++)
            {
                ants.Add(new Ant());
            }

            int[][] antTours = new int[_numAnts][];
            double[] antTourLengths = new double[_numAnts];

            for (int i = 0; i < _numAnts; i++)
            {
                int[] tour = BuildTour(ants[i]);
                double tourLength = CalculateTourLength(tour);
                antTours[i] = tour;
                antTourLengths[i] = tourLength;

                if (tourLength < _bestTourLength)
                {
                    _bestTour = tour;
                    _bestTourLength = tourLength;
                }
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

    private int[] BuildTour(Ant ant)
    {
        int numNodes = _graph.Nodes.Count;
        int[] tour = new int[numNodes];
        ant.TabuList.Clear();
        // This will generate a random integer between 1 and 10 (1 inclusive, 10 exclusive)
        int randomNumber = UnityEngine.Random.Range(0, numNodes);
        // UnityEngine.Debug.Log("random number " + randomNumber);

        ant.CurrentNode = _graph.Nodes[randomNumber].Id; // Start at a random node. 
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
            if (!ant.TabuList.Contains(i))
            {
                Edge edge = _graph.Edges.Find(e => e.Source.Id == currentNode && e.Destination.Id == i);
                if (edge != null)
                {
                    probabilities[i] = Math.Pow(_pheromones[currentNode, i], _alpha) *
                                       Math.Pow(1.0 / edge.Distance, _beta);
                    sum += probabilities[i];
                }
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
            Edge edge = _graph.Edges.Find(e => e.Source.Id == tour[i] && e.Destination.Id == tour[i + 1]);
            length += edge.Distance;
        }
        Edge lastEdge = _graph.Edges.Find(e => e.Source.Id == tour[tour.Length - 1] && e.Destination.Id == tour[0]);
        length += lastEdge.Distance;
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
        int currentNode = 0;
        double tourLength = 0.0;
        visited[currentNode] = true;

        for (int i = 1; i < _graph.Nodes.Count; i++)
        {
            int nearestNode = -1;
            double minDistance = double.MaxValue;
            for (int j = 0; j < _graph.Nodes.Count; j++)
            {
                if (!visited[j])
                {
                    Edge edge = _graph.Edges.Find(e => (e.Source.Id == currentNode && e.Destination.Id == j) ||
                                                       (e.Source.Id == j && e.Destination.Id == currentNode));
                    if (edge != null && edge.Distance < minDistance)
                    {
                        nearestNode = j;
                        minDistance = edge.Distance;
                    }
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
                // Handle the case when no unvisited node is found
                // Find the nearest unvisited node by calculating the distances directly
                double minDistanceToUnvisited = double.MaxValue;
                for (int j = 0; j < _graph.Nodes.Count; j++)
                {
                    if (!visited[j])
                    {
                        double distance = CalculateDistance(_graph.Nodes[currentNode], _graph.Nodes[j]);
                        if (distance < minDistanceToUnvisited)
                        {
                            nearestNode = j;
                            minDistanceToUnvisited = distance;
                        }
                    }
                }

                if (nearestNode != -1)
                {
                    visited[nearestNode] = true;
                    tourLength += minDistanceToUnvisited;
                    currentNode = nearestNode;
                }
                else
                {
                    // All nodes have been visited
                    break;
                }
            }
        }

        // Connect the last node back to the first node
        double distanceToFirst = CalculateDistance(_graph.Nodes[currentNode], _graph.Nodes[0]);
        tourLength += distanceToFirst;

        return tourLength;
    }

    private double CalculateDistance(Node node1, Node node2)
    {
        double dx = node1.X - node2.X;
        double dy = node1.Y - node2.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

}