using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView
{
    private AntData antData;

    public FieldOfView(AntData antData){
        this.antData = antData;
    }
   
    void FindVisibleTargets(Transform currentTransform, float viewRadius, float viewAngle)
    {
        LayerMask obstacleMask = LayerMask.GetMask("Wall");
        LayerMask combinedMask = LayerMask.GetMask("Pheromone") | LayerMask.GetMask("Checkpoint");
        antData.leftAreaTargets.Clear();
        antData.middleAreaTargets.Clear();
        antData.rightAreaTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(currentTransform.position, viewRadius, combinedMask);
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            GameObject targetGameObject = targetsInViewRadius[i].gameObject;
            Vector3 dirToTarget = (targetGameObject.transform.position - currentTransform.position).normalized;

            // Calculate the angle between the agent's forward direction and the direction to the target
            float angleToTarget = Vector3.Angle(currentTransform.forward, dirToTarget);

            if (angleToTarget < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(currentTransform.position, targetGameObject.transform.position);

                if (!Physics.Raycast(currentTransform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    if(targetGameObject.CompareTag("Checkpoint")){
                        antData.locatedFood = true;
                        antData.foodSourcePosition = targetGameObject.transform.position;
                    }
                    else if (targetGameObject.CompareTag("Pheromone")){
                         // Use cross product to determine if the target is to the left or right of the agent
                            Vector3 crossProduct = Vector3.Cross(currentTransform.forward, dirToTarget);
                            float dotProduct = Vector3.Dot(crossProduct, currentTransform.up);

                            // Classify targets based on the angle and direction relative to the agent
                            if (angleToTarget < viewAngle / 3) {
                                antData.middleAreaTargets.Add(targetGameObject);
                                continue;
                            } 
                            if(dotProduct < 0)
                                antData.rightAreaTargets.Add(targetGameObject);
                            else
                                antData.leftAreaTargets.Add(targetGameObject);
                    }
                }
            }
        }
    }
}


