using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGamePresenter
{
    void SetViewModel(MapModel mapModel, AgentModel agentModel, GraphModel graphModel);
}
