using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryController
{
    public void UpdateMemory(ref Stack<Vector3> memory, Vector3 currentPosition){
        memory.Push(currentPosition);
    }
    public void ReturnToLastPosition(Memory memory){}   
}
