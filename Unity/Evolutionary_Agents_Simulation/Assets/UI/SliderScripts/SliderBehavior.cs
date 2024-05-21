using UnityEngine;
using UnityEngine.UI;

public class SliderBehavior : MonoBehaviour
{
    Slider mySlider;
    Text sliderValueText;

  
    // Start is called before the first frame update
    void Start()
    {
        
        
        mySlider = GetComponent<Slider>();
        sliderValueText = GetComponentInChildren<Text>();

        UpdateSliderValueText((int)mySlider.value);

        mySlider.onValueChanged.AddListener(delegate { UpdateSliderValueText((int)mySlider.value); });

    }

    void UpdateSliderValueText(int value)
    {
        // Update the text to display the current slider value
        sliderValueText.text = value.ToString(); // Display one decimal place
    }
}
