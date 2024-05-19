using System.Collections.Generic;
using UnityEngine;

public class MapModel 
{
    public readonly int CheckPointSpacing = 3;
    public readonly int ErosionLimit = 4;
    public int RandomSeed;
    public int AmountOfCheckPoints;
    public float Density;
    public int CellularIterations;
    public int Width;
    public int Height;
    public MapObject[,] Map;
    public List<CheckPoint> CheckPoints;
    public AgentSpawnPoint SpawnPoint;
    MapModel(){}
}
