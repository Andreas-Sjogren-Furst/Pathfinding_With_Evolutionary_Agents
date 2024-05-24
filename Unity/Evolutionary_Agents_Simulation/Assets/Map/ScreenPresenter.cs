using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenPresenter : IGamePresenter
{
    MapModel mapModel;
    AgentModel agentModel;
    IGraphModel graphModel;
    public ScreenPresenter(MyGameManager myGameManager)
    {
        mapModel = myGameManager.mapController.mapModel;
        agentModel = myGameManager.agentController.agentModel;
        graphModel = myGameManager.HPAGraphController._graphModel;
    }

    public ScreenViewModel PackageData()
    {
        ScreenViewModel screenViewModel = new(mapModel.map, agentModel.agents, graphModel, mapModel.checkPoints, mapModel.spawnPoint);
        return screenViewModel;
    }
}
