using System.Collections.Generic;

public interface IEdgeManager
{
    void AddHPAEdge(HPANode node1, HPANode node2, double weight, int level, HPAEdgeType type, HPAPath IntraPath = null);

    public HPAEdge[] UpdateEdgesFromRemovedNode(HPANode n);
    public List<HPAPath> ConnectToBorder(HPANode n, Cluster c);

}