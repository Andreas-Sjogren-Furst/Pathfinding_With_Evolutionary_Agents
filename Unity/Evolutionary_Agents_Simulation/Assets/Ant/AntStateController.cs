using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AntStateController : MonoBehaviour
{
    private AntData antData;
    public AntStateController(AntData antData){
        this.antData = antData;
    }

    public void ChangeState(State.AntState newState){
        antData.currentState = newState;
    }
    public void ExecuteBehavior(State.AntState currentState){
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
    }
    
    public State.AntState GetCurrentState(){
        return antData.currentState;
    }
    
}
