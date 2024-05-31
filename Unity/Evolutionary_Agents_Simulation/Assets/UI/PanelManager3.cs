using UnityEngine;
using UnityEngine.UI;

public class PanelManager3 : MonoBehaviour
{

    Button hideButton;
    Text text;
    public GameObject panel1;
    private bool isOn;
    
    void Awake(){
        isOn = true;
    }

    void Start()
    {
        hideButton = GetComponentInChildren<Button>();
        text = hideButton.GetComponentInChildren<Text>();
        hideButton.onClick.AddListener(OnSubmit);
    }

    void OnSubmit(){
        isOn = !isOn;
        if(isOn) text.text = "Hide";
        else text.text = "Show";
        panel1.SetActive(isOn);       
    }
    
}
