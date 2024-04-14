using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;




public interface IPathFinder
{

    List<HPANode> FindAbstractPath(HPANode start, HPANode goal, int level);
    List<HPANode> RefinePath(List<HPANode> abstractPath, int level);
    List<HPANode> FindLocalPath(HPANode start, HPANode end, Cluster cluster);
    double CalculateDistance(List<HPANode> path);

}