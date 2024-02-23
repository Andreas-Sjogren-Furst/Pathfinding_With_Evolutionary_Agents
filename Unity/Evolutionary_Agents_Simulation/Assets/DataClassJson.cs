using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class AntData
{
    public int Steps;
}

[System.Serializable]
public class ColonyData
{
    public int AntCount;
    public Vector3 position;

    public List<AntData> Ants = new List<AntData>();
}

[System.Serializable]
public class CheckPointData
{
    public int FoodCount;
    public Vector3 position;
    // position


}

[System.Serializable]
public class Data
{
    public List<ColonyData> Colonies = new List<ColonyData>();
    public List<CheckPointData> CheckPoints = new List<CheckPointData>();
}