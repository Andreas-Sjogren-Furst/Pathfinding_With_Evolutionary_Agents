using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;




public interface IEntranceManager
{
    void CreateEntrancesBetweenClusters(Cluster cluster1, Cluster cluster2);
    HashSet<Entrance> BuildEntrances(Cluster c1, Cluster c2, int maxGroupSize = 10);
    HashSet<Entrance> GroupAndMergeEntrances(HashSet<Entrance> allEntrances, bool horizontalAlignment, int maxGroupSize = 10);
    HashSet<Entrance> GetEntrances(Cluster c1, Cluster c2);


}