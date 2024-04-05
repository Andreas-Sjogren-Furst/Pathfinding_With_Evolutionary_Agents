using System;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
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

    // public static List<Vector2Int> FindPath(Vector2Int start, Vector2Int end, int[,] map)
    // {

    //     // Directions for 4 possible movements


    //     FibonacciHeapPriorityQueue<NodeCell> openList; // for constant time to get the lowest fCost node


    //     Dictionary<Vector2Int, NodeCell> closedList;

    //     openList = new FibonacciHeapPriorityQueue<NodeCell>(SortDirection.Ascending); // TODO: change to Priority queue with fibonaci heap. 
    //     Dictionary<Vector2Int, NodeCell> openListLookup = new Dictionary<Vector2Int, NodeCell>();  // for constant time lookup


    //     closedList = new Dictionary<Vector2Int, NodeCell>();

    //     NodeCell startNode = new NodeCell(start);
    //     openList.Enqueue(startNode);
    //     openListLookup.Add(startNode.Position, startNode);



    //     while (openList.Count > 0) // N 
    //     {
    //         Debug.Log("OpenList: " + openList.Count);
    //         // for (int i = 1; i < openList.Count; i++) // N
    //         // {

    //         //     if (openList[i].fCost < currentNode.fCost || (openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost))
    //         //     // if cost is lower or if f cost is equal and hCost is lower
    //         //     {
    //         //         currentNode = openList[i];
    //         //     }
    //         // }

    //         NodeCell currentNode = openList.Dequeue();
    //         openListLookup.Remove(currentNode.Position);

    //         if (closedList.ContainsKey(currentNode.Position))
    //         {
    //             closedList[currentNode.Position] = currentNode;
    //         }
    //         else
    //         {
    //             closedList.Add(currentNode.Position, currentNode);

    //         }


    //         if (currentNode.Position == end) // N
    //         {

    //             return RetracePath(startNode, currentNode); ;
    //         }

    //         foreach (Vector2Int direction in directions)  // 8
    //         {
    //             Vector2Int neighborPos = currentNode.Position + direction;
    //             if (!PositionIsValid(neighborPos, map)) continue;

    //             NodeCell neighbor = FindExistingNode(neighborPos, openListLookup, closedList); // constant time lookup

    //             if (neighbor == null)
    //             {
    //                 //  Debug.Log("Creating new node at " + neighborPos + " from " + currentNode.position + " with gCost: " + currentNode.gCost + " and hCost: " + GetDistance(neighborPos, end) + " and fCost: " + (currentNode.gCost + GetDistance(neighborPos, end)) + " and parent: " + currentNode.position);
    //                 neighbor = new NodeCell(neighborPos);
    //             }

    //             if (closedList.ContainsKey(neighbor.Position)) continue;

    //             int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode.Position, neighbor.Position);
    //             if (newCostToNeighbor < neighbor.gCost || !openListLookup.ContainsKey(neighbor.Position)) // constant time lookup
    //             {
    //                 neighbor.gCost = newCostToNeighbor;
    //                 neighbor.hCost = GetDistance(neighbor.Position, end);
    //                 neighbor.parent = currentNode;

    //                 if (!openListLookup.ContainsKey(neighbor.Position))
    //                 {
    //                     openList.Enqueue(neighbor);
    //                     openListLookup.Add(neighbor.Position, neighbor);
    //                 }
    //             }
    //         }
    //     }
    //     return null;
    // }


    public static List<HPANode> FindPath(HPANode start, HPANode end)
    {
        var openList = new FibonacciHeapPriorityQueue<HPANode>(SortDirection.Ascending);
        var openListLookup = new Dictionary<Vector2Int, HPANode>();
        var closedList = new Dictionary<Vector2Int, HPANode>();

        start.gCost = 0;
        start.hCost = GetDistance(start.Position, end.Position);

        // Debug.Log($"Adding to open list: {neighbor.Position} with gCost: {neighbor.gCost} and hCost: {neighbor.hCost}.");
        // if (currentNode.parent != null)
        // {
        //     Debug.DrawLine(new Vector3(currentNode.Position.x, currentNode.Position.y, 0), new Vector3(currentNode.parent.Position.x, currentNode.parent.Position.y, 0), Color.red, duration: 0.001f);
        // }



        openList.Enqueue(start);
        openListLookup.Add(start.Position, start);
        Debug.Log($"Starting pathfinding from {start.Position} to {end.Position}.");
        Vector3 trans = new Vector3((float)(1 / 2.0), 0, (float)(1 / 2.0));
        Vector3 curTrans = new Vector3(-50, 1, -50);
        Debug.DrawRay(new Vector3(start.Position.x, 0, start.Position.y) + trans + curTrans, new Vector3(end.Position.x, 0, end.Position.y) + trans + curTrans, Color.green, duration: 5f);


        while (openList.Count > 0)
        {

            // When moving a node from open to closed list
            HPANode currentNode = openList.Dequeue();
            openListLookup.Remove(currentNode.Position);
            Debug.Log($"Moving to closed list: {currentNode.Position}.");





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
                return RetracePath(start, currentNode);
            }

            foreach (HPAEdge edge in currentNode.Edges)
            {
                HPANode neighbor = edge.Node1 == currentNode ? edge.Node2 : edge.Node1;

                if (closedList.ContainsKey(neighbor.Position)) continue;

                int newCostToNeighbor = currentNode.gCost + (int)edge.Weight;
                if (newCostToNeighbor < neighbor.gCost || !openListLookup.ContainsKey(neighbor.Position))
                {

                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor.Position, end.Position);
                    neighbor.parent = currentNode;

                    Debug.Log($"Updating node {neighbor.Position}: New gCost: {newCostToNeighbor}, New hCost: {neighbor.hCost}.");
                    if (!openListLookup.ContainsKey(neighbor.Position))
                    {
                        // When adding to open list


                        openList.Enqueue(neighbor);
                        openListLookup.Add(neighbor.Position, neighbor);
                    }
                }
            }
        }



        throw new Exception("Path not found");
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




    // public static List<Vector2Int> FindPath(HPANode startNode, HPANode endNode)
    // {
    //     var openList = new FibonacciHeapPriorityQueue<NodeCell>(SortDirection.Ascending);
    //     var openListLookup = new Dictionary<int, NodeCell>();
    //     var closedList = new Dictionary<int, NodeCell>();


    //     NodeCell startCell = new NodeCell(startNode.Position) { gCost = 0, hCost = CalculateHCost(startNode, endNode) };
    //     openList.Enqueue(startNode);
    //     openListLookup.Add(startNode.Id, startCell);

    //     while (openList.Count > 0)
    //     {
    //         NodeCell currentCell = openList.Dequeue();
    //         HPANode currentNode = currentCell.Node;

    //         // Check if we've reached the end node
    //         if (currentNode.Id == endNode.Id)
    //         {
    //             return RetracePath(startCell, currentCell);
    //         }

    //         openListLookup.Remove(currentNode.Id);
    //         closedList[currentNode.Id] = currentCell;

    //         // Explore neighbors
    //         foreach (var edge in currentNode.Edges)
    //         {
    //             HPANode neighborNode = edge.Node1.Id == currentNode.Id ? edge.Node2 : edge.Node1;
    //             if (closedList.ContainsKey(neighborNode.Id)) continue; // Skip if already processed

    //             int tentativeGCost = currentCell.gCost + (int)edge.Weight;
    //             NodeCell neighborCell = openListLookup.GetValueOrDefault(neighborNode.Id);

    //             // If new path to neighbor is shorter OR neighbor is not in openList
    //             if (neighborCell == null)
    //             {
    //                 neighborCell = new NodeCell(neighborNode);
    //                 openListLookup[neighborNode.Id] = neighborCell;
    //             }
    //             else if (tentativeGCost >= neighborCell.gCost) continue; // Skip if no better path found

    //             // Update neighbor properties
    //             neighborCell.gCost = tentativeGCost;
    //             neighborCell.hCost = CalculateHCost(neighborNode, endNode);
    //             neighborCell.parent = currentCell;

    //             if (!openList.Contains(neighborCell))
    //             {
    //                 openList.Enqueue(neighborCell, neighborCell.fCost);
    //             }
    //         }
    //     }


    //     return null; // Path not found
    // }




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

        // if not in the open list or closed list, return null. 
        return null;




    }

    private static int GetDistance(Vector2Int a, Vector2Int b)
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
