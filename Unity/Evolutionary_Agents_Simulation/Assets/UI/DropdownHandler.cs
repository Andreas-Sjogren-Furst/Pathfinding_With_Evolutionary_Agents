using UnityEngine;
using UnityEngine.UI;

public class DropdownHandler : MonoBehaviour
{
    Dropdown dropdown;
    PanelManager panelManager;
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
