// written by: Gustav Clausen s214940

public class CellularAutomata
{



    public static int[,] Create2DMap(int height, int width, float density, int iterations, int erosionLimit)
    {
        int[,] map2D = new int[height, width];
        map2D = GenerateNoise(map2D, density);
        map2D = CellularAutomaton(map2D, iterations, erosionLimit);
        return map2D;
    }

    public static int[,] Erode(int[,] map, int erosionLimit)
    {
        return CellularAutomaton(map, 1, erosionLimit);
    }

    private static int[,] GenerateNoise(int[,] map, float density)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                int randomValue = UnityEngine.Random.Range(1, 101); // Generate a random value

                map[i, j] = randomValue > density ? 0 : 1; // Assign floor or wall based on density.
            }
        }
        return map;
    }

    private static int[,] CellularAutomaton(int[,] map, int cellularIterations, int erosionLimit)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);

        for (int i = 0; i < cellularIterations; i++)
        {
            int[,] tempGrid = (int[,])map.Clone();

            for (int j = 0; j < height; j++)
            {
                for (int k = 0; k < width; k++)
                {
                    int neighborWallCount = 0;

                    // Check all neighboring cells in a 3x3 grid around the current cell
                    for (int y = j - 1; y <= j + 1; y++)
                    {
                        for (int x = k - 1; x <= k + 1; x++)
                        {
                            // Ensure the neighbor coordinates are within the map bounds
                            if (x >= 0 && x < width && y >= 0 && y < height)
                            {
                                // Skip the current cell itself
                                if (y != j || x != k)
                                {
                                    // Increment the wall count if the neighbor cell is a wall
                                    if (tempGrid[y, x] == 1)
                                    {
                                        neighborWallCount++;
                                    }
                                }
                            }
                            else
                            {
                                neighborWallCount++; // Increment if out of bounds, assuming border as wall
                            }
                        }
                    }
                    map[j, k] = neighborWallCount > erosionLimit ? 1 : 0; // Update based on neighbor count
                }
            }
        }
        return map;
    }

    public static MapObject[,] Convert2DTo3D(int[,] map2D)
    {
        int width = map2D.GetLength(0);
        int height = map2D.GetLength(1);
        MapObject[,] map = new MapObject[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (map2D[i, j] == 0) map[i, j] = new Tile(new UnityEngine.Vector2Int(i, j));
                else map[i, j] = new Wall(new UnityEngine.Vector2Int(i, j));
            }
        }
        return map;
    }

    public static int[,] Convert3DTo2D(MapObject[,] map)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);
        int[,] map2D = new int[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                map2D[i, j] = map[i, j].GetType() == typeof(Tile) ? 0 : 1;
            }
        }
        return map2D;
    }
}