using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAdjuster : MonoBehaviour
{
   
    private int mapSize;
    private readonly int frameSize = 10;
    void Start()
    {
        AdjustCamera();
    }

    public void AdjustCamera(){
        ScreenViewModel screenViewModel = WebView.Instance.screenPresenter.PackageData();
        mapSize = screenViewModel.map.GetLength(0)/2 + frameSize;
        Camera.main.orthographicSize = mapSize;
    }

    
}
