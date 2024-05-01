using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MapObject
{
    public CheckPoint Create(Vector2Int arrayPosition, CheckPointConfig checkPointConfig)
    {
        Type = ObjectType.CheckPoint;
        ArrayPosition = arrayPosition;
        Object = Instantiate(checkPointConfig.Prefab, ConvertArrayToMap(arrayPosition), Quaternion.identity);
        CheckPoint checkPointComponent = Object.AddComponent<CheckPoint>();
        return checkPointComponent;
    }
}
