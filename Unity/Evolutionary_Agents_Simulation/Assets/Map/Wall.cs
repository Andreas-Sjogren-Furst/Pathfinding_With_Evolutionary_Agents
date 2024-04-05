using UnityEngine;

public class Wall : MapObject
{
    public Wall(Vector2Int arrayPosition) : 
    base(arrayPosition)
    {
        Type = ObjectType.Wall;
        Prefab = AssetManager.wallPrefab;
        Tag = AssetManager.wallPrefab.tag;
    }
}
