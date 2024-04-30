using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public interface IHPAStar
{
    void Preprocessing(int maxLevel);
    void AbstractMaze();
    void BuildGraph();
    void AddLevelToGraph(int l);
    List<HPANode> HierarchicalSearch(Vector2Int start, Vector2Int goal, int level);

    public void DynamicallyAddHPANode(Vector2Int position, Boolean isFinalNodeInCluster = false);

}
