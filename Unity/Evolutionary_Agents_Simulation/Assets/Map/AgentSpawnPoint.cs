using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class AgentSpawnPoint : MapObject
{
    public AgentSpawnPoint Create(Vector2Int arrayPosition, SpawnPointConfig spawnPointConfig)
    {
        Type = ObjectType.AgentSpawnPoint;
        ArrayPosition = arrayPosition;
        Object = Instantiate(spawnPointConfig.Prefab, ConvertArrayToMap(arrayPosition), Quaternion.identity);
        AgentSpawnPoint agentSpawnPointComponent = Object.AddComponent<AgentSpawnPoint>();
        return agentSpawnPointComponent;
    }
}
