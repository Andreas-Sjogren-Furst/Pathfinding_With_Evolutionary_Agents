using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "MapModel", menuName = "Map/MapModel", order = 0)]
public class MapModel : ScriptableObject
{
    public readonly int CheckPointSpacing = 3;
    public readonly int ErosionLimit = 4;
    public int RandomSeed = 42;
    [Range(0, 20)] public int NumberOfCheckPoints;
    [Range(0, 100)] public float Density;
    [Range(1, 30)] public int CellularIterations;
    [Range (5,100)] public int mapWidth;
    [Range (5,100)] public int mapHeight;
    public MapObject[,] Map;
    public List<CheckPoint> CheckPoints;
    public AgentSpawnPoint SpawnPoint;
}
