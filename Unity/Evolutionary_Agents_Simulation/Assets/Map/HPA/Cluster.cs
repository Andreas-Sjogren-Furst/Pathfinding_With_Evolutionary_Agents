
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cluster
{
    public Guid Id { get; set; }
    public int Level { get; set; }

    public Vector2Int bottomLeftPos { get; set; }
    public Vector2Int topRightPos { get; set; }
    public HashSet<HPANode> Nodes { get; set; }

    public HashSet<HPANode> NodesLeft { get { return getNodesBasedOnDirection(CompassDirection.West); } }
    public HashSet<HPANode> NodesRight { get { return getNodesBasedOnDirection(CompassDirection.East); } }
    public HashSet<HPANode> NodesTop { get { return getNodesBasedOnDirection(CompassDirection.North); } }
    public HashSet<HPANode> NodesBottom { get { return getNodesBasedOnDirection(CompassDirection.South); } }



    public int ClusterLengthInTile { get { return topRightPos.x - bottomLeftPos.x; } }


    public HashSet<Entrance> Entrances { get; set; }

    public Cluster(int level, HashSet<HPANode> hPANodes, HashSet<Entrance> entrances, Vector2Int bottomLeftPos, Vector2Int topRightPos)
    {


        Id = Guid.NewGuid();
        Level = level;
        Nodes = hPANodes;
        Entrances = entrances;

        this.bottomLeftPos = bottomLeftPos;
        this.topRightPos = topRightPos;



    }

    public bool Contains(Vector2Int position)
    {
        return position.x >= bottomLeftPos.x && position.x <= topRightPos.x && position.y >= bottomLeftPos.y && position.y <= topRightPos.y;

    }


    private HashSet<HPANode> getNodesBasedOnDirection(CompassDirection direction)
    {
        HashSet<HPANode> nodes = new HashSet<HPANode>();
        foreach (HPANode node in this.Nodes)
        {
            if (direction == CompassDirection.North)
            {
                if (node.Position.y == topRightPos.y)
                {
                    nodes.Add(node);
                }
            }
            if (direction == CompassDirection.South)
            {
                if (node.Position.y == bottomLeftPos.y)
                {
                    nodes.Add(node);
                }
            }
            if (direction == CompassDirection.East)
            {
                if (node.Position.x == topRightPos.x)
                {
                    nodes.Add(node);
                }
            }
            if (direction == CompassDirection.West)
            {
                if (node.Position.x == bottomLeftPos.x)
                {
                    nodes.Add(node);
                }
            }
        }


        return nodes;
    }


    // private void setNodesFromCoordinate()
    // {
    //     foreach (HPANode node in this.Nodes)
    //     {
    //         if (node.Position.x == bottomLeftPos.x)
    //         {
    //             this.NodesLeft.Add(node);
    //         }
    //         if (node.Position.x == topRightPos.x)
    //         {
    //             this.NodesRight.Add(node);
    //         }
    //         if (node.Position.y == bottomLeftPos.y)
    //         {
    //             this.NodesBottom.Add(node);
    //         }
    //         if (node.Position.y == topRightPos.y)
    //         {
    //             this.NodesTop.Add(node);
    //         }
    //     }
    // }



}

public enum CompassDirection
{
    North,
    South,
    East,
    West,
    None

}

