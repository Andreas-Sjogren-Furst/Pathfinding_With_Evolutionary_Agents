using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

public class ObjectDropper : MonoBehaviour
{
    public GameObject pheromone; // Assign your prefab in the Inspector
    public GameObject colony;
    public GameObject checkpoint;
    public List<Vector3> colonyPositions;

    public List<GameObject> colonies;

    public List<GameObject> checkpoints;
    // Assign your prefab in the Inspector
    private Camera cam;
    private Ray ray;
    void Start()
    {
        colonyPositions = new List<Vector3>();
        cam = GetComponent<Camera>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropObjectAtMousePosition(pheromone);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            DropObjectAtMousePosition(colony);

        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            DropObjectAtMousePosition(checkpoint);
            // add checkpoint reference. 

        }
    }

    void DropObjectAtMousePosition(GameObject gameObject)
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit)) // Check if the ray hits something
        {
            colonyPositions.Add(hit.point);
            GameObject instantiatedObject = Instantiate(gameObject, hit.point + new Vector3(0, 0.5f, 0), Quaternion.identity); // Instantiate the object at the hit position

            if (gameObject == colony)
            {
                colonies.Add(instantiatedObject); // Add the instantiated colony to the list
            }
            else if (gameObject == checkpoint)
            {
                checkpoints.Add(instantiatedObject); // Add the instantiated colony to the list
            }
        }
    }

    internal List<GameObject> GetColonies()
    {

        return colonies;
    }

    internal List<GameObject> GetCheckpoints()
    {
        return checkpoints;

    }
}
