using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AntVision
{
    public void ClearVision(ref List<Vector3> detectbleObjects){
        detectbleObjects.Clear();
    }
    public void ScanEnvironment(ref List<Vector3> detectableObjects,int rayCount, float viewRadius, float viewAngle, Transform transform){

        float angleIncrement = viewAngle / rayCount;
        float startAngle = 0;

        for(int i = 0; i <= rayCount; i++){
            float angle = startAngle + angleIncrement * i;
            Vector3 direction = Quaternion.Euler(0,angle,0) * transform.forward;
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, viewRadius))
                detectableObjects.Add(hit.collider.transform.position);
        }
    }
}
