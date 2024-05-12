using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

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
