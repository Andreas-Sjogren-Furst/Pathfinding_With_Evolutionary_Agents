using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelManager1 : MonoBehaviour
{
    Button submitButton;
    Dropdown dropdown;
    Slider[] sliders;
    InputField inputField;
    CameraAdjuster cameraAdjuster;

    // Start is called before the first frame update
    void Start()
    {
        cameraAdjuster = Camera.main.GetComponent<CameraAdjuster>();
        submitButton = GetComponentInChildren<Button>();
        dropdown = GetComponentsInChildren<Dropdown>()[1];
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

    private MapModel CreateMapModelFromUI()
    {
        MapModel mapModel;
        List<float> values = new();
        foreach (Slider slider in sliders)
        {
            values.Add(slider.value);
        }
        int mapSize = (int)values[0] * 10;
        float density = values[1];
        int iterations = (int)values[2];
        int numberOfCheckPoints = (int)values[3];
        int amountOfAgents = (int)values[4];
        int randomSeed = int.Parse(inputField.text);
        mapModel = new(density, iterations, mapSize, numberOfCheckPoints, amountOfAgents, randomSeed);
        return mapModel;
    }

    private void ChangeSliderValues(MapModel mapModel)
    {
        foreach (Slider slider in sliders)
        {
            Text[] texts = slider.GetComponentsInChildren<Text>();
            Text sliderValue = texts[0];
            Text sliderName = texts[1];
            switch (sliderName.text)
            {
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
                    sliderValue.text = mapModel.amountOfAgents.ToString();
                    slider.value = mapModel.amountOfAgents;
                    break;
                default:
                    break;
            }
        }
        inputField.text = mapModel.randomSeed.ToString();
    }

    private void ValidateInput(string input)
    {
        if (!int.TryParse(input, out int result)) { inputField.text = "0"; }
    }
    void OnSubmit()
    {
        ValidateInput(inputField.text);
        MapModel mapModel = CreateMapModelFromUI();
        WebView.Instance.CreateNewMap(mapModel);
        cameraAdjuster.AdjustCamera();
    }
}
