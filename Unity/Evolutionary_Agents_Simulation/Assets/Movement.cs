using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public GameObject Pheromone;
    public bool Wandering;
    public float directionChangeInterval; // Time in seconds between direction changes
    public float movementSpeed;
    private float nextChangeTime = 0f;
    private Vector3 currentDirection;

    void Start()
    {   
        ChangeDirection();
    }

    void Update()
    {
        // Check if it's time to change the movement direction
        if (Time.time >= nextChangeTime)
        {
            ChangeDirection();
            Instantiate(Pheromone, transform.position, Quaternion.identity);
        }

        // Apply the movement
        MoveObject();
    }

    void ChangeDirection()
    {

        // Generate a random direction by creating a random vector
        currentDirection += new Vector3(0, Random.Range(-45f, 46f), 0);
        if(Random.Range(0,101) < 10){
            currentDirection = new Vector3(0,Random.Range(0f,360f),0);
        }
        transform.rotation = Quaternion.Euler(currentDirection);
        nextChangeTime = Time.time + directionChangeInterval;
    }

    void MoveObject()
    {
        transform.position += transform.forward * (Time.deltaTime * movementSpeed);
    
    }
}
