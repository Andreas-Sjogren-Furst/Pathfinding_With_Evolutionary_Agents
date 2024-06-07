// // written by: Gustav Clausen s214940

// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Assertions;
// using UnityEngine.TestTools;

// public class HPAStarTest
// {
//     private HPAStar _hpaStar;
//     private IGraphModel _graphModel;
//     private IClusterManager _clusterManager;
//     private INodeManager _nodeManager;
//     private IEntranceManager _entranceManager;
//     private IEdgeManager _edgeManager;
//     private IPathFinder _pathFinder;

//     [SetUp]
//     public void Setup()
//     {
//         // Mock or create instances of dependencies
//         _graphModel = new GraphModel();
//         _clusterManager = new MockClusterManager();
//         _nodeManager = new MockNodeManager();
//         _entranceManager = new MockEntranceManager();
//         _edgeManager = new MockEdgeManager();
//         _pathFinder = new MockPathFinder();

//         // Initialize HPAStar with mocked dependencies
//         _hpaStar = new HPAStar(_graphModel, _clusterManager, _nodeManager, _entranceManager, _edgeManager, _pathFinder);
//     }

//     [Test]
//     public void TestPreprocessing()
//     {
//         int maxLevel = 3;

//         // Run preprocessing to simulate environment setup
//         _hpaStar.Preprocessing(maxLevel);

//         // Assert that the correct levels have been added to the graph
//         Assert.IsTrue(_graphModel.ClusterByLevel.ContainsKey(1));
//         Assert.IsTrue(_graphModel.ClusterByLevel.ContainsKey(2));
//         Assert.IsTrue(_graphModel.ClusterByLevel.ContainsKey(3));
//     }

//     [Test]
//     public void TestPathfinding()
//     {
//         // Setup the environment
//         _hpaStar.Preprocessing(3);

//         Vector2Int start = new Vector2Int(0, 0);
//         Vector2Int goal = new Vector2Int(5, 5);
//         int level = 1;

//         // Perform pathfinding
//         var path = _hpaStar.HierarchicalSearch(start, goal, level);

//         // Check if the path is not null
//         Assert.IsNotNull(path);
//         // Check if the path starts and ends at the correct positions
//         Assert.AreEqual(start, path[0].Position);
//         Assert.AreEqual(goal, path[path.Count - 1].Position);
//     }

//     [UnityTest]
//     public IEnumerator TestDynamicNodeAddition()
//     {
//         _hpaStar.Preprocessing(3);

//         Vector2Int newNodePosition = new Vector2Int(2, 2);
//         _hpaStar.DynamicallyAddHPANode(newNodePosition, true);

//         yield return null; // Skip a frame

//         // Assert that the new node has been added properly
//         Assert.IsTrue(_nodeManager.NodeExists(newNodePosition, 1));

//         // Check if node's addition reflects in the graph structure
//         Assert.IsTrue(_clusterManager.IsNodeInCorrectCluster(newNodePosition, 1));
//     }
// }

// internal class TestAttribute : Attribute
// {
// }