using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class AntDataExport
{
    int steps;
    public AntDataExport(int steps){
        this.steps = steps;
    }
}

[System.Serializable]
public class ColonyData
{
    public int AntCount;
    public Vector3 position;

    public List<AntDataExport> Ants = new();
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