using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class NodeManager : INodeManager
{
    private readonly IGraphModel _graphModel;
    private readonly IEdgeManager _edgeManager;

    public NodeManager(IGraphModel graphModel, IEdgeManager edgeManager)
    {
        _graphModel = graphModel;
        _edgeManager = edgeManager;
    }

    public HPANode FindOrCreateNode(int x, int y, Cluster cluster)
    {
        Vector2Int position = new Vector2Int(x, y);
        int level = cluster.Level;

        if (_graphModel.NodesByLevel.ContainsKey(level) && _graphModel.NodesByLevel[level].ContainsKey(position))
        {
            return _graphModel.NodesByLevel[level][position];
        }
        else
        {
            int nCount = _graphModel.NodesByLevel.ContainsKey(level) ? _graphModel.NodesByLevel[level].Count : 0;
            HPANode newNode = new HPANode(
                id: nCount,
                cluster: cluster,
                position: position,
                level: level);

            AddHPANode(newNode, level);
            return newNode;
        }
    }

    public void AddHPANode(HPANode n, int level)
    {
        if (!_graphModel.NodesByLevel.ContainsKey(level))
        {
            _graphModel.NodesByLevel[level] = new Dictionary<Vector2Int, HPANode>();
        }

        if (!_graphModel.NodesByLevel[level].ContainsKey(n.Position))
        {
            _graphModel.NodesByLevel[level].Add(n.Position, n);
        }
        else
        {
            _graphModel.NodesByLevel[level][n.Position].Merge(n);
        }
    }

    public void insertCheckpoint(Vector2Int sPos, int maxLevel) // start node and finish node. 
    {
        Cluster c = DetermineCluster(sPos, maxLevel);
        HPANode node = FindOrCreateNode(sPos.x, sPos.y, c);

        for (int l = 1; l <= maxLevel; l++)
        {
            Cluster cTemp = DetermineCluster(sPos, l);
            _edgeManager.ConnectToBorder(node, cTemp);
        }

        node.Level = maxLevel;

    }

    public HPANode GetNodeByPosition(Vector2Int position, int level)
    {
        if (_graphModel.NodesByLevel.ContainsKey(level))
        {
            var levelNodes = _graphModel.NodesByLevel[level];
            if (levelNodes.ContainsKey(position))
            {
                return levelNodes[position];
            }
        }
        return null; // Return null if no node is found at the given position and level
    }



    private Cluster DetermineCluster(Vector2Int nPos, int level)
    {
        // Assuming clusters are well-defined and partition the space such that every position falls into exactly one cluster at each level.
        foreach (Cluster cluster in _graphModel.ClusterByLevel[level])
        {
            if (cluster.Contains(nPos)) // Implement this Contains method based on your cluster definition.
            {
                return cluster;
            }
        }
        return null; // Or handle differently if your application expects every node to fit within a cluster.
    }


}