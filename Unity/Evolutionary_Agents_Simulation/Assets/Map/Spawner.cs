using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spawner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MapModel.Map
    }

    void CreateMap(){

    }    

    void OnValidate(){
        
    }


    private void PlaceObjects(ref MapObject[,] map, int width, int height){
        for(int i = 0; i < width; i++){
            for(int j = 0; j < height; j++){
                
            }
        }
    }
   
}
