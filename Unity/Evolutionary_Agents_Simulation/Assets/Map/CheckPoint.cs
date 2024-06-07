using UnityEngine;
// Written by: Andreas Sjögren Fürst (s201189)
public class CheckPoint : MapObject
{
    public CheckPoint(Vector2Int arrayPosition) 
    {
        ArrayPosition = arrayPosition;
        Type = ObjectType.CheckPoint;
    }
}
