using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnScript : MonoBehaviour
{
    public GameObject antPrefab;
    public int amountOfAnts = 10;
    public int xPosition;
    public int zPosition;
    private Vector3 spawnVectorPosition;
    private int antCounter = 0;

    void Start()
    {
        spawnVectorPosition = new Vector3(xPosition,1,zPosition);
    }

    // Update is called once per frame
    void Update()
    {   
        if(antCounter < amountOfAnts){
            
            Instantiate(antPrefab, spawnVectorPosition, Quaternion.identity);
            antCounter += 1;
        }
    }
}
