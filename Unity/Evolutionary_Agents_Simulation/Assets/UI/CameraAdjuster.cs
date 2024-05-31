using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAdjuster : MonoBehaviour
{
   
    private int mapSize;
    private readonly int cameraHeight = 50;
    private readonly int frameSize = 10;
    void Start()
    {
        AdjustCamera();
    }

    public void AdjustCamera(){
        ScreenViewModel screenViewModel = WebView.Instance.screenPresenter.PackageData();
        mapSize = screenViewModel.map.GetLength(0)/2 + frameSize;
        int x = screenViewModel.spawnPoint.ArrayPosition.x;
        int z = screenViewModel.spawnPoint.ArrayPosition.y;
        Vector3 cameraPosition = new(x,cameraHeight,z);
        Camera.main.orthographicSize = mapSize;
        Camera.main.transform.position = cameraPosition;
    }

    
}
