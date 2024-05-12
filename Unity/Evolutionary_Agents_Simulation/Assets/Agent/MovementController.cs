using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController
{
    private readonly Agent agent;
    public MovementController(Agent agent){
        this.agent = agent;
    }
    public void Move(AgentDirection.Direction direction){
        switch(direction){
            case AgentDirection.Direction.LEFT:
                agent.Position += new Vector2(-1,0);
                break;
            case AgentDirection.Direction.UP:
                agent.Position += new Vector2(0,1);
                break;
            case AgentDirection.Direction.RIGHT:
                agent.Position += new Vector2(1,0);
                break;
            case AgentDirection.Direction.DOWN:
                agent.Position += new Vector2(0,-1);
                break;
            default:
                throw new Exception("Invalid direction for agent");
        } UpdateAgentData();
    }
    public void UpdateAgentData(){
        agent.amountOfSteps++;
        agent.visitedTiles.Add(agent.Position);
    }
    public void FollowPath(){
        if(agent.path.Count == 0) throw new Exception("path is empty");
        AgentDirection.Direction newDirection = GetDirectionFromNode(agent.path.Pop());
        Move(newDirection);
    }

    public AgentDirection.Direction GetDirectionFromNode(HPANode hpaNode){
        if(agent.Position.x == hpaNode.Position.x){
            if(agent.Position.y < hpaNode.Position.y){
                return AgentDirection.Direction.UP;
            } else return AgentDirection.Direction.DOWN;
        } else {
            if(agent.Position.x < hpaNode.Position.x){
                return AgentDirection.Direction.RIGHT;
            } else return AgentDirection.Direction.LEFT;
        } throw new Exception("Invalid path position from node");
    } 

    public void SetPath(HPAPath path){
        agent.path = new Stack<HPANode>(path.path);
    }
}
