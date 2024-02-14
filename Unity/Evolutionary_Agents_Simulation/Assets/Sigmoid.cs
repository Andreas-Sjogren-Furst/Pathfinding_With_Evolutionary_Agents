using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sigmoid : MonoBehaviour
{   
    public float steepness = 1f;
    public float xShift = 3f;
    
    public float CalculateProbability(float x){
        return 1f/(1f + Mathf.Exp(steepness * (-x+xShift)));
    }
}
