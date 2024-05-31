using System.Collections.Generic;


public class FrontierExplorer
{
    private readonly MapObject[,] map;

    public FrontierExplorer(MapObject[,] map){
        this.map = map;
    }
    public HashSet<Point> FindFrontierPoints(HashSet<Point> visibleTiles, HashSet<Point> visibleWalls) {
        HashSet<Point> frontierPoints = new();
        foreach(Point point in visibleTiles){
            List<Point> neighBoringPoints = FindNeighboringPoints(point);
            foreach(Point neighBoringPoint in neighBoringPoints){
                if(!visibleTiles.Contains(neighBoringPoint) && !visibleWalls.Contains(neighBoringPoint)){
                    frontierPoints.Add(neighBoringPoint);
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
