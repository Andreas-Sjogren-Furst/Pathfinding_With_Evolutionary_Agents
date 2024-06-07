using UnityEngine;
// Written by: Andreas Sjögren Fürst (s201189)
public class Wall : MapObject
{
    public Wall(Vector2Int arrayPosition) 
    {
        ArrayPosition = arrayPosition;
        Type = ObjectType.Wall;
    }
}
