using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sigmoid
{   
    public float steepness = 4f;
    public float xShift = 1f;
    
    public float CalculateProbability(float x){
        return 1f/(1f + Mathf.Exp(steepness * (-x+xShift)));
    }
}
