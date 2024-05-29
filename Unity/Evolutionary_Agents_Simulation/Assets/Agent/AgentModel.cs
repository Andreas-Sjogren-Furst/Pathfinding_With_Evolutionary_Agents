using System.Collections;

public class AgentModel
{
    public Agent[] agents;
    public int amountOfAgents;
    public MapObject[,] map;
    
    // Stack<Vector2Int> frontierPoints;
    
    public AgentModel(int amountOfAgents, MapObject[,] map){
        this.amountOfAgents = amountOfAgents;
        this.map = map;
        agents = new Agent[amountOfAgents];
    }
}
