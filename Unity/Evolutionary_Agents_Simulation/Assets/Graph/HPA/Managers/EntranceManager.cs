using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class EntranceManager : IEntranceManager
{
    private readonly IGraphModel _graphModel;
    private readonly INodeManager _nodeManager;

    public EntranceManager(IGraphModel graphModel, INodeManager nodeManager)
    {
        _graphModel = graphModel;
        _nodeManager = nodeManager;
    }

    public void CreateEntrancesBetweenClusters(Cluster cluster1, Cluster cluster2)
    {
        HashSet<Entrance> entrances = BuildEntrances(cluster1, cluster2);
        cluster1.Entrances.UnionWith(entrances);
        cluster2.Entrances.UnionWith(entrances);
    }

    public HashSet<Entrance> BuildEntrances(Cluster c1, Cluster c2, int maxGroupSize = 10)
    {
        HashSet<Entrance> AllEntrances = new HashSet<Entrance>();
        HashSet<Entrance> HorizontalEntrances = new HashSet<Entrance>();
        HashSet<Entrance> VerticalEntrances = new HashSet<Entrance>();

        if (!Adjacent(c1, c2)) return AllEntrances;


        if (c1.bottomLeftPos.y == c2.bottomLeftPos.y && c1.topRightPos.y == c2.topRightPos.y)
        {

            int sharedStartY = c1.bottomLeftPos.y;
            int sharedEndY = c1.topRightPos.y;

            int xForC1 = c1.topRightPos.x == c2.bottomLeftPos.x - 1 ? c1.topRightPos.x : c1.bottomLeftPos.x - 1;
            int xForC2 = xForC1 == c1.topRightPos.x ? c2.bottomLeftPos.x : c2.topRightPos.x;

            for (int y = sharedStartY; y <= sharedEndY; y++)
            {
                if (IsWalkable(new Vector2Int(xForC1, y), new Vector2Int(xForC2, y)))
                {
                    Debug.Log("Building entrances between clusters " + c1.bottomLeftPos + " and " + c2.bottomLeftPos);

                    var node1 = _nodeManager.FindOrCreateNode(xForC1, y, c1);
                    var node2 = _nodeManager.FindOrCreateNode(xForC2, y, c2);
                    HorizontalEntrances.Add(new Entrance(c1, c2, node1, node2));
                    HorizontalEntrances.Add(new Entrance(c2, c1, node2, node1));
                }
            }
        }
        else if (c1.bottomLeftPos.x == c2.bottomLeftPos.x && c1.topRightPos.x == c2.topRightPos.x)
        {
            int sharedStartX = c1.bottomLeftPos.x;
            int sharedEndX = c1.topRightPos.x;

            int yForC1 = c1.topRightPos.y == c2.bottomLeftPos.y - 1 ? c1.topRightPos.y : c1.bottomLeftPos.y - 1;
            int yForC2 = yForC1 == c1.topRightPos.y ? c2.bottomLeftPos.y : c2.topRightPos.y;

            for (int x = sharedStartX; x <= sharedEndX; x++)
            {
                if (IsWalkable(new Vector2Int(x, yForC1), new Vector2Int(x, yForC2)))
                {
                    Debug.Log("Building entrances between clusters " + c1.bottomLeftPos + " and " + c2.bottomLeftPos);

                    var node1 = _nodeManager.FindOrCreateNode(x, yForC1, c1);
                    var node2 = _nodeManager.FindOrCreateNode(x, yForC2, c2);
                    VerticalEntrances.Add(new Entrance(c1, c2, node1, node2));
                    VerticalEntrances.Add(new Entrance(c2, c1, node2, node1));
                }
            }
        }

        AllEntrances.UnionWith(GroupAndMergeEntrances(HorizontalEntrances, false, maxGroupSize));
        AllEntrances.UnionWith(GroupAndMergeEntrances(VerticalEntrances, true, maxGroupSize));

        return AllEntrances;
    }

    public HashSet<Entrance> GroupAndMergeEntrances(HashSet<Entrance> allEntrances, bool horizontalAlignment, int maxGroupSize = 10)
    {
        List<Entrance> entrances = allEntrances.ToList();
        List<List<Entrance>> groups = new List<List<Entrance>>();
        bool[] grouped = new bool[entrances.Count];

        for (int i = 0; i < entrances.Count; i++)
        {
            if (grouped[i]) continue;

            List<Entrance> currentGroup = new List<Entrance>();
            currentGroup.Add(entrances[i]);
            grouped[i] = true;

            for (int j = 0; j < entrances.Count; j++)
            {
                if (i == j || grouped[j]) continue;

                var lastEntrance = currentGroup.Last().Node1;
                var currentEntrance = entrances[j].Node1;

                bool areAdjacent = CheckAdjacency(lastEntrance, currentEntrance, horizontalAlignment);
                bool isWalkableBetween = IsWalkable(lastEntrance.Position, currentEntrance.Position);

                if (areAdjacent && isWalkableBetween)
                {
                    currentGroup.Add(entrances[j]);
                    grouped[j] = true;

                    if (currentGroup.Count >= maxGroupSize)
                    {
                        break;
                    }
                }
            }

            groups.Add(currentGroup);
        }

        HashSet<Entrance> groupedEntrances = new HashSet<Entrance>();
        foreach (var group in groups)
        {
            if (group.Count > 0)
            {
                groupedEntrances.Add(group[group.Count / 2]);
            }
        }

        return groupedEntrances;
    }

    public HashSet<Entrance> GetEntrances(Cluster c1, Cluster c2)
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
                if (e1 == e2)
                {
                    commonEntrances.Add(e1);
                    break;
                }
            }
        }

        return commonEntrances;
    }

    public void RemoveEntrance(Entrance entrance)
    {
        bool existsInCluster1 = entrance.Cluster1.Entrances.Contains(entrance);
        bool existsInCluster2 = entrance.Cluster2.Entrances.Contains(entrance);
        bool existsInLevel = _graphModel.EntrancesByLevel[entrance.Node1.Level].Contains(entrance);

        Debug.Log($"Existence in Cluster1: {existsInCluster1}, Cluster2: {existsInCluster2}, Level: {existsInLevel}");

        entrance.Cluster1.Entrances.Remove(entrance);
        entrance.Cluster2.Entrances.Remove(entrance);
        _graphModel.EntrancesByLevel[entrance.Node1.Level].Remove(entrance);
    }


    private bool Adjacent(Cluster c1, Cluster c2)
    {
        return c1.bottomLeftPos.x == c2.topRightPos.x + 1 || c1.topRightPos.x == c2.bottomLeftPos.x - 1 ||
               c1.bottomLeftPos.y == c2.topRightPos.y + 1 || c1.topRightPos.y == c2.bottomLeftPos.y - 1;
    }

    private bool CheckAdjacency(HPANode node1, HPANode node2, bool horizontalAlignment)
    {
        if (horizontalAlignment)
        {
            return node1.Position.y == node2.Position.y && Math.Abs(node1.Position.x - node2.Position.x) == 1;
        }
        else
        {
            return node1.Position.x == node2.Position.x && Math.Abs(node1.Position.y - node2.Position.y) == 1;
        }
    }



    private bool IsWalkable(Vector2Int pos1, Vector2Int pos2)
    {
        int[,] globalTileMap = _graphModel.GlobalTileMap;
        return globalTileMap[pos1.x, pos1.y] != 1 && globalTileMap[pos2.x, pos2.y] != 1;
    }
}