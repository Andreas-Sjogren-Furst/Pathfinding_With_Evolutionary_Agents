using System.Collections.Generic;
using UnityEngine;

public class AgentModel : ScriptableObject
{
    public State.ExploringState state;
    [Range(1,10)] public float viewRadius;
    public Agent[] agents;
    
}
