using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private MapModel mapModel;
    [SerializeField] private TileConfig tileConfig;
    [SerializeField] private WallConfig wallConfig;
    [SerializeField] private CheckPointConfig checkPointConfig;
    [SerializeField] private SpawnPointConfig spawnPointConfig;


    private MapController mapController;
    private PathFinder _pathFinder;
    private GraphModel _graphModel;
    private NodeManager _nodeManager;

    private HPAStar HPAStar;

    private HPAGraphView hPAGraphView;

    void Awake()
    {

        mapController = new MapController(mapModel, tileConfig, wallConfig, checkPointConfig, spawnPointConfig);
        mapController.InitMap();

        _graphModel = new GraphModel(mapModel.Map);
        _pathFinder = new PathFinder();
        if (mapModel.Map == null)
        {
            Debug.LogError("Map is null");
            return;
        }
        IEdgeManager edgeManager = new EdgeManager(_pathFinder);
        _nodeManager = new NodeManager(_graphModel, edgeManager);
        IEntranceManager entranceManager = new EntranceManager(_graphModel, _nodeManager);
        IClusterManager clusterManager = new ClusterManager(_graphModel, _nodeManager, edgeManager, entranceManager);
        HPAStar = new HPAStar(_graphModel, clusterManager, _nodeManager, entranceManager, edgeManager, _pathFinder);



    }

    // Start is called before the first frame update
    void Start()
    {

        // Create instances of the necessary classes

        HPAStar.Preprocessing(1);

        if (_graphModel == null)
        {
            Debug.LogError("Graph Model is null");
            return;
        }


        hPAGraphView.DrawGraph(_graphModel);




    }


    // Update is called once per frame
    void Update()
    {

    }
}
