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
        GetFood,
        ReturningToColony
    };

    public AntState currentState;
    private FieldOfView fieldOfViewScript;
    // Start is called before the first frame update
    void Start()
    {
        fieldOfViewScript = gameObject.GetComponent<FieldOfView>();
        sigmoid = new Sigmoid();
        movement = GetComponent<Movement>();
        currentState = AntState.Exploring;
    }

    // Update is called once per frame
    void Update()
    {
        float randNumber = Random.Range(0f,1f);
        float sigmoidNumber = sigmoid.CalculateProbability(movement.pheromoneConcentration);
        //TODO make distance logic
        if(fieldOfViewScript.locatedFood && !movement.hasFood){
            currentState = AntState.GetFood;
        }
        else if(movement.hasFood){
            currentState = AntState.ReturningToColony;
        } 
        else if(randNumber <= sigmoidNumber){
            currentState = AntState.FollowingPheromones;
        } else currentState = AntState.Exploring;
    }

}
