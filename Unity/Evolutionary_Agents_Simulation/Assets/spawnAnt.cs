using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnObject : MonoBehaviour
{
    public GameObject antPrefab;
    public int amountOfAnts;
    private int antCounter;
    
    // Start is called before the first frame update
    void Start()
    {
        antCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(antCounter < amountOfAnts){
            Instantiate(antPrefab, transform.position, Quaternion.identity);
            antCounter += 1;
        }
    }
}
