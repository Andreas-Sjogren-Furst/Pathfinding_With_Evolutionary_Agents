using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Memory : MonoBehaviour
{
    public Stack<Vector3> positions;
    private State state;
    public float updateMemory;
    private Coroutine updateMemoryCoroutine = null; // Reference to the coroutine
    public bool shouldUpdateMemory = true; // Control variable

    void Start()
    {
        state = gameObject.GetComponent<State>();
        positions = new Stack<Vector3>();
    }

    void Update()
    {
        // Start the coroutine if shouldUpdateMemory is true and the coroutine is not already running
        if (state.currentState != State.AntState.ReturningToColony && updateMemoryCoroutine == null)
        {
            updateMemoryCoroutine = StartCoroutine(UpdateMemoryWithDelay(updateMemory));
        }
        // Stop the coroutine if shouldUpdateMemory is false and the coroutine is running
        else if (state.currentState == State.AntState.ReturningToColony && updateMemoryCoroutine != null)
        {
            StopCoroutine(updateMemoryCoroutine);
            updateMemoryCoroutine = null; // Reset the coroutine reference
        }
    }

    IEnumerator UpdateMemoryWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            positions.Push(transform.position);
            Debug.Log("has pushed position");
        }
    }
}
