using System;
using System.Collections.Generic;


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

    public List<Point> Tiles(){
        int minCol = RoundTiesUp(depth * startSlope.EvaluateFraction());
        int maxCol = RoundTiesDown(depth * endSlope.EvaluateFraction());
        List<Point> tilesRelativePositions = new();
        for(int col = minCol; col <= maxCol; col++){
            tilesRelativePositions.Add(new Point(depth,col));
        } return tilesRelativePositions;
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
