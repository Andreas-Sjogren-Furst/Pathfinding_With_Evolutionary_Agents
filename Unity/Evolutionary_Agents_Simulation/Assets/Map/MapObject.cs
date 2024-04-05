using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public abstract class MapObject : MonoBehaviour
{
    public enum ObjectType {
        Tile,
        Wall,
        CheckPoint,
        AgentSpawnPoint
    }
    public readonly Vector3Int MapPosition;
    public readonly Vector2Int ArrayPosition;
    public readonly Quaternion Rotation;
    public GameObject Object { get; protected set; }
    protected GameObject Prefab;
    public string Tag { get; protected set; }
    public ObjectType Type { get; protected set; }
    public MapObject(Vector2Int arrayPosition){
        MapPosition = ConvertArrayToMap(arrayPosition);
        ArrayPosition = arrayPosition;
        Rotation = Quaternion.identity;
    }

    public void Spawn(){
        Object = Instantiate(Prefab, MapPosition, Rotation);
    }

    private Vector3Int ConvertArrayToMap(Vector2Int arrayPosition){
        return new Vector3Int(arrayPosition.x * MapModel.scaleFactor, 0 , arrayPosition.y * MapModel.scaleFactor);
    }

    public static MapObject CreateObjectFromType(int arrayNumber, int i, int j){
        ObjectType objectType = (ObjectType)arrayNumber;
        return objectType switch
        {
            ObjectType.Tile => new Tile(new Vector2Int(i, j)),
            ObjectType.Wall => new Wall(new Vector2Int(i, j)),
            ObjectType.CheckPoint => new CheckPoint(new Vector2Int(i, j)),
            ObjectType.AgentSpawnPoint => new AgentSpawnPoint(new Vector2Int(i, j)),
            _ => throw new System.Exception("Invalid arrayNumber in the method CreateObjectFromType"),
        };
    }
}
