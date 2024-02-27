using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Memory : MonoBehaviour
{
    public Stack<Vector3> positions;
    private State state;
    private Movement movement;
    public float distanceThreshold = 2f;
    
    void Start()
    {
        movement = gameObject.GetComponent<Movement>();
        state = gameObject.GetComponent<State>();
        positions = new();
    }

    void Update(){
        if(state.currentState != State.AntState.ReturningToColony){
            if(movement.GetTotalDistance() > distanceThreshold){
                positions.Push(transform.position);
                movement.ResetTotalDistance();
            }
        } 
    }
}
