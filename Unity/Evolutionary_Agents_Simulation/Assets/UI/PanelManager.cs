using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    Button submitButton;
    Dropdown dropdown;
    Slider[] sliders;

    
    // Start is called before the first frame update
    void Start()
    {
        submitButton = GetComponentInChildren<Button>();
        dropdown = GetComponentInChildren<Dropdown>();
        sliders = GetComponentsInChildren<Slider>();
        
        submitButton.onClick.AddListener(OnSubmit);    
    }

    void OnSubmit(){
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
