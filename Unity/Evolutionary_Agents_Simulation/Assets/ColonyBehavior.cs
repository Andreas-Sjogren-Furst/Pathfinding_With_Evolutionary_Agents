using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColonyBehavior : MonoBehaviour
{
    public GameObject Ant;
    public int amountOfAnts;
    public int resources;
    private int counter;
    
    // Start is called before the first frame update
    void Start()
    {
        resources = 0;
        counter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(counter < amountOfAnts){
            Instantiate(Ant, transform.position, Quaternion.identity);
            counter++;
        }
    }
    void OnTriggerEnter(Collider collider)
    {
    // Check if the object entering the trigger is an agent
    if (collider.CompareTag("Ant"))
    {
        // Access the Agent script to check for resources
        if (collider.gameObject.GetComponent<Movement>().hasFood)
        {
            collider.gameObject.GetComponent<Movement>().hasFood = false;
            resources++;
        }
    }
}
}
