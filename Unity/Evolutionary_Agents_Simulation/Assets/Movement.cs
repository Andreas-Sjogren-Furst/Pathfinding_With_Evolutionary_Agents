using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public GameObject Pheromone;
    FieldOfView fieldOfView;
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
        fieldOfView = GetComponent<FieldOfView>();
        counter = 0;
        angle = Random.Range(-20f, 20f);
        steps = Random.Range(0,6);
        StartCoroutine(SpawnObjectRoutine());
        RandomDirection();
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
            Instantiate(Pheromone, transform.position - transform.forward * 1f, Quaternion.identity); // Spawn the object at the agent's position
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

    Quaternion RandomDirection()
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
        Quaternion rotation = Quaternion.LookRotation(currentDirection);
        return rotation;

    }

    void MoveObject()
    {
        transform.position += transform.forward * (Time.deltaTime * movementSpeed);
    }

    void ChangeDirection() {
        nextChangeTime = Time.time + directionChangeInterval;
        if(Random.Range(0,101) < 0){
            transform.rotation = ACO(fieldOfView.targets);
        } else transform.rotation = RandomDirection();
    }

    Quaternion ACO(Queue<List<GameObject>> targets){
        float[] pheromoneDistrubution = new float[3];
        float viewAngle = GetComponent<FieldOfView>().viewAngle;
        float totalArea = 0f;
        for(int i = 0; i < 3; i++){
            pheromoneDistrubution[i] = CalculativeCumulativeArea(targets.Dequeue());
            totalArea += pheromoneDistrubution[i];
            if(i != 0)
                pheromoneDistrubution[i] += pheromoneDistrubution[i-1];
        } int direction = getArea(totalArea, pheromoneDistrubution);
        Quaternion temp = Quaternion.Euler(0,-viewAngle/2,0);
        Quaternion actualPosition = Quaternion.Euler(0,viewAngle/2 * direction,0);
        Vector3 tempDirection = temp * transform.forward;
        Quaternion okosdsd = Quaternion.LookRotation(tempDirection);
        return okosdsd;
    }

    int getArea(float totalArea, float[] segments){
        
        // Generate a random number between 0 (inclusive) and 101 (exclusive)
        float randomNumber = Random.Range(0, totalArea);

        // Determine which segment the randomNumber falls into
        for (int i = 0; i < segments.Length; i++)
        {
            if(i == 0 && randomNumber <= segments[i]){
                return i;
            } else {
                if(randomNumber >= segments[i - 1] && randomNumber <= segments[i]){
                    return i;
                }
            }
        } return -1;
    }

    float CalculativeCumulativeArea(List<GameObject> targets){
        float value = 0f;
        foreach(GameObject pheromone in targets){
                 value += pheromone.GetComponent<pheromoneBehavior>().alpha; 
            } return value;
    }

}
