using System;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class NodeCell : IComparable<NodeCell>
{
    public Vector2Int position;
    public int gCost; // Cost from start to node
    public int hCost; // Heuristic cost from node to end
    public int fCost => gCost + hCost; // Total cost
    public NodeCell parent;
    public NodeCell(Vector2Int _position) { position = _position; }

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
    // Directions for 4 possible movements
    private static Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(-1, 0),  // Left
        new Vector2Int(1, 0),   // Right
        new Vector2Int(0, -1),  // Down
        new Vector2Int(0, 1),   // Up
    };

    public static List<Vector2Int> FindPath(Vector2Int start, Vector2Int end, MapObject[,] map)
    {

        FibonacciHeapPriorityQueue<NodeCell> openList; // for constant time to get the lowest fCost node

        Dictionary<Vector2Int, NodeCell> closedList;

        openList = new FibonacciHeapPriorityQueue<NodeCell>(SortDirection.Ascending); // TODO: change to Priority queue with fibonaci heap. 
        Dictionary<Vector2Int, NodeCell> openListLookup = new Dictionary<Vector2Int, NodeCell>();  // for constant time lookup


        closedList = new Dictionary<Vector2Int, NodeCell>();

        NodeCell startNode = new NodeCell(start);
        openList.Enqueue(startNode);
        openListLookup.Add(startNode.position, startNode);



        while (openList.Count > 0) // N 
        {
            NodeCell currentNode = openList.Dequeue();
            openListLookup.Remove(currentNode.position);

            if (closedList.ContainsKey(currentNode.position))
            {
                closedList[currentNode.position] = currentNode;
            }
            else
            {
                closedList.Add(currentNode.position, currentNode);

            }


            if (currentNode.position == end) // N
            {

                return RetracePath(startNode, currentNode); ;
            }

            foreach (Vector2Int direction in directions)  // 4
            {
                Vector2Int neighborPos = currentNode.position + direction;
                if (!PositionIsValid(neighborPos, map)) continue;

                NodeCell neighbor = FindExistingNode(neighborPos, openListLookup, closedList); // constant time lookup

                if (neighbor == null)
                {
                    neighbor = new NodeCell(neighborPos);
                }

                if (closedList.ContainsKey(neighbor.position)) continue;

                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode.position, neighbor.position);
                if (newCostToNeighbor < neighbor.gCost || !openListLookup.ContainsKey(neighbor.position)) // constant time lookup
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor.position, end);
                    neighbor.parent = currentNode;

                    if (!openListLookup.ContainsKey(neighbor.position))
                    {
                        openList.Enqueue(neighbor);
                        openListLookup.Add(neighbor.position, neighbor);
                    }
                }
            }
        }
        return null;
    }

    private static bool PositionIsValid(Vector2Int position, MapObject[,] map) // all 0s are walkable, 1s are walls, 2s are checkpoints (which are walkable.). 
    {
        return position.x >= 0 && position.x < map.GetLength(0) && position.y >= 0 && position.y < map.GetLength(1) && (map[position.x, position.y].Type != MapObject.ObjectType.Wall);
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
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }


    private static List<Vector2Int> RetracePath(NodeCell startNode, NodeCell endNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        NodeCell currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.position);
            currentNode = currentNode.parent;
        }
        path.Add(startNode.position); // Ensure the start node is included in the path
        path.Reverse();
        return path;
    }
}
