using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomata : MonoBehaviour
{

    public GameObject MapWall;
    public int MapSize;
    public int tileSize;
    
    private Vector3 position;
    private int mapTileAmount;
   


    // Start is called before the first frame update
    void Start()
    {
        int[,] Map = init2dMap(MapSize, tileSize);

        Debug.Log(mapTileAmount);


        for (int i = 0; i < mapTileAmount; i++)
        {

            for (int j = 0; j < mapTileAmount; j++)
            {
                if (Map[i,j] == 0)
                {
                    Instantiate(MapWall, new Vector3(i, 0, j), transform.rotation);


                }
                else
                {


                }


            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    int[,] init2dMap(int MapSize, int tileSize)
    {
        mapTileAmount = MapSize / tileSize;

        return new int[mapTileAmount, mapTileAmount];
    }

}
