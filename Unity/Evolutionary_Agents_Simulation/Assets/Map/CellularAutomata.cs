
public class CellularAutomata
{
   
    public static int[,] Create2DMap(MapModel mapModel){
        int[,] map2D = new int[mapModel.mapHeight, mapModel.mapWidth];
        map2D = GenerateNoise(map2D, mapModel.Density);
        map2D = CellularAutomaton(map2D, mapModel.CellularIterations, mapModel.ErosionLimit);
        return map2D;
    }

    public static int[,] Erode(int[,] map, int erosionLimit){
        return CellularAutomaton(map, 1, erosionLimit);
    }

    private static int[,] GenerateNoise(int[,] map, float density)
    {
        int height = map.GetLength(0);
        int width = map.GetLength(1);
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                int randomValue = UnityEngine.Random.Range(0, 101); // Generate a random value
                map[i, j] = randomValue > density ? 0 : 1; // Assign floor or wall based on density.
            }
        } return map;
    }

    private static int[,] CellularAutomaton(int[,] map, int cellularIterations, int erosionLimit)
    {
        int height = map.GetLength(0);
        int width = map.GetLength(1);

        for (int i = 0; i < cellularIterations; i++)
        {
            int[,] tempGrid = (int[,])map.Clone();

            for (int j = 0; j < height; j++)
            {
                for (int k = 0; k < width; k++)
                {
                    int neighborWallCount = 0;

                    for (int y = j - 1; y <= j + 1; y++)
                    {
                        for (int x = k - 1; x <= k + 1; x++)
                        {
                            if (x >= 0 && x < width && y >= 0 && y < height)
                            {
                                if (y != j || x != k)
                                {
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
        } return map;
    }
}