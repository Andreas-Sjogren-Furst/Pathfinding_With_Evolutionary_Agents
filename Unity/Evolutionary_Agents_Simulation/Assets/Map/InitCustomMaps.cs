using System.Collections.Generic;

public class InitCustomMaps
{
    public readonly int density;
    public readonly int numberOfCheckPoints;
    public readonly int cellularIterations;
    public readonly int checkPointSpacing;
    public readonly int erosionLimit;
    public readonly int randomSeed;
    public InitCustomMaps(int density, int numberOfCheckPoints, int tileSize, int cellularIterations,
                          int checkPointSpacing, int erosionLimit, int randomSeed)
    {
        this.density = density;
        this.numberOfCheckPoints = numberOfCheckPoints;
        this.cellularIterations = cellularIterations;
        this.checkPointSpacing = checkPointSpacing;
        this.erosionLimit = erosionLimit;
        this.randomSeed = randomSeed;
    }
}
