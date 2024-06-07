using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
// Written by: Andreas Sjögren Fürst (s201189)
public abstract class MapObject
{
    public enum ObjectType {
        Tile,
        Wall,
        CheckPoint,
        AgentSpawnPoint
    }
    public bool IsExplored { get; protected set; } = false;
    public Vector2Int ArrayPosition { get; protected set; }
    public ObjectType Type {get; protected set; }

    public void SetExplored(bool isExplored){
        IsExplored = isExplored;
    }
}
