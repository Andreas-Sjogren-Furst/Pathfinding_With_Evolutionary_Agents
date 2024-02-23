using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColonyBehavior : MonoBehaviour
{

    public GameObject Ant;
    public int amountOfAnts;
    public int resources;
    private int counter;

    public List<GameObject> ants; // List to store references to all ants

    // Start is called before the first frame update
    void Start()
    {
        resources = 0;
        counter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (counter < amountOfAnts)
        {


            GameObject newAnt = Instantiate(Ant, transform.position, Quaternion.identity);
            Debug.Log("add new ant");
            ants.Add(newAnt);
            counter++;
        }
    }
    void OnTriggerEnter(Collider collider)
    {
        GameObject gameObject = collider.gameObject;
        // Check if the object entering the trigger is an agent
        if (gameObject.CompareTag("Ant"))
        {
            Debug.Log("collided");
            // Access the Agent script to check for resources
            if (gameObject.GetComponent<Movement>().hasFood)
            {
                Debug.Log("Ruturned food");
                gameObject.GetComponent<Movement>().hasFood = false;
                gameObject.GetComponent<Memory>().positions.Clear();
                resources++;
            }
        }
    }

    internal List<GameObject> GetAnts()
    {
        return ants;
    }
    internal int GetResources()
    {
        return resources;
    }

}
