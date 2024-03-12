using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class DrawFieldOfView : MonoBehaviour
{
    AntData antData;
    void Start() {
        antData = GetComponent<AntData>();
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        DrawVisionArea(transform.position, antData.viewRadius);
    }

    void DrawVisionArea(Vector3 currentPosition, float radius)
    {
        int segmentCount = 360; // Increase for smoother circles
        float angle = 0f;

        // Start at 0 degrees and move up by small increments to draw the circle
        for (int i = 0; i <= segmentCount; i++)
        {
            // Calculate the x and z positions of the current point
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            Vector3 point = new Vector3(x, 0, z) + currentPosition;

            // Calculate the x and z positions of the next point
            angle += 360f / segmentCount;
            float nextX = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            float nextZ = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            Vector3 nextPoint = new Vector3(nextX, 0, nextZ) + currentPosition;

            // Draw a line between the current point and the next point
            Gizmos.DrawLine(point, nextPoint);
        }
    }
}
