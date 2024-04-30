public interface IEdgeManager
{
    void AddHPAEdge(HPANode node1, HPANode node2, double weight, int level, HPAEdgeType type);

    public HPAEdge[] UpdateEdgesFromRemovedNode(HPANode n);
    void ConnectToBorder(HPANode n, Cluster c);

}