using UnityEngine;
// Written by: Andreas Sjögren Fürst (s201189)
public class AgentSpawnPoint : MapObject
{   
    public AgentSpawnPoint(Vector2Int arrayPosition) 
    {
        ArrayPosition = arrayPosition;
        Type = ObjectType.AgentSpawnPoint;
    }
}
