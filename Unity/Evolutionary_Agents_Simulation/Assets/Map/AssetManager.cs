using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetManager : MonoBehaviour
{
    public static GameObject tilePrefab;
    public static GameObject wallPrefab;
    public static GameObject checkPointPrefab;
    public static GameObject agentSpawnPointPrefab;

    void Awake(){
        tilePrefab = Resources.Load<GameObject>("TilePrefab");
        wallPrefab = Resources.Load<GameObject>("WallPrefab");
        checkPointPrefab = Resources.Load<GameObject>("CheckPointPrefab");
        agentSpawnPointPrefab = Resources.Load<GameObject>("AgentSpawnPointPrefab");

        // Check if any of the prefabs failed to load and log a warning or error
        if (tilePrefab == null) Debug.LogWarning("TilePrefab could not be loaded. Please check the path and name.");
        
        if (wallPrefab == null) Debug.LogWarning("WallPrefab could not be loaded. Please check the path and name.");
        
        if (checkPointPrefab == null) Debug.LogWarning("CheckPointPrefab could not be loaded. Please check the path and name.");

        if (agentSpawnPointPrefab == null) Debug.LogWarning("AgentSpawnPointPrefab could not be loaded. Please check the path and name.");

    }
}
