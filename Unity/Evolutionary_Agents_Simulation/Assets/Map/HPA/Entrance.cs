using System;

public class Entrance
{
    public Guid Id { get; set; }
    public Cluster Cluster1 { get; set; }
    public Cluster Cluster2 { get; set; }
    public HPANode Node1 { get; set; }
    public HPANode Node2 { get; set; }

    public Entrance(Cluster cluster1, Cluster cluster2, HPANode node1, HPANode node2)
    {
        Id = Guid.NewGuid();
        Cluster1 = cluster1;
        Cluster2 = cluster2;
        Node1 = node1;
        Node2 = node2;
    }
}