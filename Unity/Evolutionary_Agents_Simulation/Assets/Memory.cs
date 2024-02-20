using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Memory : MonoBehaviour
{
    public Stack<Vector3> positions;
    private State state;
    public float updateMemory;
    // Start is called before the first frame update
    void Start()
    {
        state = gameObject.GetComponent<State>();
        positions = new();
        StartCoroutine(UpdateMemoryWithDelay(updateMemory));
    }

    // Update is called once per frame
    void Update()
    {   

    }

    IEnumerator UpdateMemoryWithDelay(float updateMemory)
    {
        yield return new WaitForSeconds(updateMemory);
        positions.Push(transform.position);
    }

}
