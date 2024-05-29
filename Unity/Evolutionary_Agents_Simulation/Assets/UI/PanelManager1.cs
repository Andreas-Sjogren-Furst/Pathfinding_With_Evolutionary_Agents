using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelManager1 : MonoBehaviour
{
    Button submitButton;
    Dropdown dropdown;
    Slider[] sliders;
    InputField inputField;
    
    // Start is called before the first frame update
    void Start()
    {
        submitButton = GetComponentInChildren<Button>();
        dropdown = GetComponentInChildren<Dropdown>();
        sliders = GetComponentsInChildren<Slider>();
        inputField = GetComponentInChildren<InputField>();
        
        // Add listener to handle when the selected dropdown index changes
        dropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(dropdown); });
        submitButton.onClick.AddListener(OnSubmit);    
    }

    void DropdownValueChanged(Dropdown change)
    {
        MapModel mapModel = CustomMaps.customMaps[change.value];
        ChangeSliderValues(mapModel);
    }

    private MapModel CreateMapModelFromUI(){
        MapModel mapModel;
        List<string> values = new();
        foreach(Slider slider in sliders){
            values.Add(slider.GetComponentsInChildren<Text>()[0].text);
        }
        mapModel = new(float.Parse(values[0]),int.Parse(values[1]),int.Parse(values[2]),int.Parse(values[3]),int.Parse(values[4]));
        return mapModel;
    }

    private void ChangeSliderValues(MapModel mapModel){
        foreach(Slider slider in sliders){
            Text[] texts = slider.GetComponentsInChildren<Text>();
            Text sliderValue = texts[0];
            Text sliderName = texts[1];
            switch(sliderName.text){
                case "Size":
                    sliderValue.text = mapModel.width.ToString();
                    slider.value = mapModel.width;
                    break;
                case "Density":
                    sliderValue.text = mapModel.density.ToString();
                    slider.value = mapModel.density;
                    break;
                case "Iterations":
                    sliderValue.text = mapModel.iterations.ToString();
                    slider.value = mapModel.iterations;
                    break;
                case "Seed":
                    sliderValue.text = mapModel.randomSeed.ToString();
                    slider.value = mapModel.randomSeed;
                    break;
                case "Points":
                    sliderValue.text = mapModel.numberOfCheckPoints.ToString();
                    slider.value = mapModel.numberOfCheckPoints;
                    break;
                case "Agents":
                    sliderValue.text = "1";
                    break;
                default:
                    Debug.Log(sliderName.text);
                    break;
            }
        } inputField.text = mapModel.randomSeed.ToString();
    }

    private void ValidateInput(string input){
        if (!int.TryParse(input, out int result)) { inputField.text = "0"; }
    }
    void OnSubmit(){
        ValidateInput(inputField.text);
        MapModel mapModel = CreateMapModelFromUI();
        WebView.Instance.CreateMap(mapModel);
        WebView.Instance.RenderMap();
    }
}
