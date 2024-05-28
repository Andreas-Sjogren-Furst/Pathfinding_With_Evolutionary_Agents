using UnityEngine;
using UnityEngine.UI;

public class Toggle3 : MonoBehaviour
{
    Toggle myToggle;

    void Start()
    {
        //Fetch the Toggle GameObject
        myToggle = GetComponent<Toggle>();
        //Add listener for when the state of the Toggle changes, to take action
        myToggle.onValueChanged.AddListener(delegate { ToggleValueChanged(myToggle); });
    }

    void ToggleValueChanged(Toggle change)
    {
        WebView.Instance.ShowOrHideMap(change.isOn);
        Debug.Log("heyo");
    }
}
