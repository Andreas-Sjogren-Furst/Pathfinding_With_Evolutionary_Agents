using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MapObject
{
    public CheckPoint(Vector2Int arrayPosition) 
    {
        ArrayPosition = arrayPosition;
        Type = ObjectType.CheckPoint;
    }
}
