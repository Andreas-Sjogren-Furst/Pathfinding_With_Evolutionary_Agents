using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private MapModel mapModel;
    [SerializeField] private TileConfig tileConfig;
    [SerializeField] private WallConfig wallConfig;
    [SerializeField] private CheckPointConfig checkPointConfig;
    [SerializeField] private SpawnPointConfig spawnPointConfig;


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
