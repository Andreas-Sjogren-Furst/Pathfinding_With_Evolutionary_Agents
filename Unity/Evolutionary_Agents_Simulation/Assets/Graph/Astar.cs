// written by: Gustav Clausen s214940

using System;
using System.Collections.Generic;
using UnityEngine;

public class NodeCell : IComparable<NodeCell>
{
    public Vector2Int Position;
    public int gCost; // Cost from start to node
    public int hCost; // Heuristic cost from node to end
    public int fCost => gCost + hCost; // Total cost
    public NodeCell parent;

    public NodeCell(Vector2Int position) { Position = position; }

    public int CompareTo(NodeCell other)
    {
        if (other == null) return 1;

        int compare = fCost.CompareTo(other.fCost);
        if (compare == 0)
        {
            // If fCost is the same, further compare by hCost to maintain a consistent ordering
            compare = hCost.CompareTo(other.hCost);
        }
        return compare;
    }
}


public static class Astar
{
    private static Vector2Int[] directions = new Vector2Int[]
      {
        new Vector2Int(-1, 0),  // Left
        new Vector2Int(1, 0),   // Right
        new Vector2Int(0, -1),  // Down
        new Vector2Int(0, 1),   // Up
      };

    public static (List<Vector2Int> Path, int NodesExplored) FindPath(Vector2Int start, Vector2Int end, int[,] map) // level 0, to find path directly on map without clusters. 
    {

        // Directions for 4 possible movements


        FibonacciHeapPriorityQueue<NodeCell> openList; // for constant time to get the lowest fCost node


        Dictionary<Vector2Int, NodeCell> closedList;

        openList = new FibonacciHeapPriorityQueue<NodeCell>(SortDirection.Ascending); // priority queue with fibonaci heap. 
        Dictionary<Vector2Int, NodeCell> openListLookup = new Dictionary<Vector2Int, NodeCell>();  // for constant time lookup


        closedList = new Dictionary<Vector2Int, NodeCell>();

        NodeCell startNode = new NodeCell(start);
        openList.Enqueue(startNode);
        openListLookup.Add(startNode.Position, startNode);



        while (openList.Count > 0) // N 
        {

            NodeCell currentNode = openList.Dequeue(); // log n. 
            openListLookup.Remove(currentNode.Position);

            if (closedList.ContainsKey(currentNode.Position))
            {
                closedList[currentNode.Position] = currentNode;
            }
            else
            {
                closedList.Add(currentNode.Position, currentNode);

            }


            if (currentNode.Position == end) // N
            {

                return (RetracePath(startNode, currentNode), closedList.Count);
            }

            foreach (Vector2Int direction in directions)  // 8
            {
                Vector2Int neighborPos = currentNode.Position + direction;
                if (!PositionIsValid(neighborPos, map)) continue;

                NodeCell neighbor = FindExistingNode(neighborPos, openListLookup, closedList); // constant time lookup

                if (neighbor == null)
                {
                    neighbor = new NodeCell(neighborPos);
                }

                if (closedList.ContainsKey(neighbor.Position)) continue;

                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode.Position, neighbor.Position);
                if (newCostToNeighbor < neighbor.gCost || !openListLookup.ContainsKey(neighbor.Position)) // constant time lookup
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor.Position, end);
                    neighbor.parent = currentNode;

                    if (!openListLookup.ContainsKey(neighbor.Position))
                    {
                        openList.Enqueue(neighbor);
                        openListLookup.Add(neighbor.Position, neighbor);
                    }
                }
            }
        }
        return (null, closedList.Count);
    }


    public static HPAPath FindPath(HPANode start, HPANode end, HPAEdgeType edgeType = HPAEdgeType.INTRA)
    {
        var openList = new FibonacciHeapPriorityQueue<HPANode>(SortDirection.Ascending);
        var openListLookup = new Dictionary<Vector2Int, HPANode>();
        var closedList = new Dictionary<Vector2Int, HPANode>();

        start.gCost = 0;
        start.hCost = GetDistance(start.Position, end.Position);



        openList.Enqueue(start);
        openListLookup.Add(start.Position, start);


        while (openList.Count > 0)
        {

            // When moving a node from open to closed list
            HPANode currentNode = openList.Dequeue();
            openListLookup.Remove(currentNode.Position);


            if (closedList.ContainsKey(currentNode.Position))
            {
                closedList[currentNode.Position] = currentNode;
            }
            else
            {
                closedList.Add(currentNode.Position, currentNode);
            }

            if (currentNode.Position == end.Position)
            {
                HPAPath hpaPath = new HPAPath(RetracePath(start, currentNode));
                hpaPath.NodesExplored = closedList.Count;
                return hpaPath;
            }

            foreach (HPAEdge edge in currentNode.Edges)
            {
                if (edge.Type != edgeType) continue;
                HPANode neighbor = edge.Node1 == currentNode ? edge.Node2 : edge.Node1;

                if (closedList.ContainsKey(neighbor.Position)) continue;

                int newCostToNeighbor = currentNode.gCost + (int)edge.Weight; // cost of moving to neighbor
                if (newCostToNeighbor < neighbor.gCost || !openListLookup.ContainsKey(neighbor.Position))
                {

                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor.Position, end.Position);
                    neighbor.parent = currentNode;

                    if (!openListLookup.ContainsKey(neighbor.Position))
                    {
                        // When adding to open list


                        openList.Enqueue(neighbor);
                        openListLookup.Add(neighbor.Position, neighbor);
                    }
                }
            }
        }


        return null;

    }

    // Adjust the GetDistance method if necessary to fit your needs

    private static List<HPANode> RetracePath(HPANode startNode, HPANode endNode)
    {
        List<HPANode> path = new List<HPANode>();
        HPANode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent; // Cast to HPANode if necessary
        }
        path.Add(startNode);
        path.Reverse();




        return path;
    }



    private static bool PositionIsValid(Vector2Int position, int[,] map) // all 0s are walkable, 1s are walls, 2s are checkpoints (which are walkable.). 
    {
        return position.x >= 0 && position.x < map.GetLength(0) && position.y >= 0 && position.y < map.GetLength(1) && (map[position.x, position.y] != 1);
    }

    private static NodeCell FindExistingNode(Vector2Int position, Dictionary<Vector2Int, NodeCell> openListLookup, Dictionary<Vector2Int, NodeCell> closedList)
    {
        if (openListLookup.ContainsKey(position))
        {
            return openListLookup[position]; // Constant Look up time. 
        }

        if (closedList.ContainsKey(position))
        {
            return closedList[position]; // Constant Look up time. 
        }
        return null;
    }

    private static int GetDistance(Vector2Int a, Vector2Int b)
    {

        // Calculate the x and y distance between two points, manhatten. 
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }


    private static List<Vector2Int> RetracePath(NodeCell startNode, NodeCell endNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        NodeCell currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.parent;
        }
        path.Add(startNode.Position); // Ensure the start node is included in the path
        path.Reverse();
        return path;
    }


}
