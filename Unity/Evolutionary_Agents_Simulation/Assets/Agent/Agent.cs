using System.Collections.Generic;
using UnityEngine;

public class Agent
{
    public Stack<HPANode> path;
    public Vector2 Position;
    public int amountOfSteps;
    public int exploredTiles;
    public List<Vector2> visitedTiles;
    public Agent(Vector2 position){
        Position = position;
        amountOfSteps = 0;
        exploredTiles = 0;
        visitedTiles = new();
        path = new();
    }
}
