using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public GameObject Pheromone;
    public float spawnInterval;
    public bool Wandering;
    public float directionChangeInterval; // Time in seconds between direction changes
    public float movementSpeed;
    private float nextChangeTime = 0f;
    private Vector3 currentDirection;
    private int counter, steps;
    private float angle;

    void Start()
    {   
        counter = 0;
        angle = Random.Range(-20f, 20f);
        steps = Random.Range(0,6);
        StartCoroutine(SpawnObjectRoutine());
        ChangeDirection();
    }

    void Update()
    {
        // Check if it's time to change the movement direction
        if (Time.time >= nextChangeTime)
        {
            ChangeDirection();
        }

        // Apply the movement
        MoveObject();
    }

    private IEnumerator SpawnObjectRoutine()
    {
        // Infinite loop to spawn objects at intervals
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval); // Wait for the specified interval
            Instantiate(Pheromone, transform.position, Quaternion.identity); // Spawn the object at the agent's position
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        // Get the normal of the first contact point
        Vector3 contactNormal = collision.GetContact(0).normal; 
        // Rotate the agent to align with the normal vector of the wall
        // This makes the agent's forward direction parallel to the wall's normal, effectively "facing away" from the wall
        transform.rotation = Quaternion.LookRotation(contactNormal);
        nextChangeTime = Time.time + directionChangeInterval;
        // If you want the agent to "slide" along the wall instead of facing away, you might rotate it to be perpendicular
        // This example directly faces the wall's normal. Adjust as needed for your specific behavior (e.g., Quaternion.FromToRotation(Vector3.forward, -contactNormal) for facing towards, with adjustments for sliding behavior)
    }

    void ChangeDirection()
    {   
        if (counter == steps){
            angle = Random.Range(-12f, 12f);
            steps = Random.Range(0,6);
            counter = 0;
        } else counter++;
        
        // Apply the angle to the current rotation around the Y axis
        Quaternion rotationChange = Quaternion.Euler(0, angle, 0);

        // Update the current direction based on the new rotation
        currentDirection = rotationChange * transform.forward;

        // Apply the updated direction to the object's rotation
        transform.rotation = Quaternion.LookRotation(currentDirection);

        // Update the time for the next direction change
        nextChangeTime = Time.time + directionChangeInterval;
    }

    void MoveObject()
    {
        transform.position += transform.forward * (Time.deltaTime * movementSpeed);
    
    }
}
