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
    private bool isChangingState;
    // Start is called before the first frame update
    void Start()
    {
        isChangingState = false;
        sigmoid = new Sigmoid();
        movement = GetComponent<Movement>();
        currentState = AntState.Exploring;
    }
 

    IEnumerator ChangeStateWithDelay(AntState newState, float delay)
    {
        isChangingState = true;
        yield return new WaitForSeconds(delay);
        currentState = newState;
        isChangingState = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(isChangingState) return;
        float randNumber = Random.Range(0f,1f);
        float sigmoidNumber = sigmoid.CalculateProbability(movement.pheromoneConcentration);
        if(randNumber <= sigmoidNumber){
            StartCoroutine(ChangeStateWithDelay(AntState.FollowingPheromones, 2f));
        } else StartCoroutine(ChangeStateWithDelay(AntState.Exploring, 0f));
    }

}
