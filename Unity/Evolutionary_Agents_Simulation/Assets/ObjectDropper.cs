using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

public class ObjectDropper : MonoBehaviour
{
    public GameObject pheromone; // Assign your prefab in the Inspector
    public GameObject colony; // Assign your prefab in the Inspector
    private Camera cam;
    void Start(){
        cam = GetComponent<Camera>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Check if the left mouse button was clicked
        {   
            DropObjectAtMousePosition(pheromone);
        } else if(Input.GetMouseButtonDown(1)){ // Check if the right mouse button was clicked
            DropObjectAtMousePosition(colony);
        }   transform.position = Input.mousePosition;
    }

    void DropObjectAtMousePosition(GameObject gameObject)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) // Check if the ray hits something
        {
            Instantiate(gameObject, hit.point, Quaternion.identity); // Instantiate the object at the hit position
        }
    }
}
