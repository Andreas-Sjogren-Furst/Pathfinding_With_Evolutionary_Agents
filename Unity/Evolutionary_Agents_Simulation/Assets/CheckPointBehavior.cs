using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointBehavior : MonoBehaviour
{
    private State state;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collider)
    {
    // Check if the object entering the trigger is an agent
    if (collider.CompareTag("Ant"))
    {
        Debug.Log("has entered the source");
        collider.gameObject.GetComponent<Movement>().hasFood = true;
        collider.gameObject.GetComponent<FieldOfView>().locatedFood = false;
    }
    }
}
