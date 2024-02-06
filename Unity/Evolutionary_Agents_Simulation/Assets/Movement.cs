using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveStrength = 5f; // Adjust the force strength
    public float changeInterval = 2f; // Time in seconds between direction changes
    public float speed = 2f;
  
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
        }

        // Apply the movement force
        MoveObject();
    }

    void ChangeDirection()
    {
        // Generate a random direction by creating a random vector
        currentDirection = new Vector3(0, Random.Range(0f, 360f), 0);
        transform.rotation = Quaternion.Euler(currentDirection);
        nextChangeTime = Time.time + changeInterval;
    }

    void MoveObject()
    {
        transform.position += transform.right * (Time.deltaTime * speed);
    
    }
}
