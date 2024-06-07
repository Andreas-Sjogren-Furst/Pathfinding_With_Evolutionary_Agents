// written by: Gustav Clausen s214940

using System.Collections.Generic;

public class HPAPath
{
    public List<HPANode> path;
    public double Length => CalculateDistance(path);

    public int NodesExplored { get; set; }



    public HPAPath(List<HPANode> path)
    {
        this.path = path;
    }

    public double CalculateDistance(List<HPANode> path)
    {
        // Implementation to calculate the distance of a path
        // This method should calculate the total distance of the given path
        // You can sum the distances between consecutive points in the path
        // Return the total distance of the path

        if (path == null)
        {
            return double.PositiveInfinity;
        }
        double distance = 0;

        for (int i = 0; i < path.Count - 1; i++)
        {
            if (path[i] == null || path[i + 1] == null)
            {
                return double.PositiveInfinity;
            }
            distance += UnityEngine.Vector2Int.Distance(path[i].Position, path[i + 1].Position);
        }

        return distance;

    }

    public void AddRange(HPAPath path)
    {
        if (path == null || this.path == null || path.path == null)
        {
            return;
        }
        this.path.AddRange(path.path);
        this.NodesExplored += path.NodesExplored;
    }
}