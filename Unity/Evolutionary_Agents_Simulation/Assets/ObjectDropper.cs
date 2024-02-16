using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

public class ObjectDropper : MonoBehaviour
{
    public GameObject pheromone; // Assign your prefab in the Inspector
    public GameObject colony;
    List<Vector3> colonyPositions;
     // Assign your prefab in the Inspector
    private Camera cam;
    private Ray ray;
    void Start(){
        colonyPositions = new List<Vector3>();
        cam = GetComponent<Camera>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Check if the left mouse button was clicked
        {   
            DropObjectAtMousePosition(pheromone);
        } else if(Input.GetMouseButtonDown(1)){ // Check if the right mouse button was clicked
            DropObjectAtMousePosition(colony);
        }
    }

    void DropObjectAtMousePosition(GameObject gameObject)
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit)) // Check if the ray hits something
        {
            colonyPositions.Add(hit.point);
            Instantiate(gameObject, hit.point + new Vector3(0,0.5f,0), Quaternion.identity); // Instantiate the object at the hit position
        }
    }
}
