using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapModel : ScriptableObject
{
    [Range(0, 20)]
    public int NumberOfCheckPoints;
    [Range(1, 10)]
    public int TileSize;
    [Range(0, 100)]
    public float Density; // Density field to control noise
    [Range(0, 100)]
    public int CellularIterations;
    [Range(1, 30)]
    public int CheckPointSpacing;
    [Range(1, 10)]
    public int ErosionLimit = 4;
    [Range(0, 10)]
    public int MapNumber = 0;

    public int RandomSeed = 42; // Added seed for random number generator

    public int mapSize = 100;

    public List<Vector3> ColonyCoordinates;





}
