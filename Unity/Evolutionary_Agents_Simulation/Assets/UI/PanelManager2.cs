using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class PanelManager2 : MonoBehaviour
{
    Text text;
    
    void Start(){
        text = GetComponentInChildren<Text>();
        UpdateAmountDiscovered();
    }
    void Update(){
        UpdateAmountDiscovered();
    }

    public void UpdateAmountDiscovered(){
        ScreenViewModel screenViewModel = WebView.Instance.screenPresenter.PackageData();
        int amountOfVisibleTiles = screenViewModel.visibleTiles.Count;
        int accessibleNodes = screenViewModel.accessibleNodes;
        float amountDiscovered = (float)Math.Round((float)amountOfVisibleTiles/accessibleNodes * 100,1);
        if(amountDiscovered > 100) text.text = "100%";
        else text.text = amountDiscovered.ToString() + "%";
    }
}
