using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class ObjectDropper : MonoBehaviour
{
    public GameObject objectToDrop; // Assign your prefab in the Inspector
    private Camera cam;
    void Start(){
        cam = GetComponent<Camera>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Check if the left mouse button was clicked
        {
            DropObjectAtMousePosition();
        }
    }

    void DropObjectAtMousePosition()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) // Check if the ray hits something
        {
            Instantiate(objectToDrop, hit.point, Quaternion.identity); // Instantiate the object at the hit position
        }
    }
}
