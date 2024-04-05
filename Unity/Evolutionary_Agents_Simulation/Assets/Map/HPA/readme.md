
        // HPAEdge[,] AdjenctLevel0 = Cluster.ConvertGridToAdjacencyMatrix(tileMap, tileToHPANode);
        // Cluster cluster = new Cluster(level: 0, hPANodes: Level0HPANodes, adjacencyMatrix: AdjenctLevel0, entrances: new List<Entrance>());

        // hpaGraph.AdjacencyMatrices.Add(0, AdjenctLevel0);

        // hpaGraph.ClusterByLevel.Add(0, new List<Cluster> { cluster });

        // hpaGraph.NodesByLevel.Add(0, Level0HPANodes);





        // foreach (HPANode node in Level0HPANodes)
        // {
        //     node.Cluster = cluster;
        //     hpaGraph.insertNode(node, 0);
        // }

        // // Create HPAEdges between adjacent HPANodes
        // foreach (HPANode node in tileToHPANode.Values)
        // {
        //     Vector2Int[] directions = new Vector2Int[]
        //     {
        //         new Vector2Int(-1, 0), // Left
        //         new Vector2Int(1, 0),  // Right
        //         new Vector2Int(0, -1), // Down
        //         new Vector2Int(0, 1)   // Up
        //     };

        //     foreach (Vector2Int direction in directions)
        //     {
        //         Vector2Int neighborPos = node.Position + direction;
        //         if (tileToHPANode.ContainsKey(neighborPos))
        //         {
        //             HPANode neighbor = tileToHPANode[neighborPos];
        //             // Create an edge between the nodes
        //             // You can assign appropriate weights based on your requirements
        //             // For simplicity, assuming a weight of 1 for all edges
        //             hpaGraph.AddHPAEdge(n1: node, n2: neighbor, level: 0, weight: 1, HPAEdgeType.INTRA);
        //         }
        //     }
        // }

        // // Perform HPA* preprocessing
        // hpaGraph.AbstractMaze();
        // hpaGraph.BuildGraph();
        // hpaGraph.Preprocessing(1); // Specify the maximum level for preprocessing