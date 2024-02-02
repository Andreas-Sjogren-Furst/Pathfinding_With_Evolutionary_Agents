using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 5.0f; // Speed of the ball movement

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal"); // Get horizontal input (left/right arrow keys)
        float verticalInput = Input.GetAxis("Vertical"); // Get vertical input (up/down arrow keys)

        // Calculate new position
        Vector3 newPosition = transform.position + new Vector3(horizontalInput, 0, verticalInput) * speed * Time.deltaTime;

        // Update the ball's position
        transform.position = newPosition;
    }
}
