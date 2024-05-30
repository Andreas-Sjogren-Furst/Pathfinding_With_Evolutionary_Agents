using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PanelManager2 : MonoBehaviour
{
    Text text;

    void Start(){
        text = GetComponentInChildren<Text>();
        UpdateAmountDiscovered();
    }

    public void UpdateAmountDiscovered(){
        ScreenViewModel screenViewModel = WebView.Instance.screenPresenter.PackageData();
        int amountOfVisibleTiles = screenViewModel.visibleTiles.Count;
        int accessibleNodes = screenViewModel.accessibleNodes;
        float amountDiscovered = (float)Math.Round((float)amountOfVisibleTiles/accessibleNodes * 100,1);
        text.text = amountDiscovered.ToString() + "%";
    }
}
