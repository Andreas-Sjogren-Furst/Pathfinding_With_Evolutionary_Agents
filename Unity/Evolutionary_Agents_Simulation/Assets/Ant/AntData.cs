using System.Collections.Generic;
using UnityEngine;

public abstract class AntData : ScriptableObject
{
    // Physical and Movement Data
    public readonly float stepSize;
    public int totalSteps;
    public AntDirection.Direction currentDirection;

    // Field Of View Data
    [Range(1,10)] public float viewRadius;
    public readonly float viewAngle = 360f;
    public List<Vector3> detectableObjects = new();

    // Memory Data
    public Stack<Vector3> memoryPositions = new();

    //State data
    public State.AntState currentState;
    
}
