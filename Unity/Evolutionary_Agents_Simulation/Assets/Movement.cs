using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveStrength = 5f; // Adjust the force strength
    public float changeInterval = 2f; // Time in seconds between direction changes

    private Rigidbody rb;
    private float nextChangeTime = 0f;
    private Vector3 currentDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
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
        currentDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        nextChangeTime = Time.time + changeInterval;
    }

    void MoveObject()
    {
        // Apply a force to the Rigidbody in the chosen direction
        rb.AddForce(currentDirection * moveStrength);
    }
}
