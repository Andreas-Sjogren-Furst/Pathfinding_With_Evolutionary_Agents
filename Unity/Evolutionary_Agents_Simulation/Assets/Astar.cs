using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NodeCell
{
    public Vector2Int position;
    public float gCost; // Cost from start to node
    public float hCost; // Heuristic cost from node to end
    public float fCost { get { return gCost + hCost; } } // Total cost
    public NodeCell parent;
    public NodeCell(Vector2Int _position) { position = _position; }
}

public class Astar : MonoBehaviour
{

    public Vector2Int startCoordinate;
    public Vector2Int endCoordinate;
    public int[,] map;

    List<NodeCell> openList; // open for exploration. Nodes with lowest f(n) is chosen. 
    HashSet<NodeCell> closedList; // already explored and their neigbours. 

    void Start()
    {
        FindPath(startCoordinate, endCoordinate);
    }

    void FindPath(Vector2Int start, Vector2Int end)
    {
        openList = new List<NodeCell>();
        closedList = new HashSet<NodeCell>();

        NodeCell startNode = new NodeCell(start);
        NodeCell endNode = new NodeCell(end);

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            NodeCell currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].fCost < currentNode.fCost || openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost)
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode.position == end)
            {
                RetracePath(startNode, endNode);
                return;
            }

            foreach (NodeCell neighbor in GetNeighbors(currentNode))
            {
                if (map[neighbor.position.x, neighbor.position.y] != 0 || closedList.Contains(neighbor))
                {
                    continue;
                }

                float newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newCostToNeighbor < neighbor.gCost || !openList.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor; // g(n)
                    neighbor.hCost = GetDistance(neighbor, endNode); // h(n)
                    neighbor.parent = currentNode;

                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }
        }
    }

    List<NodeCell> GetNeighbors(NodeCell node)
    {
        List<NodeCell> neighbors = new List<NodeCell>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                Vector2Int checkPoint = new Vector2Int(node.position.x + x, node.position.y + y);
                if (checkPoint.x >= 0 && checkPoint.x < map.GetLength(0) && checkPoint.y >= 0 && checkPoint.y < map.GetLength(1))
                {
                    neighbors.Add(new NodeCell(checkPoint));
                }
            }
        }
        return neighbors;
    }

    float GetDistance(NodeCell a, NodeCell b)
    {
        float distX = Mathf.Abs(a.position.x - b.position.x);
        float distY = Mathf.Abs(a.position.y - b.position.y);

        if (distX > distY) // 14 is approximation of sqrt(2)*10, 10 for straightmoves
            return 14 * distY + 10 * (distX - distY);
        return 14 * distX + 10 * (distY - distX);
    }

    void RetracePath(NodeCell startNode, NodeCell endNode)
    {
        List<NodeCell> path = new List<NodeCell>();
        NodeCell currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        // Here you can convert the path into world positions or whatever format you need
    }
}
