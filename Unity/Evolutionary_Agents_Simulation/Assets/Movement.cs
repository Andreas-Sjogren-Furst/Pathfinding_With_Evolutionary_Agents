using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UIElements;

public class Movement : MonoBehaviour
{
    public float pheromoneConcentration;
    public float spawnInterval;
    public float directionChangeInterval; // Time in seconds between direction changes
    public GameObject Pheromone;
    public bool hasFood;
    private float[] pheromoneDistrubution;
    private FieldOfView fieldOfView;
    private Memory memory;
    private State state;
    public float movementSpeed;
    private float nextChangeTime;
    private bool atTargetPosition;
    private Vector3 targetPosition;
    public float closeEnoughThreshold = 0.1f;

    private int totalSteps;



    void Start()
    {
        atTargetPosition = false;
        hasFood = false;
        pheromoneDistrubution = new float[3];
        nextChangeTime = 0f;
        state = GetComponent<State>();
        fieldOfView = GetComponent<FieldOfView>();
        memory = GetComponent<Memory>();
        StartCoroutine(SpawnObjectRoutine());
        transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
    }

    void Update()
    {
        // Check if it's time to change the movement direction
        if (Time.time >= nextChangeTime)
        {
            SensePheromones();
            CalculatePheromoneConcentration();
            ChangeDirection();
            UpdateTime();
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
            Instantiate(Pheromone, transform.position - transform.forward * 1f, Quaternion.Euler(90f, 0f, 0f)); // Spawn the object at the agent's position
        }
    }

    void UpdateTime()
    {
        nextChangeTime = Time.time + directionChangeInterval;
    }

    void OnCollisionEnter(Collision collision)
    {
        Vector3 contactNormal = collision.GetContact(0).normal;
        transform.rotation = Quaternion.LookRotation(contactNormal);
        nextChangeTime = Time.time + directionChangeInterval;
    }

    void ReturnHome()
    {

        if (atTargetPosition == false)
        {
            targetPosition = memory.positions.Pop();
            atTargetPosition = true;
        }
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        if (distanceToTarget > closeEnoughThreshold)
        {
            Vector3 targetDirection = targetPosition - transform.position;
            transform.rotation = Quaternion.LookRotation(targetDirection, Vector3.up);
            // Consider adding movement towards the target here as well
        }
        else
        {
            if (memory.positions.Count > 0)
            {
                targetPosition = memory.positions.Pop();
            }
        }
    }
    void RandomDirection()
    {
        float randomAngle = Random.Range(-12f, 12f);

        // Apply the angle to the current rotation around the Y axis
        Quaternion rotationChange = Quaternion.Euler(0, randomAngle, 0);

        // Update the current direction based on the new rotation
        Vector3 currentDirection = rotationChange * transform.forward;

        // Apply the updated direction to the object's rotation
        transform.rotation = Quaternion.LookRotation(currentDirection);
    }

    void MoveObject()
    {

        totalSteps++;
        // Debug.Log("Total steps: " + totalSteps);

        transform.position += transform.forward * (Time.deltaTime * movementSpeed);
    }

    void ChangeDirection()
    {
        switch (state.currentState)
        {
            case State.AntState.Exploring:
                RandomDirection();
                break;
            case State.AntState.FollowingPheromones:
                ACO();
                break;
            case State.AntState.GetFood:
                RotateTowardsFoodSource();
                break;
            case State.AntState.ReturningToColony:
                ReturnHome();
                break;
        }
    }

    void SensePheromones()
    {
        pheromoneDistrubution[0] = CalculativeCumulativeArea(fieldOfView.leftAreaTargets);
        pheromoneDistrubution[1] = CalculativeCumulativeArea(fieldOfView.middleAreaTargets);
        pheromoneDistrubution[2] = CalculativeCumulativeArea(fieldOfView.rightAreaTargets);
    }
    void CalculatePheromoneConcentration()
    {
        pheromoneConcentration = pheromoneDistrubution[0] + pheromoneDistrubution[1] + pheromoneDistrubution[2];
    }
    void ACO()
    {
        Quaternion rotationChange;
        Vector3 currentDirection;
        float viewAngle = fieldOfView.viewAngle;
        if (pheromoneConcentration == 0f) return;
        for (int i = 1; i < pheromoneDistrubution.Length; i++)
        {
            pheromoneDistrubution[i] += pheromoneDistrubution[i - 1];
        }
        int direction = getArea(pheromoneConcentration, pheromoneDistrubution);
        if (direction == 1) return;
        rotationChange = direction == 0 ? Quaternion.Euler(0, viewAngle / 12, 0) : Quaternion.Euler(0, -viewAngle / 12, 0);
        currentDirection = rotationChange * transform.forward;
        transform.rotation = Quaternion.LookRotation(currentDirection);
    }

    void RotateTowardsFoodSource()
    {
        transform.LookAt(fieldOfView.foodSource.transform.position);
    }

    int getArea(float totalArea, float[] segments)
    {
        // Generate a random number between 0 (inclusive) and totalarea (exclusive)
        float randomNumber = Random.Range(0, totalArea);
        // Determine which segment the randomNumber falls into
        for (int i = 0; i < segments.Length; i++)
        {
            if (i == 0)
            {
                if (randomNumber <= segments[i])
                    return i;
            }
            else
            {
                if (randomNumber >= segments[i - 1] && randomNumber <= segments[i])
                {
                    return i;
                }
            }
        }
        return -1;
    }

    float CalculativeCumulativeArea(List<GameObject> pheromones)
    {
        float value = 0f;
        if (pheromones.Count == 0) return value;
        foreach (GameObject pheromone in pheromones)
        {
            if (pheromone != null)
                value += pheromone.GetComponent<pheromoneBehavior>().alpha;
        }
        return value;
    }

    public int GetTotalSteps()
    {
        return totalSteps;
    }





}
