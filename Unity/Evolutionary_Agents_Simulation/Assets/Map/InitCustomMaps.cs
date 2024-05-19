using System;
using System.Collections.Generic;

public class InitCustomMaps
{
    public readonly int density;
    public readonly int numberOfCheckPoints;
    public readonly int cellularIterations;
    public readonly int width;
    public readonly int height;
    public readonly int randomSeed;
    public readonly int size = 100;
    private readonly Random rand = new();

    public InitCustomMaps(int density, int numberOfCheckPoints, int cellularIterations, int height, int width)
    {
        this.density = density;
        this.numberOfCheckPoints = numberOfCheckPoints;
        this.cellularIterations = cellularIterations;
        this.width = width;
        this.height = height;
        randomSeed = rand.Next();
    }

    /*
    ### Open Square Maps ###
    InitCustomMaps(0,5,0,50,50)
    InitCustomMaps(0,5,0,100,100)
    InitCustomMaps(0,5,0,500,500)

    ### Square Maps ###
    InitCustomMaps(50,5,1,50,50)
    InitCustomMaps(50,5,5,50,50)
    InitCustomMaps(50,5,10,50,50)

    InitCustomMaps(65,5,1,50,50)
    InitCustomMaps(65,5,5,50,50)
    InitCustomMaps(65,5,10,50,50)

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
    InitCustomMaps(50,5,1,1000,1000)
    InitCustomMaps(50,5,5,5,800)
    InitCustomMaps(50,5,10,400,400)

    InitCustomMaps(65,5,1,15,85)
    InitCustomMaps(65,5,5,15,85)
    InitCustomMaps(65,5,10,15,85)
    */
}
