// Written by: Andreas Sjögren Fürst (s201189)
public class PrintPresenter : IGamePresenter<PrintViewModel>
{
    PrintViewModel printViewModel;

    public PrintPresenter(MyGameManager myGameManager)
    {
        printViewModel = new(
            myGameManager.mapController.mapModel.map,
            myGameManager.agentController.agentModel.agents,
            myGameManager.HPAGraphController._graphModel,
            myGameManager.mmasGraphController._graph,
            myGameManager.mapController.mapModel.checkPoints,
            myGameManager.mapController.mapModel.spawnPoint
        );
    }

    public PrintViewModel PackageData()
    {
        return printViewModel;
    }

}
