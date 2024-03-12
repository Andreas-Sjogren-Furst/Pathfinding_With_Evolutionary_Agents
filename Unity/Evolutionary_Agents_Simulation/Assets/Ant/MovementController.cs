using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementController
{
    public static void MoveForward(float moveSpeed, ref Transform currentTransform, int movementLength){
        Vector3 newPosition = currentTransform.position + (movementLength * currentTransform.forward); 
        currentTransform.position = Vector3.MoveTowards(currentTransform.position, newPosition, moveSpeed * Time.deltaTime);
    }
    public static void RotateTowards(ref Transform currentTransform, Vector3 targetPosition){
        Vector3 directionToTarget = targetPosition - currentTransform.position;
        currentTransform.rotation = Quaternion.LookRotation(directionToTarget);
    }
    public static void UpdateSteps(ref int totalSteps){
        totalSteps++;
    }
    public static Vector3 RandomTargetDirection(ref Transform currentTransform, int movementLength){
        Vector3[] directions = new Vector3[]{
            Vector3.up,
            Vector3.right,
            Vector3.down,
            Vector3.left
        };
        Vector3 randomDirection = movementLength * directions[Random.Range(0,directions.Length)];
        Vector3 newPosition = currentTransform.position + randomDirection;
        return newPosition;
    }
}
