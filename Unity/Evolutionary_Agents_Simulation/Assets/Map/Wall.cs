using UnityEngine;

public class Wall : MapObject
{
    public Wall Create(Vector2Int arrayPosition, WallConfig wallConfig)
    {

        Type = ObjectType.Wall;
        ArrayPosition = arrayPosition;
        Object = Instantiate(wallConfig.Prefab, ConvertArrayToMap(arrayPosition), Quaternion.identity);
        Wall wallComponent = Object.AddComponent<Wall>();
        return wallComponent;
    }
}
