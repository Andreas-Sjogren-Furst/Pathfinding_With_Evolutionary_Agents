using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenPresenter : IGamePresenter
{
    private ScreenViewModel screenViewModel;
    ScreenPresenter(ScreenViewModel screenViewModel){
        this.screenViewModel = screenViewModel;
    }
    public void SetViewModel(MapModel mapModel, AgentModel agentModel, GraphModel graphModel){
        screenViewModel.Map = mapModel.Map;
        screenViewModel.Agents = agentModel.agents;
        screenViewModel.graph = graphModel;
    }

    public ScreenViewModel ReturnViewModel(){
        return screenViewModel;
    }
}
