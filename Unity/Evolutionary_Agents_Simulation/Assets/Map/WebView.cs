using System.Collections;
using System.Collections.Generic;
using Codice.Client.BaseCommands;
using UnityEngine;

public class WebView : MonoBehaviour, IScreenView
{

    public GameObject wallPrefab;
    public GameObject tilePrefab;
    public GameObject checkPointPrefab;
    public GameObject spawnPointPrefab;
    public GameObject agentPrefab;

    private GameObject[,] InstantiatedMap;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RenderMap(MapObject[,] Map, List<CheckPoint> checkPoints, AgentSpawnPoint spawnPoint){
        ClearMap(InstantiatedMap);

        foreach(MapObject mapObject in Map){
            Vector3Int worldPosition = ConvertVector2DTo3D(mapObject.ArrayPosition);
            int i = mapObject.ArrayPosition.x;
            int j = mapObject.ArrayPosition.y;
            switch(mapObject.Type)
            {
                case MapObject.ObjectType.Tile:
                    InstantiatedMap[i,j] = Instantiate(tilePrefab,worldPosition,Quaternion.identity);
                    break;
                case MapObject.ObjectType.Wall:
                    InstantiatedMap[i,j] = Instantiate(wallPrefab,worldPosition,Quaternion.identity);
                    break;
            }

        } 
        InstantiateCheckPoints(checkPoints);
        InstantiateSpawnPoint(spawnPoint);

    }

    private void InstantiateCheckPoints(List<CheckPoint> checkPoints){
        foreach(CheckPoint checkPoint in checkPoints){
            Vector3Int worldPosition = ConvertVector2DTo3D(checkPoint.ArrayPosition);
            int i = checkPoint.ArrayPosition.x;
            int j = checkPoint.ArrayPosition.y;
            InstantiatedMap[i,j] = Instantiate(checkPointPrefab,worldPosition,Quaternion.identity);
        } 
    }
    private void InstantiateSpawnPoint(AgentSpawnPoint spawnPoint){
        Vector3Int worldPosition = ConvertVector2DTo3D(spawnPoint.ArrayPosition);
        int i = spawnPoint.ArrayPosition.x;
        int j = spawnPoint.ArrayPosition.y;
        InstantiatedMap[i,j] = Instantiate(spawnPointPrefab,worldPosition,Quaternion.identity);
    }   
    private Vector3Int ConvertVector2DTo3D(Vector2Int arrayPosition){
        return new Vector3Int(arrayPosition.x,arrayPosition.y,0);
    }

    private void ClearMap(GameObject[,] InstantiatedMap){
        if(InstantiatedMap == null) return;
        foreach(GameObject mapObject in InstantiatedMap){
            Destroy(mapObject);
        }
    }
}
