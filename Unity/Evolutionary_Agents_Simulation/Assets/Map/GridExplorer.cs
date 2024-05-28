using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                if (newX >= 0 && newX < colLength && newY >= 0 && newY < rowLength && map[newY, newX].Type == MapObject.ObjectType.Tile && !visited[newY, newX])
                {
                    queue.Enqueue((newX, newY));
                    visited[newY, newX] = true;  // Mark as visited
                }
            }
        } return accessibleCount;
    }
}
