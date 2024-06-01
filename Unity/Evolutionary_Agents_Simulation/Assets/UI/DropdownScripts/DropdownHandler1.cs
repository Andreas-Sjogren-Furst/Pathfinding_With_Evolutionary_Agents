using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownHandler1 : MonoBehaviour
{
    Dropdown dropdown;
    public Toggle toggle;
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
        if(toggle.isOn) WebView.Instance.ShowOrHideHPAGraph(true);
    }
}
