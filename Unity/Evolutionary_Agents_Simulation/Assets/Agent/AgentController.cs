

using System.Diagnostics;

public class AgentController{
    
    public AgentModel agentModel;
    public AgentController(AgentModel agentModel){
        this.agentModel = agentModel;
    }
    
    public void MoveAgents(Agent[] agents){
        foreach(Agent agent in agents){
            MoveAgent(agent);
        }
    }
    public void MoveAgent(Agent agent){
        if(agent.path.Count == 0) throw new System.Exception("MoveAgent failed because path was empty");
        HPANode nextLocation = agent.path.Pop();
        agent.position = nextLocation.Position;
    }
}