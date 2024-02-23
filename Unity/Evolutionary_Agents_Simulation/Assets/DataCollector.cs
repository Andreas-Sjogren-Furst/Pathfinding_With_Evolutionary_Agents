using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class DataCollector : MonoBehaviour


{
    // Get the reference to the colonies. 
    private ObjectDropper objectDropper;

    // Boolean to collect data. 

    public bool collectData = false;


    // Start is called before the first frame update
    void Start()
    {
        objectDropper = gameObject.GetComponent<ObjectDropper>();


    }

    // Update is called once per frame
    void Update()
    {
        if (collectData)
        {
            CollectData();
            collectData = false;
        }

    }


    public void CollectData()
    {
        Data data = new Data();

        var colonies = objectDropper.GetColonies();

        foreach (var colonyObj in colonies)
        {
            ColonyBehavior colonyBehavior = colonyObj.GetComponent<ColonyBehavior>();
            var ants = colonyBehavior.GetAnts(); // Assuming GetAnts returns a List<GameObject> of ants

            ColonyData colonyData = new ColonyData
            {
                AntCount = ants.Count,
                position = colonyObj.transform.position
            };


            foreach (var antObj in ants)
            {
                Movement antMovement = antObj.GetComponent<Movement>();
                colonyData.Ants.Add(new AntData { Steps = antMovement.GetTotalSteps() });
            }

            data.Colonies.Add(colonyData);
        }

        foreach (var checkpointObj in objectDropper.GetCheckpoints())
        {
            data.CheckPoints.Add(new CheckPointData
            {
                FoodCount = 0,
                position = checkpointObj.transform.position
            });
        }


        string jsonData = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath, "data_" + System.DateTime.Now.ToString("yyyy-dd-MM_HH-mm-ss") + ".json");
        File.WriteAllText(path, jsonData);

        Debug.Log($"Data exported to {path}");



    }
}
