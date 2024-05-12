using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class AgentSpawnPoint : MapObject
{   
    public AgentSpawnPoint(Vector2Int arrayPosition, SpawnPointConfig spawnPointConfig) 
    {
        ArrayPosition = arrayPosition;
        Type = ObjectType.AgentSpawnPoint;
    }
}
