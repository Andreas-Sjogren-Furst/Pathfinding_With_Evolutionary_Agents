
public class PrintPresenter
{
    PrintViewModel printViewModel;

    public PrintPresenter(MyGameManager myGameManager){
        printViewModel = new(
            myGameManager.mapController.mapModel.map,
            myGameManager.agentController.agentModel.agents,
            myGameManager.HPAGraphController._graphModel,
            myGameManager.mmasGraphController._graph,
            myGameManager.mapController.mapModel.checkPoints,
            myGameManager.mapController.mapModel.spawnPoint
        );
    }

    
}
