using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryController
{
    private AntData antData;

    MemoryController(AntData antData){
        this.antData = antData;
    }
    public void UpdateMemory(Memory memory, Vector3 currentPosition){
        antData.memoryPositions.Push(currentPosition);
    }
    public void ReturnToLastPosition(Memory memory){}
    
}
