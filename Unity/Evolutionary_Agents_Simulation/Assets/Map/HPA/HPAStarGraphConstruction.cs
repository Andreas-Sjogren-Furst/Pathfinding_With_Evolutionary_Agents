using System;
using System.Collections.Generic;
using System.Linq;
using Codice.Client.BaseCommands;
using Codice.Client.Common.GameUI;
using UnityEngine;


// TODO:  Global positionss
public class HPAStarGraphConstruction
{
    public int[,] GlobalTileMap { get; set; } // Map
    private Dictionary<int, HashSet<Entrance>> EntrancesByLevel { get; set; } // E 
    public Dictionary<int, HashSet<Cluster>> ClusterByLevel { get; set; } // C
    public Dictionary<int, Dictionary<Vector2Int, HPANode>> NodesByLevel { get; set; } // N

    public Dictionary<int, int> nodeCountByLevel { get; set; }

    // HPAGraph:
    // Dictionary<Vector2Int, HPANode> Nodes
    //  Usage: HPAGraph.nodes 





    public HPAStarGraphConstruction(int[,] globalTileMap)
    {
        GlobalTileMap = globalTileMap;
        EntrancesByLevel = new Dictionary<int, HashSet<Entrance>>();
        ClusterByLevel = new Dictionary<int, HashSet<Cluster>>();
        NodesByLevel = new Dictionary<int, Dictionary<Vector2Int, HPANode>>();
        nodeCountByLevel = new Dictionary<int, int>();
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


    public void AbstractMaze() // A1. 
    {
        ClusterByLevel.Add(1, BuildClusters(1));
        foreach (Cluster c1 in ClusterByLevel[1])
        {
            foreach (Cluster c2 in ClusterByLevel[1])
            {
                if (Adjacent(c1, c2))
                {
                    Debug.Log("Clusters: " + c1.Id.ToString() + " " + c2.Id.ToString());
                    HashSet<Entrance> entrances = BuildEntrances(c1, c2);
                    c1.Entrances.UnionWith(entrances);
                    c2.Entrances.UnionWith(entrances);

                    if (!this.EntrancesByLevel.ContainsKey(1))
                    {
                        this.EntrancesByLevel.Add(1, new HashSet<Entrance>());
                    }
                    this.EntrancesByLevel[1].UnionWith(entrances);
                }
            }
        }
    }

    public void BuildGraph() // B1. 
    {

        Debug.Log("Entrances: " + EntrancesByLevel[1].Count);
        Debug.Log("Nodes: " + NodesByLevel[1].Count);







        foreach (Entrance E in EntrancesByLevel[1])
        {
            Cluster c1 = E.Cluster1;
            Cluster c2 = E.Cluster2;

            HPANode n1 = NewHPANode(E, c1);
            HPANode n2 = NewHPANode(E, c2);
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
                        double d = SearchForDistance(n1, n2, c);
                        if (d < double.PositiveInfinity)
                        {
                            AddHPAEdge(n1, n2, weight: d, level: 1, HPAEdgeType.INTRA);
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




    // ... (previous code remains the same)

    // Helper methods
    public HashSet<Cluster> BuildClusters(int level) // A2. 
    {
        HashSet<Cluster> clusters = new HashSet<Cluster>();
        int clusterSize = 5 * level;

        int gridHeight = GlobalTileMap.GetLength(0) / clusterSize;
        int gridWidth = GlobalTileMap.GetLength(1) / clusterSize;

        if (!nodeCountByLevel.ContainsKey(level))
        {
            nodeCountByLevel.Add(level, 0);
        }




        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                Cluster cluster = new Cluster
                (
                    bottomLeftPos: new Vector2Int(i * clusterSize, j * clusterSize),
                    topRightPos: new Vector2Int((i + 1) * clusterSize - 1, (j + 1) * clusterSize - 1),
                    level: level,
                    hPANodes: new HashSet<HPANode>(),
                    entrances: new HashSet<Entrance>()
                );

                int startX = i * clusterSize;
                int startY = j * clusterSize;
                int endX = startX + clusterSize - 1;
                int endY = startY + clusterSize - 1;

                if (GlobalTileMap[i, j] != 1)
                { // If the tile is walkable
                  // Create a new HPANode for this tile
                    int nCount = nodeCountByLevel[level];
                    HPANode newNode = new HPANode(id: nCount, cluster: cluster, position: new Vector2Int(i, j), level: level);
                    nodeCountByLevel[level] = nCount + 1;

                    // Add the new node to the cluster's nodes
                    //      cluster.Nodes.Add(newNode);

                    // Ensure NodesByLevel is properly initialized for the current level
                    if (!NodesByLevel.ContainsKey(level))
                    {
                        NodesByLevel[level] = new Dictionary<Vector2Int, HPANode>();
                    }

                    // Add the new node to NodesByLevel
                    NodesByLevel[level].Add(newNode.Position, newNode);
                }
            }
        }

        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                Cluster cluster = new Cluster
                (
                    bottomLeftPos: new Vector2Int(i * clusterSize, j * clusterSize),
                    topRightPos: new Vector2Int((i + 1) * clusterSize - 1, (j + 1) * clusterSize - 1),
                    level: level,
                    hPANodes: new HashSet<HPANode>(),
                    entrances: new HashSet<Entrance>()
                );

                int startX = i * clusterSize;
                int startY = j * clusterSize;
                int endX = startX + clusterSize - 1;
                int endY = startY + clusterSize - 1;


                if (NodesByLevel.ContainsKey(level))
                {
                    if (NodesByLevel[level].Count == 0)
                    {
                        Debug.Log("THERE ARE NOT NODES !!!"); //TODO: FIX NO NODES
                    }
                    {

                    }


                    if (NodesByLevel.TryGetValue(level, out var levelNodes))
                    {
                        foreach (KeyValuePair<Vector2Int, HPANode> kvp in levelNodes)
                        {
                            Vector2Int position = kvp.Key;
                            HPANode node = kvp.Value;

                            if (position.x >= startX && position.x <= endX &&
                                position.y >= startY && position.y <= endY)
                            {
                                cluster.Nodes.Add(node);
                                node.Cluster = cluster;
                            }
                        }
                    }

                }


                // Create edges for nodes within the cluster
                cluster = CreateEdgesForCluster(cluster, startX, startY, endX, endY, clusterSize);
                clusters.Add(cluster);
            }
        }

        return clusters;
    }

    private HashSet<Entrance> BuildEntrances(Cluster c1, Cluster c2) // A3. 
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
        Debug.Log("Shared nodes: " + sharedNodes.Count);


        // Create entrances for each pair of shared nodes
        for (int i = 0; i < sharedNodes.Count - 1; i++)
        {
            HPANode node1 = sharedNodes[i];
            HPANode node2 = sharedNodes[i + 1];

            // Create a new entrance
            Entrance entrance = new Entrance(cluster1: c1, cluster2: c2, node1: node1, node2: node2);


            // Add the entrance to the set of entrances
            entrances.Add(entrance);
        }

        return entrances;
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
                            AddHPAEdge(n1, n2, weight: d, level: l, HPAEdgeType.INTRA);
                        }
                    }
                }
            }
        }
    }

    private Cluster CreateEdgesForCluster(Cluster cluster, int startX, int startY, int endX, int endY, int clusterSize)
    {

        foreach (HPANode node in cluster.Nodes)
        {
            int x = node.Position.x;
            int y = node.Position.y;

            int[] dx = { 0, 1, 0, -1 };
            int[] dy = { 1, 0, -1, 0 };

            for (int direction = 0; direction < 4; direction++)
            {
                int newX = x + dx[direction];
                int newY = y + dy[direction];

                if (newX >= startX && newX <= endX && newY >= startY && newY <= endY && GlobalTileMap[newX, newY] != 1)
                {
                    HPANode adjacentNode = cluster.Nodes.FirstOrDefault(n => n.Position.x == newX && n.Position.y == newY);
                    if (adjacentNode != null)
                    {
                        HPAEdge edge = new HPAEdge
                        (
                            node1: node,
                            node2: adjacentNode,
                            weight: 1,
                            level: cluster.Level,
                            type: HPAEdgeType.INTRA
                        );

                        cluster.Nodes.FirstOrDefault(n => n.Position == node.Position).Edges.Add(edge); // TODO: optimize this.
                    }
                }
            }
        }
        return cluster;
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


        if (c1.Nodes.Intersect(c2.Nodes).Count() > 0) return true;

        return false;


        // // Get the boundaries of the clusters
        // int c1StartX = (int)c1.Nodes.Min(node => node.Position.x);
        // int c1StartY = (int)c1.Nodes.Min(node => node.Position.y);
        // int c1EndX = (int)c1.Nodes.Max(node => node.Position.x);
        // int c1EndY = (int)c1.Nodes.Max(node => node.Position.y);

        // int c2StartX = (int)c2.Nodes.Min(node => node.Position.x);
        // int c2StartY = (int)c2.Nodes.Min(node => node.Position.y);
        // int c2EndX = (int)c2.Nodes.Max(node => node.Position.x);
        // int c2EndY = (int)c2.Nodes.Max(node => node.Position.y);

        // // Check if the clusters share a common boundary
        // if (c1StartX == c2EndX + 1 || c1EndX + 1 == c2StartX ||
        //     c1StartY == c2EndY + 1 || c1EndY + 1 == c2StartY)
        // {
        //     return true;
        // }

        // return false;


    }







    private HPANode NewHPANode(Entrance e, Cluster c)
    {
        // Implementation to create a new HPANode object associated with an entrance and cluster
        // This method should create and return a new HPANode object with the given entrance and cluster
        // Set the properties of the HPANode (Id, Cluster, Position, Level)
        // The position of the node can be determined based on the entrance or cluster coordinates
        // Return the created HPANode object

        // Calculate the position of the node based on the entrance coordinates
        double entranceX = (e.Node1.Position.x + e.Node2.Position.x) / 2;
        double entranceY = (e.Node1.Position.y + e.Node2.Position.y) / 2;
        Vector2Int nodePos = new Vector2Int((int)entranceX, (int)entranceY);

        int nCount = nodeCountByLevel[c.Level];
        HPANode node = new HPANode(id: nCount, cluster: c, position: nodePos, level: c.Level);
        nodeCountByLevel[c.Level] = nCount + 1;
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
            NodesByLevel[level] = new Dictionary<Vector2Int, HPANode>();
        }

        // Check if a node with the same position already exists
        if (!NodesByLevel[level].ContainsKey(n.Position))
        {
            NodesByLevel[level].Add(n.Position, n);
        }
        else
        {
            NodesByLevel[level][n.Position] = n;
            // Handle the duplicate position - replace, ignore, or merge
            // For example, to replace the existing node:
            // NodesByLevel[level][n.Position] = n;
        }
    }
    private double SearchForDistance(HPANode n1, HPANode n2, Cluster c)
    {
        // Implementation to search for the distance between two HPANodes within a cluster
        // This method should calculate and return the distance between the two HPANodes within the cluster
        // You can use any pathfinding algorithm like A* to find the distance



        // Implement A* or another pathfinding algorithm to find the shortest path
        // This is a placeholder; you'll need to implement or integrate an actual pathfinding algorithm
        List<HPANode> path = Astar.FindPath(n1, n2); // Adjust the FindPath method to work with your Astar implementation

        // Calculate the distance based on the path found
        // The distance calculation would depend on how your pathfinding algorithm represents paths
        double distance = path != null ? CalculateDistance(path) : double.PositiveInfinity; // Implement CalculateDistance based on your needs

        return distance;
    }

    private double CalculateDistance(List<HPANode> path)
    {
        // Implementation to calculate the distance of a path
        // This method should calculate the total distance of the given path
        // You can sum the distances between consecutive points in the path
        // Return the total distance of the path
        double distance = 0;

        for (int i = 0; i < path.Count - 1; i++)
        {
            distance += Vector2Int.Distance(path[i].Position, path[i + 1].Position);
        }

        return distance;

    }

    public void AddHPAEdge(HPANode node1, HPANode node2, double weight, int level, HPAEdgeType type)
    {
        // Ensure the level exists
        if (!NodesByLevel.ContainsKey(level))
        {
            throw new Exception($"Level {level} does not exist.");
        }



        HPAEdge edge1 = new HPAEdge(node1, node2, weight, level, type);
        HPAEdge edge2 = new HPAEdge(node2, node1, weight, level, type);

        node1.Edges.Add(edge1);
        node2.Edges.Add(edge2);

        NodesByLevel[level][node1.Position] = node1;
        NodesByLevel[level][node2.Position] = node2;



    }


    // Example of how you might convert a Vector2Int position to a matrix index
    private int ConvertPositionToIndex(Vector2Int position)
    {
        // This conversion depends on how you decide to map positions to indices.
        // For example, you could linearize the position by using the position.x and position.y
        // to calculate a unique index for a flattened matrix or directly map them
        // to 2D matrix coordinates if your structure supports it.

        int maxWidth = GlobalTileMap.GetLength(0);




        return position.x * maxWidth + position.y;
    }



    // public void AddHPAEdge(HPANode n1, HPANode n2, int level, double weight, HPAEdgeType type)
    // {
    //     // Implementation to add an HPAEdge between two HPANodes at the specified level
    //     // This method should create an HPAEdge object and add it to the AdjacencyMatrices dictionary
    //     // If the level key doesn't exist in the AdjacencyMatrices dictionary, create a new 2D array for that level
    //     // Set the properties of the HPAEdge (Id, Node1, Node2, Weight, Level, Type)
    //     // Add the HPAEdge to the adjacency matrix at the given level

    //     // Create a new HPAEdge object
    //     HPAEdge edge = new HPAEdge(
    //         node1: n1,
    //         node2: n2,
    //         weight: weight,
    //         level: level,
    //         type: type
    //     );

    //     // Check if the level key exists in the AdjacencyMatrices dictionary
    //     if (!AdjacencyMatrices.ContainsKey(level))
    //     {
    //         // // Create a new 2D array for the level
    //         // int size = NodesByLevel[level].Count;
    //         // Debug.Log("nodes by level Size: " + size);
    //         // AdjacencyMatrices[level] = new HPAEdge[size, size];
    //         throw new Exception("No level found in AdjacencyMatrices");
    //     }

    //     // Add the HPAEdge to the adjacency matrix at the given level







    //     AdjacencyMatrices[level][index1, index2] = edge;
    //     AdjacencyMatrices[level][index2, index1] = edge;
    // }


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


        throw new NotImplementedException();
    }

    private void connectToBorder(HPANode n, Cluster c)
    {
        int level = c.Level;

        foreach (HPANode node in c.Entrances.Select(e => e.Node1).Where(node => node != n))
        {

            double d = SearchForDistance(n, node, c);
            if (d < double.PositiveInfinity)
            {
                AddHPAEdge(n, node, weight: d, level: level, HPAEdgeType.INTRA); //TODO: unsure if Intra is corect.? 
            }

        }
    }

    public void insertNode(HPANode s, int maxLevel) // start node and finish node. 
    {
        for (int l = 1; l <= maxLevel; l++)
        {
            Cluster c = DetermineCluster(s, l);
            connectToBorder(s, c);
        }

        s.Level = maxLevel;


        if (!NodesByLevel[maxLevel].ContainsKey(s.Position))
        {
            NodesByLevel[maxLevel].Add(s.Position, s);
        }
        else
        {
            NodesByLevel[maxLevel][s.Position] = s;
        }
        {

        }

    }



    private Cluster DetermineCluster(HPANode n, int level)
    {
        // Assuming clusters are well-defined and partition the space such that every position falls into exactly one cluster at each level.
        foreach (Cluster cluster in ClusterByLevel[level])
        {
            if (cluster.Contains(n.Position)) // Implement this Contains method based on your cluster definition.
            {
                return cluster;
            }
        }
        return null; // Or handle differently if your application expects every node to fit within a cluster.
    }


    public List<HPANode> hierarchicalSearch(HPANode s, HPANode g, int level)
    {
        insertNode(s, level);
        insertNode(g, level);
        List<HPANode> absPath = searchForPath(s.Position, g.Position, level);
        List<HPANode> llPath = refinePath(absPath, level);
        //smPath = smoothPath(llPath);
        return llPath;
    }

    public List<HPANode> searchForPath(Vector2Int s, Vector2Int g, int level)
    {
        // Implementation of A* search algorithm to find a path in the abstract graph
        // You can use any standard A* implementation or library
        // The search should be performed on the abstract graph at the specified level
        // Return the list of HPANodes representing the abstract path from s to g

        // Example implementation using a placeholder A* algorithm

        if (!NodesByLevel.ContainsKey(level))
        {
            throw new Exception("Level not found in NodesByLevel");

        }

        if (!NodesByLevel[level].ContainsKey(s))
        {

            throw new Exception("Start node not found in NodesByLevel");



        }


        HPANode start = NodesByLevel[level][s];
        HPANode goal = NodesByLevel[level][g];




        List<HPANode> abstractPath = Astar.FindPath(start, goal);




        return abstractPath;





    }

    public List<HPANode> refinePath(List<HPANode> abstractPath, int level)
    {
        List<HPANode> refinedPath = new List<HPANode>();

        // Iterate through each pair of consecutive HPANodes in the abstract path
        for (int i = 0; i < abstractPath.Count - 1; i++)
        {

            HPANode startNode = abstractPath[i];
            HPANode endNode = abstractPath[i + 1];

            // Check if the start and end nodes are in the same cluster
            if (startNode.Cluster == endNode.Cluster)
            {
                // If in the same cluster, find the local path within the cluster
                List<HPANode> localPath = findLocalPath(startNode, endNode, startNode.Cluster);
                refinedPath.AddRange(localPath);
            }
            else
            {
                // If in different clusters, find the path through the entrance
                Entrance entrance = findEntrance(startNode.Cluster, endNode.Cluster);
                List<HPANode> entrancePath = findLocalPath(startNode, entrance.Node1, startNode.Cluster);
                refinedPath.AddRange(entrancePath);
                refinedPath.Add(entrance.Node2);
            }
        }

        // Add the position of the last HPANode in the abstract path
        refinedPath.Add(abstractPath[abstractPath.Count - 1]);

        return refinedPath;
    }

    private List<HPANode> findLocalPath(HPANode startNode, HPANode endNode, Cluster cluster)
    {
        // Implementation to find the local path between two HPANodes within a cluster
        // You can use any pathfinding algorithm like A* to find the path
        // Return the list of Vector2Int positions representing the local path

        // Example implementation using a placeholder A* algorithm
        List<HPANode> localPath = Astar.FindPath(startNode, endNode);

        return localPath;
    }

    private Entrance findEntrance(Cluster startCluster, Cluster endCluster)
    {
        // Implementation to find the entrance connecting two clusters
        // You can iterate through the entrances of the clusters and find the matching one
        // Return the found Entrance object

        foreach (Entrance entrance in startCluster.Entrances)
        {
            if ((entrance.Cluster1 == startCluster && entrance.Cluster2 == endCluster) ||
                (entrance.Cluster1 == endCluster && entrance.Cluster2 == startCluster))
            {
                return entrance;
            }
        }

        return null;
    }





    // public List<Vector2Int> smoothPath(List<Vector2Int> path)
    // {
    //     List<Vector2Int> smoothedPath = new List<Vector2Int>();

    //     // Add the first position to the smoothed path
    //     smoothedPath.Add(path[0]);

    //     // Iterate through the path positions
    //     for (int i = 1; i < path.Count - 1; i++)
    //     {
    //         Vector2Int currentPos = path[i];
    //         Vector2Int nextPos = path[i + 1];

    //         // Check if there is a straight line between the current position and the next position
    //         if (!isWalkable(currentPos, nextPos))
    //         {
    //             // If not walkable, add the current position to the smoothed path
    //             smoothedPath.Add(currentPos);
    //         }
    //     }

    //     // Add the last position to the smoothed path
    //     smoothedPath.Add(path[path.Count - 1]);

    //     return smoothedPath;
    // }

    // private bool isWalkable(Vector2Int startPos, Vector2Int endPos)
    // {
    //     // Implementation to check if there is a walkable straight line between two positions
    //     // You can use a line drawing algorithm like Bresenham's line algorithm
    //     // Check if all the positions along the line are walkable (e.g., not blocked by obstacles)
    //     // Return true if the line is walkable, false otherwise

    //     // Example implementation using a placeholder walkability check
    //     // Assuming you have a method to check if a position is walkable
    //     foreach (Vector2Int pos in getPositionsAlongLine(startPos, endPos))
    //     {
    //         if (!isPositionWalkable(pos))
    //         {
    //             return false;
    //         }
    //     }

    //     return true;
    // }

    // private bool isPositionWalkable(Vector2Int position)
    // {
    //     // Implementation to check if a position is walkable
    //     // You can check against your game's walkability criteria (e.g., not blocked by obstacles)
    //     // Return true if the position is walkable, false otherwise

    //     // Placeholder implementation
    //     // Assuming you have a method to check if a position is walkable in your game
    //     return IsWalkable(position);
    // }

    // private bool IsWalkable(Vector2Int position)
    // {
    //     return GlobalTileMap[position.x, position.y] == 0;


    // }

    // private List<Vector2Int> getPositionsAlongLine(Vector2Int startPos, Vector2Int endPos)
    // {
    //     // Implementation to get the positions along a straight line between two positions
    //     // You can use a line drawing algorithm like Bresenham's line algorithm
    //     // Return the list of Vector2Int positions along the line

    //     // Placeholder implementation
    //     // Assuming you have a method to get the positions along a line in your game
    //     return GetPositionsAlongLine(startPos, endPos);
    // }
}






