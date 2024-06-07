using UnityEngine;
using UnityEngine.UI;
// Written by: Andreas Sjögren Fürst (s201189)
public class DropdownHandler2 : MonoBehaviour
{
    Dropdown dropdown;
    PanelManager1 panelManager;
    void Start()
    {
        // Get the Dropdown component from the same GameObject
        dropdown = GetComponent<Dropdown>();

        // Add listener to handle when the selected dropdown index changes
        dropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(dropdown); });
        
    }

    // This function is called each time the selected item changes
    void DropdownValueChanged(Dropdown change)
    {
        MapModel mapModel = CustomMaps.customMaps[change.value];
        //panelManager.ChangeValues(mapModel);
    }
}
