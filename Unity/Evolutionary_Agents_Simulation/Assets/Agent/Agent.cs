using System.Collections.Generic;
using UnityEngine;

public class Agent
{
    public Stack<HPANode> path;
    public Vector2 position;
    public int amountOfSteps;
    public int agentId;
    public int exploredTiles;
    public List<Vector2> visitedTiles;
    public Agent(Vector2 position, int agentId){
        this.position = position;
        this.agentId = agentId;
        amountOfSteps = 0;
        exploredTiles = 0;
        visitedTiles = new();
        path = new();
    }
}
