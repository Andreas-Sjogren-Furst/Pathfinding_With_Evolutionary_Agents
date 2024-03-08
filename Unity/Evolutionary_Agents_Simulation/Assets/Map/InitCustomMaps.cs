using System.Collections.Generic;

public static class InitCustomMaps
{
    private static Dictionary<string, int> InitializePreDefinedParameters(int mapNumber)
    {
        Dictionary<string, int> mapParameters = new Dictionary<string, int>();

        if (mapNumber == 1)
        {
            mapParameters.Add("Density", 0);
            mapParameters.Add("NumberOfCheckPoints", 1);
            mapParameters.Add("TileSize", 1);
            mapParameters.Add("CellularIterations", 5);
            mapParameters.Add("CheckPointSpacing", 10);
            mapParameters.Add("ErosionLimit", 4);
            mapParameters.Add("RandomSeed", 42);
        }
        else if (mapNumber == 2)
        {
            mapParameters.Add("Density", 56);
            mapParameters.Add("NumberOfCheckPoints", 1);
            mapParameters.Add("TileSize", 1);
            mapParameters.Add("CellularIterations", 14);
            mapParameters.Add("CheckPointSpacing", 10);
            mapParameters.Add("ErosionLimit", 4);
            mapParameters.Add("RandomSeed", 9);
        }
        else if (mapNumber == 3)
        {
            mapParameters.Add("Density", 52);
            mapParameters.Add("NumberOfCheckPoints", 1);
            mapParameters.Add("TileSize", 2);
            mapParameters.Add("CellularIterations", 2);
            mapParameters.Add("CheckPointSpacing", 10);
            mapParameters.Add("ErosionLimit", 4);
            mapParameters.Add("RandomSeed", 4);
        }
        else if (mapNumber == 4)
        {
            mapParameters.Add("Density", 30);
            mapParameters.Add("NumberOfCheckPoints", 4);
            mapParameters.Add("TileSize", 1);
            mapParameters.Add("CellularIterations", 5);
            mapParameters.Add("CheckPointSpacing", 4);
            mapParameters.Add("ErosionLimit", 4);
            mapParameters.Add("RandomSeed", 87);
        }
        else if (mapNumber == 5)
        {
            mapParameters.Add("Density", 20);
            mapParameters.Add("NumberOfCheckPoints", 5);
            mapParameters.Add("TileSize", 1);
            mapParameters.Add("CellularIterations", 6);
            mapParameters.Add("CheckPointSpacing", 5);
            mapParameters.Add("ErosionLimit", 4);
            mapParameters.Add("RandomSeed", 109);
        }
        else if (mapNumber == 6)
        {
            mapParameters.Add("Density", 10);
            mapParameters.Add("NumberOfCheckPoints", 6);
            mapParameters.Add("TileSize", 1);
            mapParameters.Add("CellularIterations", 7);
            mapParameters.Add("CheckPointSpacing", 6);
            mapParameters.Add("ErosionLimit", 4);
            mapParameters.Add("RandomSeed", 131);
        }
        else if (mapNumber == 7)
        {
            mapParameters.Add("Density", 0);
            mapParameters.Add("NumberOfCheckPoints", 7);
            mapParameters.Add("TileSize", 1);
            mapParameters.Add("CellularIterations", 8);
            mapParameters.Add("CheckPointSpacing", 7);
            mapParameters.Add("ErosionLimit", 4);
            mapParameters.Add("RandomSeed", 153);
        }
        else if (mapNumber == 8)
        {
            mapParameters.Add("Density", -10);
            mapParameters.Add("NumberOfCheckPoints", 8);
            mapParameters.Add("TileSize", 1);
            mapParameters.Add("CellularIterations", 9);
            mapParameters.Add("CheckPointSpacing", 8);
            mapParameters.Add("ErosionLimit", 4);
            mapParameters.Add("RandomSeed", 175);
        }
        else if (mapNumber == 9)
        {
            mapParameters.Add("Density", -20);
            mapParameters.Add("NumberOfCheckPoints", 9);
            mapParameters.Add("TileSize", 1);
            mapParameters.Add("CellularIterations", 10);
            mapParameters.Add("CheckPointSpacing", 9);
            mapParameters.Add("ErosionLimit", 4);
            mapParameters.Add("RandomSeed", 197);
        }
        else if (mapNumber == 10)
        {
            mapParameters.Add("Density", -30);
            mapParameters.Add("NumberOfCheckPoints", 10);
            mapParameters.Add("TileSize", 1);
            mapParameters.Add("CellularIterations", 11);
            mapParameters.Add("CheckPointSpacing", 10);
            mapParameters.Add("ErosionLimit", 4);
            mapParameters.Add("RandomSeed", 219);
        }


        return mapParameters;
    }

    public static Dictionary<string, int> GetMapParameters(int mapNumber)
    {
        return InitializePreDefinedParameters(mapNumber);
    }
}
