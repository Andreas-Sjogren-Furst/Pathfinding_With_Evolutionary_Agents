using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MapObject
{
    public CheckPoint(Vector2Int arrayPosition) :
    base(arrayPosition)
    {
        Type = ObjectType.CheckPoint;
        Prefab = AssetManager.checkPointPrefab;
        Tag = AssetManager.checkPointPrefab.tag;
    }
}
