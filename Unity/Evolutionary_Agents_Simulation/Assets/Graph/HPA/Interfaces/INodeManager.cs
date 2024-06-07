// written by: Gustav Clausen s214940

using UnityEngine;

public interface INodeManager
{
    HPANode FindOrCreateNode(int x, int y, Cluster cluster);
    void AddHPANode(HPANode n, int level);

    void insertCheckpoint(Vector2Int s, int maxLevel);

    HPANode GetNodeByPosition(Vector2Int position, int level);
    void RemoveNode(HPANode node);
}