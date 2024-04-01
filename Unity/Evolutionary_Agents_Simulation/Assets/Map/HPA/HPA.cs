using System;
using System.Collections.Generic;
using System.Linq;
using Codice.Client.BaseCommands;
using UnityEngine;

public class HPAStarGraphConstruction
{
    private Dictionary<int, HashSet<Entrance>> EntrancesByLevel { get; set; } // E 
    public Dictionary<int, List<Cluster>> ClusterByLevel { get; set; } // C
    public Dictionary<int, List<HPANode>> NodesByLevel { get; set; } // N
    public Dictionary<int, HPAEdge[,]> AdjacencyMatrices { get; set; } // G

    public void AbstractMaze()
    {
        EntrancesByLevel = new Dictionary<int, HashSet<Entrance>>();
        ClusterByLevel = new Dictionary<int, List<Cluster>>();
        ClusterByLevel[1] = BuildClusters(1);

        foreach (var c1 in ClusterByLevel[1])
        {
            foreach (var c2 in ClusterByLevel[1])
            {
                if (Adjacent(c1, c2))
                {
                    var entrances = BuildEntrances(c1, c2);
                    EntrancesByLevel[1].UnionWith(entrances);
                }
            }
        }
    }

    public void BuildGraph()
    {
        NodesByLevel = new Dictionary<int, List<HPANode>>();
        AdjacencyMatrices = new Dictionary<int, HPAEdge[,]>();

        foreach (var e in EntrancesByLevel[1])
        {
            var c1 = GetCluster(e, 1);
            var c2 = GetCluster(e, 1);
            var n1 = NewHPANode(e, c1);
            var n2 = NewHPANode(e, c2);
            AddHPANode(n1, 1);
            AddHPANode(n2, 1);
            AddHPAEdge(n1, n2, 1, 1, HPAEdgeType.INTER);
        }

        foreach (Cluster c in ClusterByLevel[1])
        {
            foreach (HPANode n1 in c.Nodes)
            {
                foreach (HPANode n2 in c.Nodes)
                {
                    if (n1.Id != n2.Id)
                    {
                        var d = SearchForDistance(n1, n2, c);
                        if (d < double.PositiveInfinity)
                        {
                            AddHPAEdge(n1, n2, 1, d, HPAEdgeType.INTRA);
                        }
                    }
                }
            }
        }
    }

    public void AddLevelToGraph(int l)
    {
        ClusterByLevel[l] = BuildClusters(l);

        foreach (Cluster c1 in ClusterByLevel[l])
        {
            foreach (Cluster c2 in ClusterByLevel[l])
            {
                if (!Adjacent(c1, c2))
                    continue;

                foreach (Entrance e in GetEntrances(c1, c2))
                {
                    SetLevel(GetHPANode1(e), l);
                    SetLevel(GetHPANode2(e), l);
                    SetLevel(GetHPAEdge(e), l);
                }
            }
        }

        foreach (Cluster c in ClusterByLevel[l])
        {
            foreach (HPANode n1 in c.Nodes)
            {
                foreach (HPANode n2 in c.Nodes)
                {
                    if (n1 != n2)
                    {
                        var d = SearchForDistance(n1, n2, c);
                        if (d < double.PositiveInfinity)
                        {
                            AddHPAEdge(n1, n2, l, d, HPAEdgeType.INTRA);
                        }
                    }
                }
            }
        }
    }

    private void SetLevel(HPAEdge hPAEdge, int l)
    {
        throw new NotImplementedException();
    }

    public void Preprocessing(int maxLevel)
    {
        AbstractMaze();
        BuildGraph();

        for (int l = 2; l <= maxLevel; l++)
        {
            AddLevelToGraph(l);
        }
    }


    // ... (previous code remains the same)

    // Helper methods
    private List<Cluster> BuildClusters(int level)
    {
        // Implementation to build clusters at the specified level
        // This method should create and return a list of clusters based on the level
        // You can use your own logic to determine how to group nodes into clusters
        // For example, you can use a grid-based approach or any other clustering algorithm
        // we will use a grid based approach, where each level is divided into a grid of clusters
        // the size of the clusters is 5 * level. 
        // Return the list of created clusters

        List<Cluster> clusters = new List<Cluster>();

        // Determine the size of each cluster based on the level
        int clusterSize = 5 * level;

        // Get the dimensions of the grid
        int gridWidth = 100 / clusterSize;
        int gridHeight = 100 / clusterSize;

        // Create clusters based on the grid
        for (int row = 0; row < gridHeight; row++)
        {
            for (int col = 0; col < gridWidth; col++)
            {
                Cluster cluster = new Cluster();
                cluster.Id = Guid.NewGuid();
                cluster.Level = level;
                cluster.Nodes = new List<HPANode>();
                cluster.Entrances = new List<Entrance>();

                // Determine the boundaries of the cluster
                int startX = col * clusterSize;
                int startY = row * clusterSize;
                int endX = startX + clusterSize - 1;
                int endY = startY + clusterSize - 1;

                // Add nodes to the cluster based on their positions
                foreach (HPANode node in NodesByLevel[level])
                {
                    if (node.Position.x >= startX && node.Position.x <= endX &&
                        node.Position.y >= startY && node.Position.y <= endY)
                    {
                        cluster.Nodes.Add(node);
                        node.Cluster = cluster;
                    }
                }

                clusters.Add(cluster);
            }
        }

        return clusters;

    }

    private bool Adjacent(Cluster c1, Cluster c2)
    {
        // Implementation to check if two clusters are adjacent
        // This method should determine if there is a direct connection between the two clusters
        // You can use your own criteria to define adjacency. 
        // The clusters are adjacent if they share a common boundary
        // Return true if the clusters are adjacent, false otherwise

        // Check if the clusters are at the same level
        if (c1.Level != c2.Level)
            return false;

        // Get the boundaries of the clusters
        int c1StartX = (int)c1.Nodes.Min(node => node.Position.x);
        int c1StartY = (int)c1.Nodes.Min(node => node.Position.y);
        int c1EndX = (int)c1.Nodes.Max(node => node.Position.x);
        int c1EndY = (int)c1.Nodes.Max(node => node.Position.y);

        int c2StartX = (int)c2.Nodes.Min(node => node.Position.x);
        int c2StartY = (int)c2.Nodes.Min(node => node.Position.y);
        int c2EndX = (int)c2.Nodes.Max(node => node.Position.x);
        int c2EndY = (int)c2.Nodes.Max(node => node.Position.y);

        // Check if the clusters share a common boundary
        if (c1StartX == c2EndX + 1 || c1EndX + 1 == c2StartX ||
            c1StartY == c2EndY + 1 || c1EndY + 1 == c2StartY)
        {
            return true;
        }

        return false;


    }

    private HashSet<Entrance> BuildEntrances(Cluster c1, Cluster c2)
    {
        // Implementation to build entrances between two adjacent clusters
        // This method should create and return a set of entrances that connect the two clusters
        // Entrances are shared nodes between two adjacent clusters that allow movement between them
        // Create Entrance objects and set their properties (Id, Cluster1, Cluster2, Node1, Node2)
        // Return the set of created entrances

        HashSet<Entrance> entrances = new HashSet<Entrance>();

        // Check if the clusters are adjacent
        if (!Adjacent(c1, c2))
            return entrances;

        // Find the shared nodes between the two clusters
        var sharedNodes = c1.Nodes.Intersect(c2.Nodes).ToList();

        // Create entrances for each pair of shared nodes
        for (int i = 0; i < sharedNodes.Count - 1; i++)
        {
            HPANode node1 = sharedNodes[i];
            HPANode node2 = sharedNodes[i + 1];

            // Create a new entrance
            Entrance entrance = new Entrance();
            entrance.Id = Guid.NewGuid();
            entrance.Cluster1 = c1;
            entrance.Cluster2 = c2;
            entrance.Node1 = node1;
            entrance.Node2 = node2;

            // Add the entrance to the set of entrances
            entrances.Add(entrance);
        }

        return entrances;
    }

    private Cluster GetCluster(Entrance e, int level)
    {
        // Implementation to get the cluster associated with an entrance at a specific level
        // This method should find and return the cluster that contains the given entrance at the specified level
        // You can use the ClusterByLevel dictionary to retrieve the clusters at the given level
        // Iterate through the clusters and check if the entrance belongs to any of them
        // Return the found cluster, or null if no cluster is found
        // Check if the ClusterByLevel dictionary contains the specified level
        if (ClusterByLevel.ContainsKey(level))
        {
            // Retrieve the clusters at the specified level
            List<Cluster> clusters = ClusterByLevel[level];

            // Iterate through the clusters
            foreach (Cluster cluster in clusters)
            {
                // Check if the entrance belongs to the current cluster
                if (cluster.Entrances.Contains(e))
                {
                    // Return the found cluster
                    return cluster;
                }
            }
        }

        // Return null if no cluster is found
        return null;
    }

    private HPANode NewHPANode(Entrance e, Cluster c)
    {
        // Implementation to create a new HPANode object associated with an entrance and cluster
        // This method should create and return a new HPANode object with the given entrance and cluster
        // Set the properties of the HPANode (Id, Cluster, Position, Level)
        // The position of the node can be determined based on the entrance or cluster coordinates
        // Return the created HPANode object
        HPANode node = new HPANode();

        // Set the properties of the HPANode
        node.Id = Guid.NewGuid();
        node.Cluster = c;
        node.Level = c.Level;

        // Calculate the position of the node based on the entrance coordinates
        double entranceX = (e.Node1.Position.x + e.Node2.Position.x) / 2;
        double entranceY = (e.Node1.Position.y + e.Node2.Position.y) / 2;
        node.Position = new Vector2Int((int)entranceX, (int)entranceY);

        // Return the created HPANode object
        return node;
    }

    private void AddHPANode(HPANode n, int level)
    {
        // Implementation to add an HPANode to the NodesByLevel dictionary at the specified level
        // This method should add the given HPANode to the list of nodes at the specified level
        // If the level key doesn't exist in the NodesByLevel dictionary, create a new list for that level
        // Add the HPANode to the list of nodes at the given level
        // Check if the NodesByLevel dictionary contains the specified level
        if (!NodesByLevel.ContainsKey(level))
        {
            // If the level key doesn't exist, create a new list for that level
            NodesByLevel[level] = new List<HPANode>();
        }

        // Add the HPANode to the list of nodes at the given level
        NodesByLevel[level].Add(n);
    }
    private double SearchForDistance(HPANode n1, HPANode n2, Cluster c)
    {
        // Assuming a fixed size for the example. You might need to dynamically calculate or retrieve this.
        int mapWidth = 100; // Example width
        int mapHeight = 100; // Example height

        // Initialize the map array with 1s
        int[,] map = new int[mapWidth, mapHeight];
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                map[x, y] = 1; // 1 indicates non traversable terrain
            }
        }

        // Mark positions of HPANodes within the cluster as 0 (considering these positions are special or non-traversable)
        foreach (HPANode node in c.Nodes)
        {
            if (node.Position.x >= 0 && node.Position.x < mapWidth && node.Position.y >= 0 && node.Position.y < mapHeight)
            {
                map[(int)node.Position.x, (int)node.Position.y] = 0;
            }
        }

        // Implement A* or another pathfinding algorithm to find the shortest path
        // This is a placeholder; you'll need to implement or integrate an actual pathfinding algorithm
        Astar astar = new Astar();
        List<Vector2Int> path = astar.FindPath(n1.Position, n2.Position, map); // Adjust the FindPath method to work with your Astar implementation

        // Calculate the distance based on the path found
        // The distance calculation would depend on how your pathfinding algorithm represents paths
        double distance = path != null ? CalculateDistance(path) : double.PositiveInfinity; // Implement CalculateDistance based on your needs

        return distance;
    }

    private double CalculateDistance(List<Vector2Int> path)
    {
        // Implementation to calculate the distance of a path
        // This method should calculate the total distance of the given path
        // You can sum the distances between consecutive points in the path
        // Return the total distance of the path
        double distance = 0;

        for (int i = 0; i < path.Count - 1; i++)
        {
            distance += Vector2Int.Distance(path[i], path[i + 1]);
        }

        return distance;

    }

    private void AddHPAEdge(HPANode n1, HPANode n2, int level, double weight, HPAEdgeType type)
    {
        // Implementation to add an HPAEdge between two HPANodes at the specified level
        // This method should create an HPAEdge object and add it to the AdjacencyMatrices dictionary
        // If the level key doesn't exist in the AdjacencyMatrices dictionary, create a new 2D array for that level
        // Set the properties of the HPAEdge (Id, Node1, Node2, Weight, Level, Type)
        // Add the HPAEdge to the adjacency matrix at the given level

        // Create a new HPAEdge object
        HPAEdge edge = new HPAEdge
        {
            Id = Guid.NewGuid(),
            Node1 = n1,
            Node2 = n2,
            Weight = weight,
            Level = level,
            Type = type
        };

        // Check if the level key exists in the AdjacencyMatrices dictionary
        if (!AdjacencyMatrices.ContainsKey(level))
        {
            // Create a new 2D array for the level
            int size = NodesByLevel[level].Count;
            AdjacencyMatrices[level] = new HPAEdge[size, size];
        }

        // Add the HPAEdge to the adjacency matrix at the given level
        int index1 = NodesByLevel[level].IndexOf(n1);
        int index2 = NodesByLevel[level].IndexOf(n2);
        AdjacencyMatrices[level][index1, index2] = edge;
        AdjacencyMatrices[level][index2, index1] = edge;
    }


    private HashSet<Entrance> GetEntrances(Cluster c1, Cluster c2)
    {
        // Implementation to get the entrances between two clusters
        // This method should find and return the set of entrances that connect the two clusters
        // You can iterate through the entrances of both clusters and find the common ones
        // Return the set of found entrances
        HashSet<Entrance> commonEntrances = new HashSet<Entrance>();

        // Iterate through the entrances of both clusters and find the common ones
        foreach (Entrance e1 in c1.Entrances)
        {
            foreach (Entrance e2 in c2.Entrances)
            {
                if (e1.Id == e2.Id)
                {
                    commonEntrances.Add(e1);
                    break;
                }
            }
        }

        return commonEntrances;
    }

    private void SetLevel(HPANode n, int level)
    {
        // Implementation to set the level of an HPANode
        // This method should update the Level property of the given HPANode to the specified level
        n.Level = level;

    }


    private HPANode GetHPANode1(Entrance e)
    {
        // Implementation to get the first HPANode associated with an entrance
        // This method should return the Node1 property of the given Entrance object
        return e.Node1;

    }

    private HPANode GetHPANode2(Entrance e)
    {
        // Implementation to get the second HPANode associated with an entrance
        // This method should return the Node2 property of the given Entrance object
        return e.Node2;

    }

    private HPAEdge GetHPAEdge(Entrance e)
    {
        // Implementation to get the HPAEdge associated with an entrance
        // This method should find and return the HPAEdge that corresponds to the given entrance
        // You can iterate through the adjacency matrices at different levels to find the matching edge
        // Return the found HPAEdge, or null if no edge is found
        // Iterate through the adjacency matrices at different levels to find the matching edge
        foreach (var kvp in AdjacencyMatrices)
        {
            int level = kvp.Key;
            HPAEdge[,] adjacencyMatrix = kvp.Value;

            int rows = adjacencyMatrix.GetLength(0);
            int cols = adjacencyMatrix.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    HPAEdge edge = adjacencyMatrix[i, j];
                    if (edge != null &&
                   ((edge.Node1.Id == e.Node1.Id && edge.Node2.Id == e.Node2.Id) ||
                    (edge.Node1.Id == e.Node2.Id && edge.Node2.Id == e.Node1.Id)))
                    {
                        return edge;
                    }
                }
            }
        }

        return null;
    }
}


public class Cluster
{
    public Guid Id { get; set; }
    public int Level { get; set; }
    public List<HPANode> Nodes { get; set; }
    public List<Entrance> Entrances { get; set; }
}
public class Entrance
{
    public Guid Id { get; set; }
    public Cluster Cluster1 { get; set; }
    public Cluster Cluster2 { get; set; }
    public HPANode Node1 { get; set; }
    public HPANode Node2 { get; set; }
}
public class HPANode
{
    public Guid Id { get; set; }
    public Cluster Cluster { get; set; }
    public Vector2Int Position { get; set; }
    public int Level { get; set; }
}
public class HPAEdge
{
    public Guid Id { get; set; }
    public HPANode Node1 { get; set; }
    public HPANode Node2 { get; set; }
    public double Weight { get; set; }
    public int Level { get; set; }
    public HPAEdgeType Type { get; set; }
}
public enum HPAEdgeType { INTER, INTRA }


