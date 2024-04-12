using UnityEngine;

public abstract class Distrubution
{
   
    public static int Uniform(float maxValue, float[] fieldOfViewSegments){
        // Generate a random number between 0 (inclusive) and totalarea (exclusive)
        float randomNumber = Random.Range(0, maxValue);
        // Determine which segment the randomNumber falls into
        for (int i = 0; i < fieldOfViewSegments.Length; i++)
        {
            if (i == 0)
            {
                if (randomNumber <= fieldOfViewSegments[i])
                    return i;
            }
            else
            {
                if (randomNumber >= fieldOfViewSegments[i - 1] && randomNumber <= fieldOfViewSegments[i])
                {
                    return i;
                }
            }
        } throw new System.ArgumentException("number is out of range");
    }

    public static float Sigmoid(float x, float steepness = 4f, float shift = 1f){
        return 1f/(1f + Mathf.Exp(steepness * (-x+shift)));
    }

}
