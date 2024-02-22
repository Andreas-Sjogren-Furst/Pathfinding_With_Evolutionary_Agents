using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Memory : MonoBehaviour
{
    public List<Vector3> positions;
    private State state;
    public float updateMemory;
    private Coroutine updateMemoryCoroutine = null; // Reference to the coroutine
    public bool shouldUpdateMemory = true; // Control variable

    void Start()
    {
        state = gameObject.GetComponent<State>();
        positions = new();
    }

    public void UpdateMemory(){
        if(state.currentState == State.AntState.Exploring || state.currentState == State.AntState.FollowingPheromones){
            positions.Add(transform.position);
        } else{
            positions.Clear();
        }
    }
}
