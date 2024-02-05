using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnScript : MonoBehaviour
{
    public GameObject antPrefab;
    public int amountOfAnts = 10;
    private int antCounter = 0;
    // Update is called once per frame
    void Update()
    {   
        if(antCounter < amountOfAnts){
            Instantiate(antPrefab, transform.position, Quaternion.identity);
            antCounter += 1;
        }
    }
}
