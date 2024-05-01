using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public abstract class MapObject : MonoBehaviour
{
    public enum ObjectType
    {
        Tile,
        Wall,
        CheckPoint,
        AgentSpawnPoint
    }
    public bool IsExplored { get; protected set; } = false;
    public Vector2Int ArrayPosition { get; protected set; }
    public GameObject Object { get; protected set; }

    public ObjectType Type { get; protected set; }

    public void SetExplored(bool isExplored)
    {
        IsExplored = isExplored;
    }
    public Vector3Int ConvertArrayToMap(Vector2Int arrayPosition)
    {
        return new Vector3Int(arrayPosition.x * MapModel.scaleFactor, 0, arrayPosition.y * MapModel.scaleFactor);
    }


}
