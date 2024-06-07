
using System.Collections.Generic;
// Written by: Andreas Sjögren Fürst (s201189)
public class GridExplorer 
{
    public static int CountAccessibleNodes(MapObject[,] map, AgentSpawnPoint spawnPoint)
    {
        int rowLength = map.GetLength(0);
        int colLength = map.GetLength(1);
        
        int[] dx = { -1, 0, 1, 0 };
        int[] dy = { 0, 1, 0, -1 };
        
        int startX = spawnPoint.ArrayPosition.x;
        int startY = spawnPoint.ArrayPosition.y;
        
        // Check if starting point is valid
        if (startX < 0 || startX >= colLength || startY < 0 || startY >= rowLength || map[startY, startX].Type == MapObject.ObjectType.Wall)
            return 0;
        
        // Queue for managing BFS 
        Queue<(int x, int y)> queue = new();
        queue.Enqueue((startX, startY));

        // Set to keep track of visited nodes
        bool[,] visited = new bool[rowLength, colLength];
        visited[startY, startX] = true;

        // Variable to count accessible nodes
        int accessibleCount = 0;

        while (queue.Count > 0)
        {
            (int x, int y) = queue.Dequeue();
            accessibleCount++;

            // Explore adjacent cells around current node
            for (int i = 0; i < 4; i++)
            {
                int newX = x + dx[i];
                int newY = y + dy[i];

                // Check bounds and whether the cell is walkable and not visited
                if (newX >= 0 && newX < colLength && newY >= 0 && newY < rowLength && map[newX, newY].Type == MapObject.ObjectType.Tile && !visited[newX, newY])
                {
                    queue.Enqueue((newX, newY));
                    visited[newX, newY] = true;  // Mark as visited
                }
            }
        } return accessibleCount;
    }

    public static HashSet<Point> ClusterFrontierPoints(HashSet<Point> frontierPoints, Point frontierPoint, MapObject[,] map){
        int rowLength = map.GetLength(0);
        int colLength = map.GetLength(1);
        
        int[] dx = { -1, 0, 1, 0, 1, -1, -1, 1};
        int[] dy = { 0, 1, 0, -1, 1, -1, 1, -1};
        
        int startX = frontierPoint.x;
        int startY = frontierPoint.y;
        
        // Check if starting point is valid
        if (startX < 0 || startX >= colLength || startY < 0 || startY >= rowLength || map[startX, startY].Type == MapObject.ObjectType.Wall)
            throw new System.Exception("invalid frontier point");
        
        // Queue for managing BFS 
        Queue<(int x, int y)> queue = new();
        queue.Enqueue((startX, startY));

        // Set to keep track of visited nodes
        bool[,] visited = new bool[rowLength, colLength];
        visited[startY, startX] = true;

        // Variable to cluster frontier points
        HashSet<Point> clusteredFrontierPoints = new();

        while (queue.Count > 0)
        {
            (int x, int y) = queue.Dequeue();
            clusteredFrontierPoints.Add(new Point(x,y));

            // Explore adjacent cells around current node
            for (int i = 0; i < 8; i++)
            {
                int newX = x + dx[i];
                int newY = y + dy[i];

                // Check bounds and whether the cell is walkable and not visited
                if (newX >= 0 && newX < colLength && newY >= 0 && newY < rowLength && map[newX, newY].Type == MapObject.ObjectType.Tile && !visited[newX, newY])
                {
                    if(frontierPoints.Contains(new Point(newX,newY))){
                        queue.Enqueue((newX, newY));
                        visited[newX, newY] = true;
                    }
                     
                }
            }
        } return clusteredFrontierPoints;
    }
}
