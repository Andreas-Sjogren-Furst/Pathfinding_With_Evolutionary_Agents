using System;
using System.Collections.Generic;
using UnityEngine;

public class NodeCell
{
    public Vector2Int position;
    public int gCost; // Cost from start to node
    public int hCost; // Heuristic cost from node to end
    public int fCost { get { return gCost + hCost; } } // Total cost
    public NodeCell parent;
    public NodeCell(Vector2Int _position) { position = _position; }
}

public class Astar
{
    public Vector2Int startCoordinate;
    public Vector2Int endCoordinate;
    public List<Vector2Int> ShortestPath = new List<Vector2Int>();

    List<NodeCell> openList;
    HashSet<NodeCell> closedList;

    // Directions for 8 possible movements
    private Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(-1, 0),  // Left
        new Vector2Int(1, 0),   // Right
        new Vector2Int(0, -1),  // Down
        new Vector2Int(0, 1),   // Up
        new Vector2Int(-1, -1), // Bottom-Left
        new Vector2Int(-1, 1),  // Top-Left
        new Vector2Int(1, -1),  // Bottom-Right
        new Vector2Int(1, 1)    // Top-Right
    };

    public bool FindPath(Vector2Int start, Vector2Int end, int[,] map)
    {
        openList = new List<NodeCell>();
        closedList = new HashSet<NodeCell>();

        NodeCell startNode = new NodeCell(start);
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Debug.Log("OpenList: " + openList.Count);
            NodeCell currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].fCost < currentNode.fCost || (openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost))
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode.position == end)
            {
                RetracePath(startNode, currentNode);
                return true;
            }

            foreach (Vector2Int direction in directions)
            {
                Vector2Int neighborPos = currentNode.position + direction;
                if (!PositionIsValid(neighborPos, map)) continue;

                NodeCell neighbor = FindExistingNode(neighborPos, openList, closedList);
                if (neighbor == null)
                {
                    //  Debug.Log("Creating new node at " + neighborPos + " from " + currentNode.position + " with gCost: " + currentNode.gCost + " and hCost: " + GetDistance(neighborPos, end) + " and fCost: " + (currentNode.gCost + GetDistance(neighborPos, end)) + " and parent: " + currentNode.position);
                    neighbor = new NodeCell(neighborPos);
                }

                if (closedList.Contains(neighbor)) continue;

                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode.position, neighbor.position);
                if (newCostToNeighbor < neighbor.gCost || !openList.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor.position, end);
                    neighbor.parent = currentNode;

                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }
        }
        return false;
    }

    bool PositionIsValid(Vector2Int position, int[,] map) // all 0s are walkable, 1s are walls, 2s are checkpoints (which are walkable.). 
    {
        return position.x >= 0 && position.x < map.GetLength(0) && position.y >= 0 && position.y < map.GetLength(1) && (map[position.x, position.y] != 1);
    }

    NodeCell FindExistingNode(Vector2Int position, List<NodeCell> openList, HashSet<NodeCell> closedList)
    {
        foreach (var node in openList)
        {
            if (node.position == position) return node;
        }

        foreach (var node in closedList)
        {
            if (node.position == position) return node;
        }

        return null;
    }

    int GetDistance(Vector2Int a, Vector2Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);


        if (dx > dy)
            return (int)14 * dy + 10 * (dx - dy);
        return (int)14 * dx + 10 * (dy - dx);
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
