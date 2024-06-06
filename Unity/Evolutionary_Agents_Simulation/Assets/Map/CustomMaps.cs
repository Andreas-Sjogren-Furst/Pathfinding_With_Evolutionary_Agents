
public class CustomMaps
{

    public static MapModel[] customMaps = {
                                        new (0,0,20,5,1,0), // 0
                                        new (20,4,40,5,1,0),  // 1
                                        new (25,5,100,5,3,0), // 2
                                        new (75,8,50,5,5,0), // 3
                                        new (75,6,100,5,3,0), // 4 
                                        new (50,9,100,5,7,0), // 5
                                        new (60,19,100,20,1,0), // 6
                                        new (50,2,100,5,1,0), // 7
                                        new (60,20,100,40,1,0), // 8
                                        new (60,10,100,1,1,0), // 9
                                        new (50,5,100,1,1,0), // 10
                                        new (52,2,4,1,1,0), // 11
                                        new (30,5,87,4,1,0), // 12 
                                        new (20,6,109,5,1,0) // 13
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
