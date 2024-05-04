using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class PathfindingTest
{



    // A Test behaves as an ordinary method
    [Test]
    public void PathfindingTestSimplePasses()
    {
        // Create instances of the necessary classes

        InitCustomMaps mapModel = new InitCustomMaps(density: 0, numberOfCheckPoints: 0, cellularIterations: 0, checkPointSpacing: 10, erosionLimit: 4, randomSeed: 0);
        int[,] tileMap = CellularAutomata.Create2DMap(mapModel);


        Vector2Int start = new Vector2Int(0, 0);
        Vector2Int end = new Vector2Int(99, 99);



        HPAPath hpaPath = EffiencyInNodeExploration(tileMap, start, end, 1, RefinePath: false);

        Debug.Log("HPA STAR Abstract level 1 : ");

        Debug.Log("Path Length: " + hpaPath.path.Count);
        Debug.Log("Nodes Explored: " + hpaPath.NodesExplored);


        HPAPath hpaPath3 = EffiencyInNodeExploration(tileMap, start, end, 2, RefinePath: false);

        Debug.Log("HPA STAR Abstract level 2 : ");

        Debug.Log("Path Length: " + hpaPath3.path.Count);
        Debug.Log("Nodes Explored: " + hpaPath3.NodesExplored);



        HPAPath hpaPath4 = EffiencyInNodeExploration(tileMap, start, end, 3, RefinePath: false);

        Debug.Log("HPA STAR Abstract level 3 : ");

        Debug.Log("Path Length: " + hpaPath4.path.Count);
        Debug.Log("Nodes Explored: " + hpaPath4.NodesExplored);


        HPAPath hpaPath2 = EffiencyInNodeExploration(tileMap, start, end, 3, RefinePath: true);
        Debug.Log("HPA STAR: ");
        Debug.Log("Path Length: " + hpaPath2.path.Count);
        Debug.Log("Nodes Explored: " + hpaPath2.NodesExplored);







        Debug.Log("A* : ");

        (List<Vector2Int> Path, int NodesExplored) Apath = Astar.FindPath(start, end, tileMap);

        Debug.Log("Path Length: " + Apath.Path.Count);
        Debug.Log("Nodes Explored: " + Apath.NodesExplored);



        // for (int i = 0; i < hpaPath2.path.Count; i++)
        // {
        //     Debug.Log("HPA Path: " + hpaPath2.path[i].Position);
        //     if (Apath.Path.Count > i)
        //         Debug.Log("A* Path: " + Apath.Path[i]);
        // }



        // Effiency in node exploration. 


    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator PathfindingTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }


    public HPAPath EffiencyInNodeExploration(int[,] tileMap, Vector2Int start, Vector2Int end, int maxLevel, Boolean RefinePath = false)
    {
        PathFinder _pathFinder = new PathFinder();
        GraphModel _graphModel = new GraphModel(tileMap);
        IEdgeManager edgeManager = new EdgeManager(_pathFinder);
        NodeManager _nodeManager = new NodeManager(_graphModel, edgeManager);
        IEntranceManager entranceManager = new EntranceManager(_graphModel, _nodeManager);
        IClusterManager clusterManager = new ClusterManager(_graphModel, _nodeManager, edgeManager, entranceManager);
        IHPAStar HPAStar = new HPAStar(_graphModel, clusterManager, _nodeManager, entranceManager, edgeManager, _pathFinder);

        HPAStar.Preprocessing(maxLevel);
        if (RefinePath)
        {
            HPAPath hpaPath = HPAStar.HierarchicalSearch(start, end, maxLevel);
            return hpaPath;
        }
        else
        {
            HPAPath hpaPath = HPAStar.HierarchicalAbstractSearch(start, end, maxLevel);
            return hpaPath;
        }

    }





}
