using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    [SerializeField] private MapModel mapModel;


    private MapController mapController;



    void Awake()
    {

        mapController = new MapController(mapModel, tileConfig, wallConfig, checkPointConfig, spawnPointConfig);


    }

    // Start is called before the first frame update
    void Start()
    {
        mapController.InitMap();

    }


    // Update is called once per frame
    void Update()
    {

    }
}
