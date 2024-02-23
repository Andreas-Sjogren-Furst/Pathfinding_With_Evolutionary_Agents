using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColonyBehavior : MonoBehaviour
{

    public GameObject Ant;
    public int amountOfAnts;
    public int resources;
    private int counter;

    public List<GameObject> ants; // List to store references to all ants

    // Start is called before the first frame update
    void Start()
    {
        resources = 0;
        counter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (counter < amountOfAnts)
        {
            GameObject newAnt = Instantiate(Ant, transform.position, Quaternion.identity);
            ants.Add(newAnt);
            counter++;
        }
    }
}
