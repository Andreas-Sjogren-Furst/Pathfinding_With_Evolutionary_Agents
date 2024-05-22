using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IScreenView
{
    void RenderMap(MapObject[,] Map, List<CheckPoint> checkPoints, AgentSpawnPoint spawnPoint);

    
}   
