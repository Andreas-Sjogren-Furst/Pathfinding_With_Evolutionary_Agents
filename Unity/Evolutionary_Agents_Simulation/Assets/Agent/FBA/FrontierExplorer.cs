using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;


public class FrontierExplorer
{
    private readonly MapObject[,] map;

    public FrontierExplorer(MapObject[,] map){
        this.map = map;
    }

    public Dictionary<int,Point> FindFrontier(HashSet<Point> visibleTiles, HashSet<Point> visibleWalls){
        Dictionary<int,Point> frontier = new();
        HashSet<Point> frontierPoints = FindFrontierPoints(visibleTiles,visibleWalls);
        int frontierPointId = 0;
        while(frontierPoints.Count != 0){
            List<Point> temp = frontierPoints.ToList();
            Point point = temp[0];
            HashSet<Point> frontierCluster = GridExplorer.ClusterFrontierPoints(frontierPoints,point,map);
            frontier.Add(frontierPointId, FindClosestToCentroid(frontierCluster));
            frontierPoints.ExceptWith(frontierCluster);
            frontierPointId++;
        } return frontier;
    }

    public Point FindClosestToCentroid(HashSet<Point> frontierCluster){
        Point centroid = CalculateCentroid(frontierCluster);
        List<Point> frontierClusterList = frontierCluster.ToList();
        Point closestPoint = frontierClusterList[0];
        double minDistance = double.MaxValue;
        foreach(Point frontierPoint in frontierCluster){
            double distance = CalculateEuclideanDistance(frontierPoint, centroid);
            if(distance < minDistance){
                minDistance = distance;
                closestPoint = frontierPoint;
            }
        }
        return closestPoint;
    }

    private Point CalculateCentroid(HashSet<Point> frontierCluster){
        int sumX = 0;
        int sumY = 0;
        foreach(Point frontierPoint in frontierCluster){
            sumX += frontierPoint.x;
            sumY += frontierPoint.y;
        }
        int totalFrontierPoints = frontierCluster.Count;
        return new Point(sumX/totalFrontierPoints, sumY/totalFrontierPoints);
    }

    private double CalculateEuclideanDistance(Point p1, Point p2){
        int dx = p1.x - p2.x; 
        int dy = p1.y - p1.y;
        return Math.Sqrt(dx*dx + dy * dy);
    }

    public HashSet<Point> FindFrontierPoints(HashSet<Point> visibleTiles, HashSet<Point> visibleWalls) {
        HashSet<Point> frontierPoints = new();
        foreach(Point point in visibleTiles){
            List<Point> neighBoringPoints = FindNeighboringPoints(point);
            foreach(Point neighBoringPoint in neighBoringPoints){
                if(!visibleTiles.Contains(neighBoringPoint) && !visibleWalls.Contains(neighBoringPoint)){
                    frontierPoints.Add(point);
                }
            }
        } return frontierPoints;
    }

    private List<Point> FindNeighboringPoints(Point point){
        List<Point> neighBoringPoints = new();
        int[] dx = { -1, 0, 1, 0 };
        int[] dy = { 0, 1, 0, -1 };
        for(int i = 0; i < 4; i++){
            Point neighboringPoint = new(point.x + dx[i],point.y + dy[i]);
            if(neighboringPoint.x >= 0 && neighboringPoint.x < map.GetLength(0) && neighboringPoint.y >= 0 && neighboringPoint.y < map.GetLength(1)){
                neighBoringPoints.Add(neighboringPoint);
            }
        } return neighBoringPoints;
    }
}
