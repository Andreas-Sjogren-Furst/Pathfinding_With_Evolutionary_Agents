using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;

public class PrintViewModel
{
    [JsonProperty("map")]
    public MapObject[,] map;

    [JsonProperty("agents")]
    public Agent[] agents;

    [JsonProperty("hpaGraph")]
    public IGraphModel hpaGraph;

    [JsonProperty("mmasGraph")]
    public Graph mmasGraph;

    [JsonProperty("checkpoints")]
    public List<CheckPoint> checkPoints;

    [JsonProperty("spawnPoint")]
    public AgentSpawnPoint spawnPoint;

    public PrintViewModel(MapObject[,] map, Agent[] agents, IGraphModel hpaGraph, Graph mmasGraph, List<CheckPoint> checkPoints, AgentSpawnPoint spawnPoint){
        this.map = map;
        this.agents = agents;
        this.hpaGraph = hpaGraph;
        this.mmasGraph = mmasGraph;
        this.checkPoints = checkPoints;
        this.spawnPoint = spawnPoint;
    }
}
