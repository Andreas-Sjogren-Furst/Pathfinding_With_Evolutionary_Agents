using System;
using System.Collections.Generic;

public class CustomMaps
{

    MapModel[] customMaps = {      new (0,0,50,5,0),
                                   new (0,0,100,5,0),
                                   new (0,5,0,500,0),

                                   new (25,1,50,5,0),
                                   new (25,1,100,5,0),
                                   new (25,1,500,5,0),

                                   new (50,2,50,5,0),
                                   new (50,2,100,5,0),
                                   new (50,2,500,5,0),

                                   new (0,5,42,1,0),
                                   new (56,14,9,1,0),
                                   new (52,2,4,1,0),
                                   new (30,5,87,4,0),
                                   new (20,6,109,5,0)
                                   };

    public MapModel GetCustomMap(int mapNumber)
    {
        return customMaps[mapNumber];
    }


    /*
    ### Open Square Maps ###
    InitCustomMaps(0,5,0,50)
    InitCustomMaps(0,5,0,100)
    InitCustomMaps(0,5,0,500)

    ### Square Maps ###
    InitCustomMaps(50,5,1,50)
    InitCustomMaps(50,5,5,50)
    InitCustomMaps(50,5,10,50)

    InitCustomMaps(65,5,1,50)
    InitCustomMaps(65,5,5,50)
    InitCustomMaps(65,5,10,50)

    ### Rectangular Maps (easy) ###
    InitCustomMaps(50,5,1,25,75)
    InitCustomMaps(50,5,5,25,75)
    InitCustomMaps(50,5,10,25,75)

    InitCustomMaps(65,5,1,25,75)
    InitCustomMaps(65,5,5,25,75)
    InitCustomMaps(65,5,10,25,75)


    ### Rectangular Maps (medium) ###
    InitCustomMaps(50,5,1,15,85)
    InitCustomMaps(50,5,5,15,85)
    InitCustomMaps(50,5,10,15,85)

    InitCustomMaps(65,5,1,15,85)
    InitCustomMaps(65,5,5,15,85)
    InitCustomMaps(65,5,10,15,85)

    ### Extreme Maps ###
    InitCustomMaps(50,5,1,500)
    InitCustomMaps(50,5,5,800)
    InitCustomMaps(50,5,10,1000)

    InitCustomMaps(65,5,1,15,85)
    InitCustomMaps(65,5,5,15,85)
    InitCustomMaps(65,5,10,15,85)
    */
}
