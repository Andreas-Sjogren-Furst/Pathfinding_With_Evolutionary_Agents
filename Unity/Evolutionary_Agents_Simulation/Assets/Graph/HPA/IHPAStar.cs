// written by: Gustav Clausen s214940
using System;
using UnityEngine;


public interface IHPAStar
{
    void Preprocessing(int maxLevel, int clusterSize = 10);
    void AbstractMaze(int clusterSize);
    void BuildGraph();
    void AddLevelToGraph(int l);
    HPAPath HierarchicalSearch(Vector2Int start, Vector2Int goal, int level);

    HPAPath HierarchicalAbstractSearch(Vector2Int start, Vector2Int goal, int level);

    public void DynamicallyAddHPANode(Vector2Int position, Boolean isFinalNodeInCluster = false);

    public void DynamicallyRemoveHPANode(Vector2Int position);

}
