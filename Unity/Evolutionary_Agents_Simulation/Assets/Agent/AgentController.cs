using System.Collections.Generic;

public class AgentController{
    
    public AgentModel agentModel;
    public AgentController(AgentModel agentModel){
        this.agentModel = agentModel;
    }
    
    public void Scan(){
        FieldOfView fieldOfView = new(agentModel.map);
        Agent agent = agentModel.agents[0];
        Point agentPosition = new(agent.position.x, agent.position.y);
        fieldOfView.ComputeFOV(agentPosition);
        List<Point> computedVisibleTiles = fieldOfView.markVisibleTiles;
        List<Point> computedVisibleWalls = fieldOfView.markVisibleWalls;
        AddVisibleTilesAndWalls(computedVisibleTiles,computedVisibleWalls);
    }

    public void UpdateFrontierPoints(){
        FrontierExplorer frontierExplorer = new FrontierExplorer(agentModel.map);
        HashSet<Point> newFrontierPoints = frontierExplorer.FindFrontierPoints(agentModel.visibleTiles,agentModel.visibleWalls);
        ResetFrontierPoints();
        foreach(Point frontierPoint in newFrontierPoints) { agentModel.frontierPoints.Add(frontierPoint); }

    }

    public void UpdateFrontier(){
        FrontierExplorer frontierExplorer = new(agentModel.map);
        HashSet<Point> frontiers = frontierExplorer.FindFrontier(agentModel.visibleTiles,agentModel.visibleWalls);
        ResetFrontier();
        foreach(Point frontier in frontiers) { agentModel.frontier.Add(frontier); }
    }

    private void ResetFrontierPoints(){
        agentModel.frontierPoints = new();
    }
    private void ResetFrontier(){
        agentModel.frontier = new();
    }

    private void AddVisibleTilesAndWalls(List<Point> computedVisibleTiles, List<Point> computedVisibleWalls){
        foreach(Point computedVisibleTile in computedVisibleTiles){
            agentModel.visibleTiles.Add(computedVisibleTile);
        }
        foreach(Point computedVisibleWall in computedVisibleWalls){
            agentModel.visibleWalls.Add(computedVisibleWall);
        }
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