using UnityEngine;

public class Wall : MapObject
{
    public Wall(Vector2Int arrayPosition) 
    {
        ArrayPosition = arrayPosition;
        Type = ObjectType.Wall;
    }
}
