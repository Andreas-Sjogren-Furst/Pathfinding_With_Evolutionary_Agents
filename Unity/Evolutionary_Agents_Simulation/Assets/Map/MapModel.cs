using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "MapModel", menuName = "MapModel", order = 0)]
public class MapModel : ScriptableObject
{
    [Range(0, 20)] public int NumberOfCheckPoints;
    public static readonly int scaleFactor = 10;
    [Range(0, 100)] public float Density;
    [Range(1, 30)] public int CellularIterations;
    [Range(1, 10)] public int CheckPointSpacing;
    [Range(0, 10)] public int ErosionLimit;
    public int MapNumber = 0;
    public int RandomSeed = 42; // Added seed for random number generator
    [Range (5,100)] public int mapWidth;
    [Range (5,100)] public int mapHeight;
    public MapObject[,] Map;
    public List<CheckPoint> CheckPoints;
    public AgentSpawnPoint SpawnPoint;
}
