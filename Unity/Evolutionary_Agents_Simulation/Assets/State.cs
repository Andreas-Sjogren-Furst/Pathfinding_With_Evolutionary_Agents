using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State : MonoBehaviour
{
    Movement movement;
    Sigmoid sigmoid;
    public enum AntState {
        Exploring,
        FollowingPheromones,
        ReturningToColony
    };

    public AntState currentState;
    // Start is called before the first frame update
    void Start()
    {
        sigmoid = new Sigmoid();
        movement = GetComponent<Movement>();
        currentState = AntState.Exploring;
    }

    // Update is called once per frame
    void Update()
    {
        float randNumber = Random.Range(0f,1f);
        float sigmoidNumber = sigmoid.CalculateProbability(movement.pheromoneConcentration);
        Debug.Log(sigmoid.CalculateProbability(3));
        if(randNumber <= sigmoidNumber){
            currentState = AntState.FollowingPheromones;
        } else currentState = AntState.Exploring;
    }

}
