using UnityEngine;

public class AgentSpawnPoint : MapObject
{   
    public AgentSpawnPoint(Vector2Int arrayPosition) 
    {
        ArrayPosition = arrayPosition;
        Type = ObjectType.AgentSpawnPoint;
    }
}
