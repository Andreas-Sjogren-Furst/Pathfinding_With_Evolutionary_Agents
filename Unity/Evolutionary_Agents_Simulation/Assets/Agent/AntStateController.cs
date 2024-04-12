using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AntStateController : MonoBehaviour
{
    
    public void ChangeState(ref State.AntState currentState, State.AntState newState){
        currentState = newState;
    }
    /*public void ExecuteBehavior(State.AntState currentState){
        switch (currentState)
        {
            case State.AntState.Exploring:
                RandomDirection();
                break;
            case State.AntState.FollowingPheromones:
                ACO();
                break;
            case State.AntState.GetFood:
                RotateTowardsFoodSource();
                break;
            case State.AntState.ReturningToColony:
                ReturnHome();
                break;
        }
    }*/
}
