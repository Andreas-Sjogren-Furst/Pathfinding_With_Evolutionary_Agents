using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

// public class NodePair : IEquatable<NodePair>
// {
//     public Node Node1 { get; }
//     public Node Node2 { get; }

//     public NodePair(Node node1, Node node2)
//     {
//         Node1 = node1;
//         Node2 = node2;
//     }

//     public bool Equals(NodePair other)
//     {
//         return other != null && Node1.Id == other.Node1.Id && Node2.Id == other.Node2.Id;
//     }

//     public override int GetHashCode()
//     {
//         return Node1.Id + Node2.Id;
//     }
// }


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

    public override int GetHashCode()
    {
        // Create a hash code that is based on the position.
        // You might use a combination of x and y coordinates to do this.
        return Id;
    }

    public override bool Equals(object obj)
    {
        if (obj is Node other)
        {
            return this.Id == other.Id;
        }
        return false;
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


    public override int GetHashCode()
    {
        return Source.Id * 1000 + Destination.Id;
    }

    public override bool Equals(object obj)
    {
        if (obj is Edge other)
        {
            return this.Source.Id == other.Source.Id && this.Destination.Id == other.Destination.Id;
        }
        return false;
    }

    //  any time => ændrer på objektiv funktionen, og bruge den tidligere løsning, som warm start løsning. 
    //  Hvor lang tid tager før systemet tiltager sig.
    //  Sammenlignet med at det starte helt. 
}

public class Graph
{
    public List<Node> Nodes { get; set; } // nodes. 
    // public double[,] AdjacencyMatrix { get; set; } // Edges.

    public Dictionary<int, double> Edges { get; set; } // Edges.



    public Graph()
    {
        Nodes = new List<Node>();
        Edges = new Dictionary<int, double>();
    }

    public void AddNode(Node node)
    {
        Nodes.Add(node);
    }

    public void RemoveNode(Node node)
    {
        Nodes.Remove(node);
    }

    public void AddEdge(Node source, Node destination, double distance)
    {
        int key = source.Id * 1000 + destination.Id;
        Edges[key] = distance;
    }

    public double getEdge(Node source, Node destination)
    {
        int key = source.Id * 1000 + destination.Id;
        if (Edges.TryGetValue(key, out double distance))
        {
            return distance;
        }
        return Double.MaxValue;
    }

    public void RemoveEdge(Node source, Node destination)
    {
        int key = source.Id * 1000 + destination.Id;
        Edges.Remove(key);
    }

    public void RemoveIsolatedNodes()
    {
        List<Node> isolatedNodes = new List<Node>();

        foreach (Node node in Nodes)
        {
            bool isConnected = false;
            foreach (Node otherNode in Nodes)
            {
                if (node != otherNode && (getEdge(node, otherNode) < Double.MaxValue || getEdge(otherNode, node) < Double.MaxValue))
                {
                    isConnected = true;
                    break;
                }
            }

            if (!isConnected)
            {
                isolatedNodes.Add(node);
            }
        }

        foreach (Node isolatedNode in isolatedNodes)
        {
            RemoveNode(isolatedNode);
        }
    }


    public bool IsFullyConnected()
    {
        var visited = new HashSet<Node>();
        var toVisit = new Queue<Node>();

        if (Nodes.Count == 0) return true;

        toVisit.Enqueue(Nodes[0]);
        while (toVisit.Count > 0)
        {
            var node = toVisit.Dequeue();
            if (!visited.Contains(node))
            {
                visited.Add(node);
                foreach (var otherNode in Nodes)
                {
                    if (node != otherNode && (getEdge(node, otherNode) < Double.MaxValue || getEdge(otherNode, node) < Double.MaxValue))
                    {
                        toVisit.Enqueue(otherNode);
                    }
                }
            }
        }
        return visited.Count == Nodes.Count;
    }


}

public class Ant
{
    public Node CurrentNode { get; set; }
    public HashSet<Node> TabuList { get; set; }
    public double TourLength { get; set; }

    public Ant()
    {
        CurrentNode = null;
        TabuList = new HashSet<Node>(); // hashset offers o(1) for checking if it contains an element. 
        TourLength = 0.0;
    }
}

public class MMAS
{
    public int _numAnts;
    private readonly double _alpha;
    private readonly double _beta;
    private readonly double _rho;
    private readonly double _q;
    public double _tauMax { get; private set; }
    public double _tauMin { get; private set; }

    public Graph _graph;
    public Dictionary<int, double> _pheromones { get; private set; }
    private Node[] _bestTour;
    private double _bestTourLength;

    private double previousPheromoneSum = 0;
    private const double convergenceThreshold = 0.0001; // Threshold to determine convergence
    private const int convergenceCountRequired = 10; // Number of consecutive iterations required for convergence
    private int convergenceCount = 0;


    private int GetPheromoneKey(Node source, Node destination)
    {
        return source.Id * 1000 + destination.Id;
    }

    public double getPheromone(Node source, Node destination)
    {
        int key = GetPheromoneKey(source, destination);
        if (_pheromones.TryGetValue(key, out double pheromone))
        {
            return pheromone;
        }
        return 0; // Return 0 if no pheromone value is found, indicating no pheromone has been deposited.
    }


    public void setPheromone(Node source, Node destination, double pheromone)
    {
        int key = GetPheromoneKey(source, destination);
        _pheromones[key] = pheromone; // Dictionary will automatically add or overwrite the key-value pair
    }
    public void removePheromone(Node source, Node destination)
    {
        int key = GetPheromoneKey(source, destination);
        _pheromones.Remove(key); // Remove the key if it exists
    }



    public MMAS(int numAnts, double alpha, double beta, double rho, double q, Graph graph)
    {
        _graph = graph;
        _numAnts = numAnts;
        _alpha = alpha;
        _beta = beta;
        _rho = rho; // pheromone evaporation
        _q = q; // 
        _graph = graph;

        _graph = null;
        _pheromones = null;
        _bestTour = null;
        _bestTourLength = double.MaxValue;
    }

    public void SetGraph(Graph graph)
    {
        _graph = graph;
        int numNodes = _graph.Nodes.Count;
        _numAnts = numNodes;
        _tauMax = 1.0 / (_rho * GetNearestNeighborTourLength());
        _tauMin = _tauMax / (2.0 * numNodes);
        _pheromones = new Dictionary<int, double>();
        InitializePheromones();
        _bestTourLength = double.MaxValue;
        _bestTour = new Node[numNodes];
    }

    public Node[] GetBestTour()
    {
        return _bestTour;
    }

    public double GetBestTourLength()
    {
        return _bestTourLength;
    }

    // TODO: MMAS, dynamisk, hvis en edge forsvinder, kan man nemt compute det restende af turen. 
    public int Run(int maxIterations) // O(I*N^2) // The ants construct solutions concurrently. Avg. computation time of Berlin52 = 2.7 seconds on M1 Pro. 
    {
        int iterations = 0;
        for (iterations = 0; iterations < maxIterations; iterations++)
        {
            Ant[] ants = new Ant[_numAnts];
            Node[][] antTours = new Node[_numAnts][];
            double[] antTourLengths = new double[_numAnts];

            System.Threading.Tasks.Parallel.For(0, _numAnts, i =>
            {
                ants[i] = new Ant();
                antTours[i] = BuildTour(ants[i], i); // O(N^2) // ybdersøg om deg(v)=d O(N*d)
                antTourLengths[i] = CalculateTourLength(antTours[i]); // O(N)
            });

            // Find the best tour after all ants have finished their tours
            int bestIndex = Array.IndexOf(antTourLengths, antTourLengths.Min());
            if (antTourLengths[bestIndex] < _bestTourLength)
            {
                _bestTour = antTours[bestIndex];
                _bestTourLength = antTourLengths[bestIndex];
            }

            UpdatePheromones(antTours, antTourLengths); // O(N^2)
            ApplyPheromoneTrailLimits(); // O(N^2)

            // check convergence. 

            double currentPheromoneSum = SumAllPheromones();
            if (Math.Abs(currentPheromoneSum - previousPheromoneSum) <= convergenceThreshold)
            {
                convergenceCount++;
                if (convergenceCount >= convergenceCountRequired)
                {
                    UnityEngine.Debug.Log("Convergence reached after " + iterations + " iterations.");
                    break; // Early stopping, this signifcanlty increased the speed of the algorithm from 5 sec to 0.5 sec
                }
            }
            else
            {
                convergenceCount = 0; // Reset if changes are above the threshold
            }
            previousPheromoneSum = currentPheromoneSum;


        }
        return iterations;

    }


    private double SumAllPheromones()
    {
        double sum = 0;
        foreach (var pheromone in _pheromones.Values)
        {
            sum += pheromone;
        }
        return sum;
    }


    public void AddNode(Node node)
    {
        if (!_graph.Nodes.Contains(node))
        {
            _graph.AddNode(node);
            // _numAnts = _graph.Nodes.Count;
            // recalculate tau min and tau max
            int numNodes = _graph.Nodes.Count;
            _tauMax = 1.0 / (_rho * GetNearestNeighborTourLength());
            _tauMin = _tauMax / (2.0 * numNodes);
            _bestTourLength = double.MaxValue;
            _bestTour = new Node[numNodes];

            // Initialize pheromones for new connections
            foreach (Node existingNode in _graph.Nodes)
            {
                if (existingNode != node)
                {
                    int keyForward = GetPheromoneKey(node, existingNode);
                    int keyBackward = GetPheromoneKey(existingNode, node);
                    _pheromones[keyForward] = _tauMax;  // Or some initial value
                    _pheromones[keyBackward] = _tauMax; // Or some initial value
                }
            }
        }
    }

    public void RemoveNode(Node node)
    {
        if (_graph.Nodes.Contains(node))
        {
            // Remove all edges and pheromones associated with this node
            foreach (Node otherNode in _graph.Nodes)
            {
                if (otherNode != node)
                {
                    removePheromone(node, otherNode);
                    removePheromone(otherNode, node);
                    _graph.RemoveEdge(node, otherNode);
                    _graph.RemoveEdge(otherNode, node);
                }
            }
            _graph.RemoveNode(node);
            _bestTourLength = double.MaxValue;
            _bestTour = new Node[_graph.Nodes.Count];
            // _numAnts = _graph.Nodes.Count;

        }
    }

    public void AddEdge(Node source, Node destination, double distance)
    {
        if (!_graph.Edges.ContainsKey(GetPheromoneKey(source, destination)))
        {
            _graph.AddEdge(source, destination, distance);
            setPheromone(source, destination, _tauMax); // Initialize with some pheromone level, usually _tauMax
        }
    }

    public void RemoveEdge(Node source, Node destination)
    {
        int key = GetPheromoneKey(source, destination);
        if (_graph.Edges.ContainsKey(key))
        {
            _graph.RemoveEdge(source, destination);
            removePheromone(source, destination);
        }
    }


    private void InitializePheromones()
    {
        for (int i = 0; i < _graph.Nodes.Count; i++)
        {
            Node nodei = _graph.Nodes[i];
            for (int j = 0; j < _graph.Nodes.Count; j++)
            {
                Node nodej = _graph.Nodes[j];
                setPheromone(nodei, nodej, _tauMax);
            }
        }
    }

    private Node[] BuildTour(Ant ant, int startNode)
    {
        int numNodes = _graph.Nodes.Count;
        Node[] tour = new Node[numNodes];
        ant.TabuList.Clear();
        // This will generate a random integer between 1 and 10 (1 inclusive, 10 exclusive)
        // int randomNumber = UnityEngine.Random.Range(0, numNodes);
        // UnityEngine.Debug.Log("random number " + randomNumber);

        ant.CurrentNode = _graph.Nodes[startNode]; // Start at a random node. 
        ant.TabuList.Add(ant.CurrentNode);
        tour[0] = ant.CurrentNode;

        for (int i = 1; i < numNodes; i++)
        {
            ant.CurrentNode = SelectNextNode(ant); // O(N)
            ant.TabuList.Add(ant.CurrentNode);
            tour[i] = ant.CurrentNode;
        }

        return tour;
    }

    private Node SelectNextNode(Ant ant)
    {
        Node currentNode = ant.CurrentNode;
        double[] probabilities = new double[_graph.Nodes.Count];
        double sum = 0.0;

        for (int i = 0; i < _graph.Nodes.Count; i++)
        {
            Node nextNode = _graph.Nodes[i];
            // UnityEngine.Debug.Log("current node" + currentNode);
            if (!ant.TabuList.Contains(nextNode) && _graph.getEdge(currentNode, nextNode) < Double.MaxValue)
            {
                // Directly access the distance from the adjacency matrix
                double distance = _graph.getEdge(currentNode, nextNode);
                // Calculate the probability of moving to node i
                probabilities[i] = Math.Pow(getPheromone(currentNode, nextNode), _alpha) * Math.Pow(1.0 / distance, _beta); // the heuritct value for Nij is 1/Jij where J is the distance. Since shorter distance, will give higher heuristic. 
                sum += probabilities[i];
            }
        }

        double random = new System.Random().NextDouble() * sum;
        double cumulativeProbability = 0.0;

        for (int i = 0; i < _graph.Nodes.Count; i++) // sorterings algortime, til at sortere kummulerede sandsynligheder. 
        { //  Så de større muligheder rammer først.
            Node nextNode = _graph.Nodes[i];


            if (!ant.TabuList.Contains(nextNode))
            {
                cumulativeProbability += probabilities[i];
                if (cumulativeProbability >= random)
                {
                    return nextNode;
                }
            }
        }

        return null;
    }

    private double CalculateTourLength(Node[] tour)
    {
        double length = 0.0;
        for (int i = 0; i < tour.Length - 1; i++)
        {
            // Directly access the distance between consecutive nodes in the tour from the adjacency matrix
            double distance = _graph.getEdge(tour[i], tour[i + 1]);
            if (distance < Double.MaxValue)
            {
                length += distance;
            }
            else
            {
                // Handle the case where there is no direct path between consecutive nodes in the tour
                // This case should theoretically not occur in a valid tour, but the check is here for completeness
                throw new InvalidOperationException($"No direct path between nodes {tour[i].Id} and {tour[i + 1].Id}.");
            }
        }
        // Add the distance from the last node back to the first to complete the tour
        double lastDistance = _graph.getEdge(tour[tour.Length - 1], tour[0]);
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





    private void UpdatePheromones(Node[][] antTours, double[] antTourLengths)
    {
        for (int i = 0; i < _graph.Nodes.Count; i++)
        {
            for (int j = 0; j < _graph.Nodes.Count; j++)
            {
                Node sourceNode = _graph.Nodes[i];
                Node destinationNode = _graph.Nodes[j];
                double currentPheromone = getPheromone(sourceNode, destinationNode);
                setPheromone(sourceNode, destinationNode, currentPheromone * _rho);

                // _pheromones[i, j] *= _rho;
            }
        }

        Node[] bestTour = antTours[Array.IndexOf(antTourLengths, antTourLengths.Min())];
        double bestTourLength = antTourLengths.Min();

        // Pheromone reinforcement for each edge in the best tour
        for (int i = 0; i < _graph.Nodes.Count - 1; i++)
        {
            Node sourceNode = bestTour[i];
            Node destinationNode = bestTour[i + 1];

            // Update source to destination
            double currentPheromone = getPheromone(sourceNode, destinationNode);
            setPheromone(sourceNode, destinationNode, currentPheromone + _q / bestTourLength);

            // Update destination to source for undirected graph symmetry
            currentPheromone = getPheromone(destinationNode, sourceNode);
            setPheromone(destinationNode, sourceNode, currentPheromone + _q / bestTourLength);
        }

        // Closing the tour: last node to the first node in the tour
        Node lastNode = bestTour[_graph.Nodes.Count - 1];
        Node firstNode = bestTour[0];
        double closingPheromone = getPheromone(lastNode, firstNode);
        setPheromone(lastNode, firstNode, closingPheromone + _q / bestTourLength);
        closingPheromone = getPheromone(firstNode, lastNode);
        setPheromone(firstNode, lastNode, closingPheromone + _q / bestTourLength);


        // int[] bestTour = antTours[Array.IndexOf(antTourLengths, antTourLengths.Min())];
        // double bestTourLength = antTourLengths.Min();

        // for (int i = 0; i < _graph.Nodes.Count - 1; i++)
        // {
        //     _pheromones[bestTour[i], bestTour[i + 1]] += _q / bestTourLength;
        //     _pheromones[bestTour[i + 1], bestTour[i]] += _q / bestTourLength;
        // }

        // _pheromones[bestTour[_graph.Nodes.Count - 1], bestTour[0]] += _q / bestTourLength;
        // _pheromones[bestTour[0], bestTour[_graph.Nodes.Count - 1]] += _q / bestTourLength;
    }

    private void ApplyPheromoneTrailLimits()
    {
        for (int i = 0; i < _graph.Nodes.Count; i++)
        {
            Node nodei = _graph.Nodes[i];
            for (int j = 0; j < _graph.Nodes.Count; j++)
            {
                Node nodej = _graph.Nodes[j];
                setPheromone(nodei, nodej, Math.Max(_tauMin, Math.Min(getPheromone(nodei, nodej), _tauMax)));
            }
        }
    }

    private double GetNearestNeighborTourLength()
    {
        bool[] visited = new bool[_graph.Nodes.Count];
        Node currentNode = _graph.Nodes[0]; // Starting from the first node
        double tourLength = 0.0;
        visited[currentNode.Id] = true;

        for (int i = 1; i < _graph.Nodes.Count; i++)
        {
            Node nearestNode = null;
            double minDistance = double.MaxValue;

            for (int j = 0; j < _graph.Nodes.Count; j++)
            {
                Node nodej = _graph.Nodes[j];
                // Directly use the adjacency matrix to get the distance
                double distance = _graph.getEdge(currentNode, nodej);
                if (!visited[j] && distance < minDistance)
                {
                    nearestNode = nodej;
                    minDistance = distance;
                }
            }

            if (nearestNode != null)
            {
                visited[nearestNode.Id] = true;
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
        if (_graph.getEdge(currentNode, _graph.Nodes[0]) < Double.MaxValue)
        {
            tourLength += _graph.getEdge(currentNode, _graph.Nodes[0]);
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