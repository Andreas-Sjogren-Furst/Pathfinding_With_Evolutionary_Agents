using System.Collections;

public class AgentModel
{
    public Agent[] agents;
    public int amountOfAgents;
    
    //Stack<Vector2Int> frontierPoints;
    
    public AgentModel(int amountOfAgents){
        this.amountOfAgents = amountOfAgents;
        agents = new Agent[amountOfAgents];
    }
}
