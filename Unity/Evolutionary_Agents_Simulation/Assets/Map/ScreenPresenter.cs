using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenPresenter : IGamePresenter<ScreenViewModel>
{
    public ScreenViewModel screenViewModel;
    public ScreenPresenter(MyGameManager myGameManager){
        screenViewModel = new ScreenViewModel(
            myGameManager.mapController.mapModel.accessibleNodes,
            myGameManager.agentController.agentModel.visibleTiles,
            myGameManager.mapController.mapModel.map,
            myGameManager.agentController.agentModel.agents,
            myGameManager.HPAGraphController._graphModel,
            myGameManager.mmasGraphController._graph,
            myGameManager.mapController.mapModel.checkPoints,
            myGameManager.mapController.mapModel.spawnPoint
        );
    }
    
    public ScreenViewModel PackageData(){
        return screenViewModel;
    }
}
