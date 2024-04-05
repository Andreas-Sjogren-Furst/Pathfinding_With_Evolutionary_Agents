using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class AgentSpawnPoint : MapObject
{
    public static Vector2Int spawnPoint;
    public AgentSpawnPoint(Vector2Int arrayPosition) : 
    base(arrayPosition)
    {
        Type = ObjectType.AgentSpawnPoint;
        Prefab = AssetManager.agentSpawnPointPrefab;
        spawnPoint = arrayPosition;
        Tag = AssetManager.agentSpawnPointPrefab.tag;
    }

}
