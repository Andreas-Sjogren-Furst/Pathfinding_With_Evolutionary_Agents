using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController
{
    private AntData antData;
    public MovementController(AntData antData){
        this.antData = antData;
    }
    public Vector3 MoveForward(float moveSpeed, Transform currentTransform, ref int currentSteps){
        currentSteps++;
        Vector3 newPosition = currentTransform.position + currentTransform.forward * (Time.deltaTime * moveSpeed);
        return newPosition;
    }

    public Quaternion RotateRandomly(Transform currentTransform, float rotateAngle){
        float randomAngle = Random.Range(-rotateAngle, rotateAngle);

        // Apply the angle to the current rotation around the Y axis
        Quaternion rotationChange = Quaternion.Euler(0, randomAngle, 0);

        // Update the current direction based on the new rotation
        Vector3 currentDirection = rotationChange * currentTransform.forward;

        // Apply the updated direction to the object's rotation
        Quaternion newRotation = Quaternion.LookRotation(currentDirection);

        return newRotation;

    }
    public Quaternion RotateTowards(Transform currentTransform, Vector3 targetPosition){
        Vector3 directionToTarget = targetPosition - currentTransform.position;
        Quaternion newRotation = Quaternion.LookRotation(directionToTarget);
        return newRotation;
    }

    public Quaternion ACO(Transform currentTransform, float viewAngle, float pheromoneConcentration, float[] pheromoneDistrubution)
    {
        if(pheromoneDistrubution.Length > 3) throw new System.ArgumentException("Length of pheromoneDistrubution was invalid"); 

        pheromoneDistrubution[1] += pheromoneDistrubution[0];
        pheromoneDistrubution[2] += pheromoneDistrubution[1];
        
        int direction = Distrubution.Uniform(pheromoneConcentration, pheromoneDistrubution);
        if (direction == 1) return currentTransform.rotation; // left = 0, forward = 1, right = 2

        Quaternion rotationChange = direction == 0 ? Quaternion.Euler(0, viewAngle / 12, 0) : Quaternion.Euler(0, -viewAngle / 12, 0);
        Vector3 newDirection = rotationChange * currentTransform.forward;
        Quaternion newRotation = Quaternion.LookRotation(newDirection);
        return newRotation;
    }
}
