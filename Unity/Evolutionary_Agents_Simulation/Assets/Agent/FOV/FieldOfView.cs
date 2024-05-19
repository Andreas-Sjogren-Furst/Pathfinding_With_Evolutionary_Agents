using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FieldOfView
{
    
    public List<Vector2Int> ComputeFOV(Vector2Int origin, MapObject[,] map){
        List<Vector2Int> markVisible = new(){origin};

        foreach (Quadrant.Cardinal cardinal in Enum.GetValues(typeof(Quadrant.Cardinal))){
            Quadrant quadrant = new(cardinal, origin);
            Row firstRow = new(1, new Fraction(-1), new Fraction(1));
            markVisible.AddRange(Scan(firstRow, map));
        } return markVisible;
        
    }
    public List<Vector2Int> Scan(Row row, MapObject[,] map) {
        List<Vector2Int> markVisible = new();
        Stack<Row> rows = new();
        rows.Append(row);
        while(rows.Count != 0){
            row = rows.Pop();
            MapObject prevTile = null;
            foreach(MapObject tile in row.GetTiles(map)){   
                if(tile.Type == MapObject.ObjectType.Wall || IsSymmetric(row, tile)){
                    markVisible.Add(new Vector2Int(tile.ArrayPosition.x,tile.ArrayPosition.y));
                }
                if(prevTile.Type == MapObject.ObjectType.Wall && tile.Type == MapObject.ObjectType.Tile){
                    row.startSlope = Slope(tile);
                }
                if(prevTile.Type == MapObject.ObjectType.Tile && tile.Type == MapObject.ObjectType.Wall){
                    Row nextRow = row.Next();
                    nextRow.endSlope = Slope(tile);
                    rows.Append(nextRow);
                } prevTile = tile;
            }

            if(prevTile.Type == MapObject.ObjectType.Tile) rows.Append(row.Next());

        } return markVisible;
    }

    private void Reveal(Vector2Int tile, Quadrant quadrant, ref List<Vector2Int> markVisible){
        Vector2Int absolutGridPosition = quadrant.Transform(tile);
        markVisible.Add(absolutGridPosition);
    }

    private void IsWall(Vector2Int tile, Quadrant quadrant, ref List<Vector2Int> isBlocking){
        if(tile == null) return;
        Vector2Int absolutGridPosition = quadrant.Transform(tile);
        isBlocking.Add(absolutGridPosition);
    }

    private void IsFloor(Vector2Int tile, Quadrant quadrant, ref List<Vector2Int> isNotBlocking){
        if(tile == null) return;
        Vector2Int absolutGridPosition = quadrant.Transform(tile);
        isNotBlocking.Add(absolutGridPosition);
    }

    public Fraction Slope(MapObject tile){
        int rowDepth = tile.ArrayPosition.x;
        int col = tile.ArrayPosition.y;
        return new Fraction(2 * col - 1, 2 * rowDepth);
    }
    
    public bool IsSymmetric(Row row, MapObject tile){
        int rowDepth = tile.ArrayPosition.x;
        int col = tile.ArrayPosition.y;
        return col >= row.depth * row.startSlope.EvaluateFraction() && col <= row.depth * row.endSlope.EvaluateFraction();
    }
}
