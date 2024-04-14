public interface IEdgeManager
{
    void AddHPAEdge(HPANode node1, HPANode node2, double weight, int level, HPAEdgeType type);

    void ConnectToBorder(HPANode n, Cluster c);

}