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


public class Astar
{
    public Vector2Int startCoordinate;
    public Vector2Int endCoordinate;
    public List<Vector2Int> ShortestPath = new List<Vector2Int>();



    FibonacciHeapPriorityQueue<NodeCell> openList; // for constant time to get the lowest fCost node


    Dictionary<Vector2Int, NodeCell> closedList;



    // Directions for 8 possible movements
    private Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(-1, 0),  // Left
        new Vector2Int(1, 0),   // Right
        new Vector2Int(0, -1),  // Down
        new Vector2Int(0, 1),   // Up
       //   new Vector2Int(-1, -1), // Bottom-Left
       // new Vector2Int(-1, 1),  // Top-Left
      //  new Vector2Int(1, -1),  // Bottom-Right
      //  new Vector2Int(1, 1)    // Top-Right
    };

    public bool FindPath(Vector2Int start, Vector2Int end, int[,] map)
    {
        openList = new FibonacciHeapPriorityQueue<NodeCell>(SortDirection.Ascending); // TODO: change to Priority queue with fibonaci heap. 
        Dictionary<Vector2Int, NodeCell> openListLookup = new Dictionary<Vector2Int, NodeCell>();  // for constant time lookup


        closedList = new Dictionary<Vector2Int, NodeCell>();

        NodeCell startNode = new NodeCell(start);
        openList.Enqueue(startNode);
        openListLookup.Add(startNode.position, startNode);



        while (openList.Count > 0) // N 
        {
            Debug.Log("OpenList: " + openList.Count);
            // for (int i = 1; i < openList.Count; i++) // N
            // {

            //     if (openList[i].fCost < currentNode.fCost || (openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost))
            //     // if cost is lower or if f cost is equal and hCost is lower
            //     {
            //         currentNode = openList[i];
            //     }
            // }

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
                RetracePath(startNode, currentNode);
                return true;
            }

            foreach (Vector2Int direction in directions)  // 8
            {
                Vector2Int neighborPos = currentNode.position + direction;
                if (!PositionIsValid(neighborPos, map)) continue;

                NodeCell neighbor = FindExistingNode(neighborPos, openListLookup, closedList); // constant time lookup

                if (neighbor == null)
                {
                    //  Debug.Log("Creating new node at " + neighborPos + " from " + currentNode.position + " with gCost: " + currentNode.gCost + " and hCost: " + GetDistance(neighborPos, end) + " and fCost: " + (currentNode.gCost + GetDistance(neighborPos, end)) + " and parent: " + currentNode.position);
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
        return false;
    }

    bool PositionIsValid(Vector2Int position, int[,] map) // all 0s are walkable, 1s are walls, 2s are checkpoints (which are walkable.). 
    {
        return position.x >= 0 && position.x < map.GetLength(0) && position.y >= 0 && position.y < map.GetLength(1) && (map[position.x, position.y] != 1);
    }

    NodeCell FindExistingNode(Vector2Int position, Dictionary<Vector2Int, NodeCell> openListLookup, Dictionary<Vector2Int, NodeCell> closedList)
    {
        if (openListLookup.ContainsKey(position))
        {
            return openListLookup[position]; // Constant Look up time. 
        }

        if (closedList.ContainsKey(position))
        {
            return closedList[position]; // Constant Look up time. 
        }

        // if not in the open list or closed list, return null. 
        return null;




    }

    int GetDistance(Vector2Int a, Vector2Int b)
    {
        // int dx = Mathf.Abs(a.x - b.x);
        // int dy = Mathf.Abs(a.y - b.y);


        // if (dx > dy)
        //     return (int)14 * dy + 10 * (dx - dy);
        // return (int)14 * dx + 10 * (dy - dx);


        // (5,10) - (20,30) = (15, 20) => 15 + 20 = 35
        // Calculate the x and y distance between two points. 
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }


    void RetracePath(NodeCell startNode, NodeCell endNode)
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
        ShortestPath = path;
    }
}
