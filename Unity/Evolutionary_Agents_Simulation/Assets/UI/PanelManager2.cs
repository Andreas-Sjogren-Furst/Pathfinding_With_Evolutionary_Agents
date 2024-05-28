using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelManager2 : MonoBehaviour
{
    Text text;

    void Start(){
        text = GetComponentInChildren<Text>();
        ScreenViewModel screenViewModel = WebView.Instance.screenPresenter.PackageData();
        text.text = screenViewModel.accessibleNodes.ToString();
    }
}
