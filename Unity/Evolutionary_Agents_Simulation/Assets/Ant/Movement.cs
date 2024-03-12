using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UIElements;

public class Movement : MonoBehaviour
{
    AntData antData;
    void Start()
    {   
        antData = GetComponent<AntData>();
    }

    void Update()
    {
        Transform currentTransform = antData.transform;
        Vector3 position = MovementController.RandomTargetDirection(ref currentTransform,1);
        MovementController.MoveForward(antData.moveSpeed, ref currentTransform, antData.stepSize);
    }
}
