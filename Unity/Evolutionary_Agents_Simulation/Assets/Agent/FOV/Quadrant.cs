using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quadrant
{
    public enum Cardinal{
        north,
        east,
        south,
        west
    }
    private readonly Vector2Int origin;
    private readonly Cardinal cardinal;
    public Quadrant(Cardinal cardinal, Vector2Int origin){
        this.cardinal = cardinal;
        this.origin = origin;
    }

    public Vector2Int Transform(Vector2Int tile){
        int row = tile.x;
        int col = tile.y;
        if(cardinal == Cardinal.north) return new Vector2Int(origin.x + col, origin.y - row);
        if(cardinal == Cardinal.south) return new Vector2Int(origin.x + col, origin.y + row);
        if(cardinal == Cardinal.east) return new Vector2Int(origin.x + row, origin.y + col);
        if(cardinal == Cardinal.west) return new Vector2Int(origin.x - row, origin.y + col);
        throw new System.Exception("Invalid cardinal value");
    }

}
