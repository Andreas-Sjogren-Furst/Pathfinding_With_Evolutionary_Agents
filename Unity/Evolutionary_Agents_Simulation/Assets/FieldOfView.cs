using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float viewRadius;
    [Range(0, 360)] public float viewAngle;
    public LayerMask targetMask; // Specify which layer the objects are on that you want to detect
    public LayerMask obstacleMask; // Layer mask for obstacles

    public List<GameObject> leftAreaTargets = new List<GameObject>();
    public List<GameObject> middleAreaTargets = new List<GameObject>();
    public List<GameObject> rightAreaTargets = new List<GameObject>();

    public Queue<List<GameObject>> targets = new Queue<List<GameObject>>();
    void Start()
    {
        StartCoroutine("FindTargetsWithDelay", 0.2f);
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        leftAreaTargets.Clear();
        middleAreaTargets.Clear();
        rightAreaTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            GameObject targetGameObject = targetsInViewRadius[i].gameObject;
            Vector3 dirToTarget = (targetGameObject.transform.position - transform.position).normalized;

            // Calculate the angle between the agent's forward direction and the direction to the target
            float angleToTarget = Vector3.Angle(transform.forward, dirToTarget);

            if (angleToTarget < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, targetGameObject.transform.position);

                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    // Use cross product to determine if the target is to the left or right of the agent
                    Vector3 crossProduct = Vector3.Cross(transform.forward, dirToTarget);
                    float dotProduct = Vector3.Dot(crossProduct, transform.up);

                    // Classify targets based on the angle and direction relative to the agent
                    if (angleToTarget < viewAngle / 3)
                    {
                        // Middle section
                        middleAreaTargets.Add(targetGameObject);
                    }
                    else
                    {
                        if (dotProduct < 0)
                        {
                            // Right section
                            rightAreaTargets.Add(targetGameObject);
                        }
                        else
                        {
                            // Left section
                            leftAreaTargets.Add(targetGameObject);
                        }
                    }
                }
            }
        }
    }


    // Helper method to convert angle in degrees to a direction vector
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);
    }
}


