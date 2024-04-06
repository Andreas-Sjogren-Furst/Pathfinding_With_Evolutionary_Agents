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

            // HPANode n1 = NewHPANode(E, c1);
            // HPANode n2 = NewHPANode(E, c2);
            // AddHPANode(n1, 1);
            // AddHPANode(n2, 1);

            AddHPAEdge(E.Node1, E.Node2, 1, 1, HPAEdgeType.INTER); // optimize to only put 1 entrance in the middle. 

        }

        // add INTER edges between Entrances in the same cluster. 

        Debug.Log("clusters: " + ClusterByLevel[1].Count);


        foreach (Cluster c in ClusterByLevel[1])
        {
            Debug.Log($"Cluster {c.Id} has {c.Entrances.Count} entrances");

            foreach (Entrance e1 in c.Entrances)
            {
                foreach (Entrance e2 in c.Entrances)
                {
                    if (e1.Node1.Position != e2.Node1.Position)
                    {
                        double d = SearchForDistance(e1.Node1, e2.Node1, c);
                        if (d < double.PositiveInfinity)
                        {
                            AddHPAEdge(e1.Node1, e2.Node1, weight: 1, level: 1, HPAEdgeType.INTER);
                        }
                    }
                }
            }
        }



        // Not needed: too much overhead. 

        // foreach (Cluster c in ClusterByLevel[1])
        // {
        //     foreach (HPANode n1 in c.Nodes)
        //     {
        //         foreach (HPANode n2 in c.Nodes)
        //         {
        //             if (n1.Id != n2.Id)
        //             {
        //                 double d = SearchForDistance(n1, n2, c);
        //                 if (d < double.PositiveInfinity)
        //                 {
        //                     // AddHPAEdge(n1, n2, weight: d, level: 1, HPAEdgeType.INTRA);
        //                 }
        //             }
        //         }
        //     }
        // }
    }



    private void SetLevel(HPAEdge hPAEdge, int l)
    {
        throw new NotImplementedException();
    }




    // ... (previous code remains the same)

    // Helper methods
    public HashSet<Cluster> BuildClusters(int level)
    {
        HashSet<Cluster> clusters = new HashSet<Cluster>();
        int clusterSize = 10 * level; // This defines how large each cluster is relative to the level.

        // Calculating the number of clusters needed to cover the entire map.
        int gridHeight = (int)Math.Ceiling((double)GlobalTileMap.GetLength(0) / clusterSize);
        int gridWidth = (int)Math.Ceiling((double)GlobalTileMap.GetLength(1) / clusterSize);

        // Ensure the node count for this level is initialized.
        if (!nodeCountByLevel.ContainsKey(level))
        {
            nodeCountByLevel.Add(level, 0);
        }

        // Loop through each grid position to create clusters.
        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                int startX = i * clusterSize;
                int startY = j * clusterSize;
                int endX = Math.Min(startX + clusterSize - 1, GlobalTileMap.GetLength(0) - 1);
                int endY = Math.Min(startY + clusterSize - 1, GlobalTileMap.GetLength(1) - 1);

                Cluster cluster = new Cluster(
                    bottomLeftPos: new Vector2Int(startX, startY),
                    topRightPos: new Vector2Int(endX, endY),
                    level: level,
                    hPANodes: new HashSet<HPANode>(),
                    entrances: new HashSet<Entrance>()
                );

                // Populate nodes within the cluster bounds.
                for (int x = startX; x <= endX; x++)
                {
                    for (int y = startY; y <= endY; y++)
                    {
                        if (GlobalTileMap[x, y] != 1) // Assuming 1 means non-walkable.
                        {
                            int nCount = nodeCountByLevel[level]++;
                            HPANode newNode = new HPANode(id: nCount, cluster: cluster, position: new Vector2Int(x, y), level: level);
                            cluster.Nodes.Add(newNode);

                            if (!NodesByLevel.ContainsKey(level))
                            {
                                NodesByLevel[level] = new Dictionary<Vector2Int, HPANode>();
                            }
                            NodesByLevel[level].Add(new Vector2Int(x, y), newNode);
                        }
                    }
                }

                // Optionally, create edges for nodes within the cluster if necessary here.
                cluster = CreateEdgesForCluster(cluster, startX, startY, endX, endY, clusterSize);
                clusters.Add(cluster);
            }
        }

        return clusters;
    }

    // private HashSet<Entrance> BuildEntrances(Cluster c1, Cluster c2)
    // {
    //     HashSet<Entrance> entrances = new HashSet<Entrance>();

    //     // Ensure the clusters are adjacent; Adjacent method remains unchanged
    //     if (!Adjacent(c1, c2)) return entrances;

    //     // Calculate shared borders for all possible adjacency cases
    //     // Horizontal Adjacency (East-West or West-East)
    //     if (c1.bottomLeftPos.y == c2.bottomLeftPos.y && c1.topRightPos.y == c2.topRightPos.y)
    //     {
    //         int sharedStartY = c1.bottomLeftPos.y;
    //         int sharedEndY = c1.topRightPos.y;
    //         // Determine which cluster is on the left
    //         int xForC1 = c1.topRightPos.x == c2.bottomLeftPos.x - 1 ? c1.topRightPos.x : c1.bottomLeftPos.x - 1;
    //         int xForC2 = xForC1 == c1.topRightPos.x ? c2.bottomLeftPos.x : c2.topRightPos.x;

    //         for (int y = sharedStartY; y <= sharedEndY; y++)
    //         {
    //             if (GlobalTileMap[xForC1, y] != 1 && GlobalTileMap[xForC2, y] != 1)
    //             {
    //                 // Find or create nodes and entrances
    //                 var node1 = FindOrCreateNode(xForC1, y, c1);
    //                 var node2 = FindOrCreateNode(xForC2, y, c2);
    //                 entrances.Add(new Entrance(c1, c2, node1, node2)); //  undirected graph
    //                 entrances.Add(new Entrance(c2, c1, node2, node1)); // undirected graph
    //             }
    //         }
    //     }
    //     // Vertical Adjacency (North-South or South-North)
    //     else if (c1.bottomLeftPos.x == c2.bottomLeftPos.x && c1.topRightPos.x == c2.topRightPos.x)
    //     {
    //         int sharedStartX = c1.bottomLeftPos.x;
    //         int sharedEndX = c1.topRightPos.x;
    //         // Determine which cluster is on the bottom
    //         int yForC1 = c1.topRightPos.y == c2.bottomLeftPos.y - 1 ? c1.topRightPos.y : c1.bottomLeftPos.y - 1;
    //         int yForC2 = yForC1 == c1.topRightPos.y ? c2.bottomLeftPos.y : c2.topRightPos.y;

    //         for (int x = sharedStartX; x <= sharedEndX; x++)
    //         {
    //             if (GlobalTileMap[x, yForC1] != 1 && GlobalTileMap[x, yForC2] != 1)
    //             {

    //                 // check if it is on a line segment for previous in the set. 




    //                 // Find or create nodes and entrances
    //                 var node1 = FindOrCreateNode(x, yForC1, c1);
    //                 var node2 = FindOrCreateNode(x, yForC2, c2);
    //                 entrances.Add(new Entrance(c1, c2, node1, node2)); // undirected graph
    //                 entrances.Add(new Entrance(c2, c1, node2, node1)); // undirected graph
    //             }
    //         }
    //     }

    //     Debug.Log("Entrances added betwen: " + c1.bottomLeftPos.ToString() + " and " + c2.bottomLeftPos.ToString() + " : " + entrances.Count.ToString());

    //     // clean up the entrances to only have 1 entrance between 2 clusters.










    //     return entrances;
    // }


    private HashSet<Entrance> BuildEntrances(Cluster c1, Cluster c2, int maxGroupSize = 5) // optimized to build less entrances. 
    {
        HashSet<Entrance> entrances = new HashSet<Entrance>();

        // Ensure the clusters are adjacent
        if (!Adjacent(c1, c2)) return entrances;

        List<Tuple<int, int>> potentialEntranceCoordinates = new List<Tuple<int, int>>();

        // Determine shared border and orientation (horizontal or vertical adjacency)
        bool isHorizontal = c1.bottomLeftPos.y == c2.bottomLeftPos.y && c1.topRightPos.y == c2.topRightPos.y;
        int sharedStart = isHorizontal ? c1.bottomLeftPos.y : c1.bottomLeftPos.x;
        int sharedEnd = isHorizontal ? c1.topRightPos.y : c1.topRightPos.x;
        int lengthAlongBorder = sharedEnd - sharedStart + 1;

        // Find all potential entrances along the shared border
        for (int i = 0; i < lengthAlongBorder; i++)
        {
            int posAlongBorder = sharedStart + i;
            int posC1 = isHorizontal ? c1.topRightPos.x : c1.topRightPos.y;
            int posC2 = isHorizontal ? c2.bottomLeftPos.x : c2.bottomLeftPos.y;

            if (GlobalTileMap[isHorizontal ? posC1 : posAlongBorder, isHorizontal ? posAlongBorder : posC1] != 1 &&
                GlobalTileMap[isHorizontal ? posC2 : posAlongBorder, isHorizontal ? posAlongBorder : posC2] != 1)
            {
                potentialEntranceCoordinates.Add(Tuple.Create(posAlongBorder, i));
            }
        }

        // Group adjacent coordinates into entrances, max size defined by maxGroupSize
        for (int i = 0; i < potentialEntranceCoordinates.Count; i += maxGroupSize)
        {
            int groupSize = Math.Min(maxGroupSize, potentialEntranceCoordinates.Count - i);
            int middleIndex = i + groupSize / 2;
            Tuple<int, int> midPoint = potentialEntranceCoordinates[middleIndex];

            int coord = midPoint.Item1;
            int xForC1 = isHorizontal ? c1.topRightPos.x : coord;
            int yForC1 = isHorizontal ? coord : c1.topRightPos.y;
            int xForC2 = isHorizontal ? c2.bottomLeftPos.x : coord;
            int yForC2 = isHorizontal ? coord : c2.bottomLeftPos.y;

            // Find or create nodes at the central point of each group
            var node1 = FindOrCreateNode(xForC1, yForC1, c1);
            var node2 = FindOrCreateNode(xForC2, yForC2, c2);
            entrances.Add(new Entrance(c1, c2, node1, node2));
            entrances.Add(new Entrance(c2, c1, node2, node1));
        }

        Debug.Log("Entrances added between: " + c1.bottomLeftPos.ToString() + " and " + c2.bottomLeftPos.ToString() + " : " + entrances.Count.ToString());

        return entrances;
    }


    public CompassDirection getClusterDirection(Cluster c1, Cluster c2)
    {
        // Calculate the centroids of both clusters
        Vector2 c1Center = new Vector2((c1.bottomLeftPos.x + c1.topRightPos.x) / 2.0f, (c1.bottomLeftPos.y + c1.topRightPos.y) / 2.0f);
        Vector2 c2Center = new Vector2((c2.bottomLeftPos.x + c2.topRightPos.x) / 2.0f, (c2.bottomLeftPos.y + c2.topRightPos.y) / 2.0f);

        // Determine the direction based on the centroids
        if (c2Center.x > c1Center.x) return CompassDirection.East;
        if (c2Center.x < c1Center.x) return CompassDirection.West;
        if (c2Center.y > c1Center.y) return CompassDirection.North;
        if (c2Center.y < c1Center.y) return CompassDirection.South;

        return CompassDirection.None;
    }

    private HPANode FindOrCreateNode(int x, int y, Cluster cluster)
    {
        Vector2Int position = new Vector2Int(x, y);
        int level = cluster.Level;

        // Check if a node at the given position and level already exists
        if (NodesByLevel.ContainsKey(level) && NodesByLevel[level].ContainsKey(position))
        {
            // Node exists, return it
            return NodesByLevel[level][position];
        }
        else
        {
            // Node does not exist, create a new one
            int nCount = nodeCountByLevel[level];
            HPANode newNode = new HPANode(
                id: nCount, // You need to implement this method
                cluster: cluster,
                position: position,
                level: level);
            nodeCountByLevel[level] = nCount + 1;

            // Assuming the node's constructor sets its initial properties

            // Add the new node to the hierarchical structure
            AddHPANode(newNode, level);

            return newNode;
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
                            AddHPAEdge(n1, n2, weight: d, level: l, HPAEdgeType.INTRA);
                        }
                    }
                }
            }
        }
    }

    private Cluster CreateEdgesForCluster(Cluster cluster, int startX, int startY, int endX, int endY, int clusterSize)
    {
        // Dictionary for quick node lookup based on positions within the cluster
        Dictionary<Vector2Int, HPANode> nodeLookup = new Dictionary<Vector2Int, HPANode>();
        foreach (var node in cluster.Nodes)
        {
            nodeLookup[new Vector2Int(node.Position.x, node.Position.y)] = node;
        }

        int[] dx = { 1, -1, 0, 0 }; // East, West, North, South
        int[] dy = { 0, 0, 1, -1 };

        foreach (HPANode node in cluster.Nodes)
        {
            int x = node.Position.x;
            int y = node.Position.y;

            for (int direction = 0; direction < 4; direction++)
            {
                int newX = x + dx[direction];
                int newY = y + dy[direction];
                Vector2Int newPosition = new Vector2Int(newX, newY);

                // Ensure the new position is within the cluster bounds and the map tile at this position is walkable
                if (newX >= startX && newX <= endX && newY >= startY && newY <= endY && GlobalTileMap[newX, newY] != 1)
                {
                    if (nodeLookup.TryGetValue(newPosition, out HPANode adjacentNode))
                    {
                        // Create an edge only if both current and adjacent positions are walkable
                        if (GlobalTileMap[x, y] != 1 && GlobalTileMap[newX, newY] != 1)
                        {
                            HPAEdge edge = new HPAEdge
                            (
                                node1: node,
                                node2: adjacentNode,
                                weight: 1,  // Assuming uniform cost
                                level: cluster.Level,
                                type: HPAEdgeType.INTRA
                            );

                            node.Edges.Add(edge); // Add edge directly to the node's edges list
                        }
                    }
                }
            }
        }
        return cluster;
    }



    public bool Adjacent(Cluster c1, Cluster c2)
    {
        return c1.bottomLeftPos.x == c2.topRightPos.x + 1 || c1.topRightPos.x == c2.bottomLeftPos.x - 1 ||
               c1.bottomLeftPos.y == c2.topRightPos.y + 1 || c1.topRightPos.y == c2.bottomLeftPos.y - 1;





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
            NodesByLevel[level][n.Position].Merge(n);

        }
    }
    private double SearchForDistance(HPANode n1, HPANode n2, Cluster c) // unsure if this should use A*, it is very slow if it do so...
    {
        // Implementation to search for the distance between two HPANodes within a cluster
        // This method should calculate and return the distance between the two HPANodes within the cluster
        // You can use any pathfinding algorithm like A* to find the distance



        // Implement A* or another pathfinding algorithm to find the shortest path
        // This is a placeholder; you'll need to implement or integrate an actual pathfinding algorithm
        //  List<HPANode> path = Astar.FindPath(n1, n2, HPAEdgeType.INTRA); // Adjust the FindPath method to work with your Astar implementation

        // Calculate the distance based on the path found
        // The distance calculation would depend on how your pathfinding algorithm represents paths
        //  double distance = path != null ? CalculateDistance(path) : double.PositiveInfinity; // Implement CalculateDistance based on your needs

        if (n1.Cluster != c || n2.Cluster != c)
        {
            return double.PositiveInfinity;

        }

        return Vector2Int.Distance(n1.Position, n2.Position);
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

        // If the cluster might contain both Node1 and Node2 for its entrances,
        // consider using a HashSet to avoid duplicate nodes if Node1 == Node2 in some cases.
        HashSet<HPANode> borderNodes = new HashSet<HPANode>();
        foreach (Entrance e in c.Entrances)
        {
            if (e.Node1 != n) borderNodes.Add(e.Node1);
            if (e.Node2 != n && e.Node2 != null) borderNodes.Add(e.Node2); // Add Node2 if it's used and valid
        }

        foreach (HPANode borderNode in borderNodes)
        {
            double distance = SearchForDistance(n, borderNode, c);
            if (distance < double.PositiveInfinity)
            {
                AddHPAEdge(n, borderNode, distance, level, HPAEdgeType.INTER); // Confirm edge type based on usage
            }
        }
    }


    public void insertNode(HPANode s, int maxLevel) // start node and finish node. 
    {
        if (!NodesByLevel[maxLevel].ContainsKey(s.Position))
        {
            NodesByLevel[maxLevel].Add(s.Position, s);
        }
        else
        {
            NodesByLevel[maxLevel][s.Position].Merge(s);
        }
        {

        }

        s = NodesByLevel[maxLevel][s.Position];

        for (int l = 1; l <= maxLevel; l++)
        {
            Cluster c = DetermineCluster(s, l);
            connectToBorder(s, c);
        }

        s.Level = maxLevel;

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
        if (absPath == null)
        {

            throw new Exception("No path found in hierarchicalSearch");
            return null;

        }
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




        List<HPANode> abstractPath = Astar.FindPath(start, goal, HPAEdgeType.INTER);




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
        List<HPANode> localPath = Astar.FindPath(startNode, endNode, HPAEdgeType.INTRA);

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






