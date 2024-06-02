using System.Collections.Generic;
using UnityEngine;

public class Agent
{
    public Stack<Vector2Int> path;
    public Vector2Int position;
    public int amountOfSteps;
    public int agentId;
    public int exploredTiles;
    public List<Vector2> visitedTiles;
    public SearchState.state state;
    public Agent(Vector2Int position, int agentId){
        this.position = position;
        this.agentId = agentId;
        amountOfSteps = 0;
        exploredTiles = 0;
        visitedTiles = new();
        path = new();
        state = SearchState.state.scanning;
    }
}
