using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class Row
{
    public readonly int depth;
    public Fraction startSlope;
    public Fraction endSlope;

    public Row(int depth, Fraction startSlope, Fraction endSlope){
        this.depth = depth;
        this.startSlope = startSlope;
        this.endSlope = endSlope;
    }

    public List<MapObject> GetTiles(MapObject[,] map){
        int minCol = RoundTiesUp(depth * startSlope.EvaluateFraction());
        int maxCol = RoundTiesDown(depth * endSlope.EvaluateFraction());
        List<MapObject> tiles = new();
        for(int col = minCol; col <= maxCol; col++){
            tiles.Add(map[depth,col]);
        } return tiles;
    }

    public Row Next(){
        return new Row(depth + 1, startSlope, endSlope);
    }
    
    private int RoundTiesUp(float number){
        return (int)Math.Floor(number + 0.5);
    }

    private int RoundTiesDown(float number){
        return (int)Math.Ceiling(number - 0.5);
    }
}
