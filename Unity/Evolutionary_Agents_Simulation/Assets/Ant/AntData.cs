using System.Collections.Generic;
using UnityEngine;

public class AntData : MonoBehaviour
{
    // Physical and Movement Data
    public readonly int stepSize = 1;
    public readonly float moveSpeed = 2f;
    public int totalSteps;
    public AntDirection.Direction currentDirection;

    // Field Of View Data
    [Range(1,10)] public float viewRadius;
    public readonly float viewAngle = 360f;
    public List<Vector3> detectableObjects;
    
    // Memory Data
    public Stack<Vector3> memoryPositions;

    //State data
    public State.AntState currentState;
    
}
