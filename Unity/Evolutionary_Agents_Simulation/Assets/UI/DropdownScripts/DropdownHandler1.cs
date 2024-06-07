using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// Written by: Andreas Sjögren Fürst (s201189)
public class DropdownHandler1 : MonoBehaviour
{
    Dropdown dropdown;
    public Toggle hpaToggle;
    public Toggle pathToggle;
    void Start()
    {
        // Get the Dropdown component from the same GameObject
        dropdown = GetComponent<Dropdown>();

        // Add options for dropdown
        SetDropdownOptions();

        // Add listener to handle when the selected dropdown index changes
        dropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(dropdown); });
        
    }
    void SetDropdownOptions(){
        dropdown.ClearOptions();
        List<Dropdown.OptionData> dropdownOptions = new();
        int amountHPALevels = WebView.Instance.amountHPALevels;
        for(int i = 0; i < amountHPALevels; i++){
            dropdownOptions.Add(new Dropdown.OptionData((i + 1).ToString()));
        } dropdown.AddOptions(dropdownOptions);
        
    }

    // This function is called each time the selected item changes
    void DropdownValueChanged(Dropdown change)
    {
        WebView.Instance.ShowOrHideHPAGraph(false);
        WebView.Instance.SetCurrentHPALevel(change.value);
        WebView.Instance.RenderPath();
        WebView.Instance.ShowOrHidePath(pathToggle.isOn);
        if(hpaToggle.isOn) WebView.Instance.ShowOrHideHPAGraph(true);
        
        
    }
}
