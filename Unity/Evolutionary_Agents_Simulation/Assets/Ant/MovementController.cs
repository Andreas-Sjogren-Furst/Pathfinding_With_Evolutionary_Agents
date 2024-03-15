using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController
{
    public void MoveForward(float moveSpeed, ref Transform currentTransform){
        currentTransform.position += currentTransform.forward * (Time.deltaTime * moveSpeed);
    }
    public void RotateTowards(ref Transform currentTransform, Vector3 targetPosition){
        Vector3 directionToTarget = targetPosition - currentTransform.position;
        currentTransform.rotation = Quaternion.LookRotation(directionToTarget);
    }
    public void UpdateSteps(ref int totalSteps){
        totalSteps++;
    }
}
