using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public abstract class AntData : ScriptableObject
{
    // Pheromone Data
    public float pheromoneConcentration;
    [Range(0,45)] public float evaporationConstant;
    [Range(0,20)] public float dropPheromoneInterval;

    // Physical/Movement Data
    [Range(0,20)] public float movementSpeed;
    public int totalSteps;
    public bool hasFood;

    // Field Of View Data
    [Range(0,15)] public float viewRadius;
    [Range(0, 360)] public float viewAngle;
    public List<GameObject> leftAreaTargets = new();
    public List<GameObject> middleAreaTargets = new();
    public List<GameObject> rightAreaTargets = new();
    public bool locatedFood;

    // Memory Data
    public Stack<Vector3> memoryPositions = new();
    
}
