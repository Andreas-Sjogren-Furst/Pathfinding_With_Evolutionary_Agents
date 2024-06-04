using System;
using System.Collections.Generic;
using UnityEngine;

public class AgentController{
    
    public AgentModel agentModel;
    public AgentController(AgentModel agentModel){
        this.agentModel = agentModel;
        agentModel.accesibleCheckpoints = GridExplorer.CountAccesibleCheckpoints(agentModel.map, agentModel.spawnPoint,agentModel.checkPoints);
    }
    
    public void Scan(Agent agent){
        FieldOfView fieldOfView = new(agentModel.map);
        Point agentPosition = new(agent.position.x, agent.position.y);
        fieldOfView.ComputeFOV(agentPosition);
        AddVisibleTilesAndWalls(fieldOfView.markVisibleTiles,fieldOfView.markVisibleWalls);
    }

    public void UpdateFrontierPoints(){
        FrontierExplorer frontierExplorer = new FrontierExplorer(agentModel.map);
        HashSet<Point> newFrontierPoints = frontierExplorer.FindFrontierPoints(agentModel.visibleTiles,agentModel.visibleWalls);
        ResetFrontierPoints();
        foreach(Point frontierPoint in newFrontierPoints) { agentModel.frontierPoints.Add(frontierPoint); }

    }

    public void UpdateFrontier(){
        FrontierExplorer frontierExplorer = new(agentModel.map);
        Dictionary<int,Point> centroids = frontierExplorer.FindFrontier(agentModel.visibleTiles,agentModel.visibleWalls);
        ResetFrontier();
        foreach(KeyValuePair<int,Point> centroid in centroids) { agentModel.centroids.Add(centroid.Key,centroid.Value); }
    }

    private void ResetFrontierPoints(){
        agentModel.frontierPoints = new();
    }
    private void ResetFrontier(){
        agentModel.centroids = new();
    }

    private void AddVisibleTilesAndWalls(List<Point> computedVisibleTiles, List<Point> computedVisibleWalls){
        foreach(Point computedVisibleTile in computedVisibleTiles){
            agentModel.visibleTiles.Add(computedVisibleTile);
        }
        foreach(Point computedVisibleWall in computedVisibleWalls){
            agentModel.visibleWalls.Add(computedVisibleWall);
        }
    }

    private void MoveAgent(Agent agent){ 
        Vector2Int nextPosition = agent.path.Pop();
        agent.position = nextPosition;
    }

    private Point FindClosestCentroid(Agent agent){
        Dictionary<int,Point> centroids = agentModel.centroids;
        double minDistance = double.MaxValue;
        Point closestPoint = null;
        int key = 0;
        foreach(KeyValuePair<int,Point> centroid in centroids){
            Point agentPosition = new(agent.position.x,agent.position.y);
            double distance = CalculateEuclideanDistance(agentPosition, centroid.Value);
            if(distance < minDistance){
                minDistance = distance;
                key = centroid.Key;
                closestPoint = centroid.Value;
            }
        } RemoveCentroidFromStack(key);
        return closestPoint;
    }
    private void RemoveCentroidFromStack(int key){
        agentModel.centroids.Remove(key);
    }
    private double CalculateEuclideanDistance(Point p1, Point p2){
        int dx = p1.x - p2.x; 
        int dy = p1.y - p1.y;
        return Math.Sqrt(dx*dx + dy * dy);
    }

    public void SimulateAgents(){
        int[,] map = CastToIntMap(agentModel.map);
        foreach(Agent agent in agentModel.agents){
            if(agent.state == SearchState.state.exploring){
                if(agent.path.Count == 0) agent.state = SearchState.state.scanning;
                else MoveAgent(agent);
            } else if(agent.state == SearchState.state.scanning){
               Scan(agent);
               UpdateFrontier();
               UpdateFrontierPoints();
               List<Vector2Int> path = null;
               while(agentModel.centroids.Count != 0 && path == null){
                Point centroid = FindClosestCentroid(agent);
                Vector2Int start = new(agent.position.x,agent.position.y);
                Vector2Int end = new(centroid.x,centroid.y);
                path = Astar.FindPath(end, start, map).Path;
               }
               if(agentModel.centroids.Count == 0) {
                agent.state = SearchState.state.idle;
                continue;
               } 
               agent.path = new Stack<Vector2Int>(path);
               agent.state = SearchState.state.exploring;
                
               
            } else if(agent.state == SearchState.state.idle){
                if(agentModel.centroids.Count > 0){
                    agent.state = SearchState.state.exploring;
                } Debug.Log("Im in idle");
            } else throw new SystemException("Invalid state for agent");
        }
    }

    private int[,] CastToIntMap(MapObject[,] map){
        int width = map.GetLength(0);
        int height = map.GetLength(1);
        int[,] intMap = new int[height,width];
        for(int i = 0; i < height; i++){
            for(int j = 0; j < width; j++){
                MapObject.ObjectType mapObjectType = map[i,j].Type;
                if(mapObjectType == MapObject.ObjectType.Tile)
                    intMap[i,j] = 0;
                else if(mapObjectType == MapObject.ObjectType.Wall)
                    intMap[i,j] = 1;
                else
                    throw new SystemException("Couldn't convert mapObject type to a valid integer");
            }
        } return intMap; 
    }
}