using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;




public interface IPathFinder
{

    HPAPath FindAbstractPath(HPANode start, HPANode goal, int level);
    HPAPath RefinePath(HPAPath abstractPath, int level);
    HPAPath FindLocalPath(HPANode start, HPANode end, Cluster cluster);

}