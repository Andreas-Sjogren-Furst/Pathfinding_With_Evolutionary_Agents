using System.Collections.Generic;
// Written by: Andreas Sjögren Fürst (s201189)
public class FieldOfView
{
    public List<Point> markVisibleTiles;
    public List<Point> markVisibleWalls;
    private readonly MapObject[,] map;
    public FieldOfView(MapObject[,] map){
        this.map = map;
        markVisibleTiles = new();
        markVisibleWalls = new();
    }
    public void ComputeFOV(Point origin){
        markVisibleTiles = new(){origin};
        markVisibleWalls = new();
        for(int i = 0; i < 4; i++){
            Quadrant quadrant = new(i, origin);
            Row firstRow = new(1, new Fraction(-1), new Fraction(1));
            ScanIterative(firstRow, quadrant);
        }
    }
    
    private void ScanIterative(Row row, Quadrant quadrant) {
        Stack<Row> rows = new();
        rows.Push(row);
        while(rows.Count != 0){
            row = rows.Pop();
            Point prevTile = null;
            foreach(Point tile in row.Tiles()){ 
                if(IsWall(tile, quadrant) || IsSymmetric(row, tile)){
                    Reveal(tile,quadrant);
                }
                if(IsWall(prevTile,quadrant) && IsFloor(tile,quadrant)){
                    row.startSlope = Slope(tile);
                }
                if(IsFloor(prevTile, quadrant) && IsWall(tile, quadrant)){
                    Row nextRow = row.Next();
                    nextRow.endSlope = Slope(tile);
                    rows.Push(nextRow);
                } prevTile = tile;
            }
            if(IsFloor(prevTile, quadrant)){
                rows.Push(row.Next());
            } 
    
        }
    }

    private void Reveal(Point tile, Quadrant quadrant){
        if(tile == null || !IsTileExisting(tile,quadrant)) return;
        Point absolutGridPosition = quadrant.Transform(tile);
        if(map[absolutGridPosition.x,absolutGridPosition.y].Type == MapObject.ObjectType.Tile){
            markVisibleTiles.Add(absolutGridPosition);
        } else markVisibleWalls.Add(absolutGridPosition);        
    }

    private bool IsWall(Point tile, Quadrant quadrant){
        if(tile == null || !IsTileExisting(tile,quadrant)) return false;
        Point absolutGridPosition = quadrant.Transform(tile);
        MapObject mapObject = map[absolutGridPosition.x,absolutGridPosition.y];
        if(mapObject.Type == MapObject.ObjectType.Wall){
            return true;
        } else return false;    
    }

    private bool IsFloor(Point tile, Quadrant quadrant){
        if(tile == null || !IsTileExisting(tile, quadrant)) return false;
        Point absolutGridPosition = quadrant.Transform(tile);
        MapObject mapObject = map[absolutGridPosition.x,absolutGridPosition.y];
        if(mapObject.Type == MapObject.ObjectType.Tile) return true;
        else return false;
    }

    public Fraction Slope(Point tile){
        int rowDepth = tile.x;
        int col = tile.y;
        return new Fraction(2 * col - 1, 2  * rowDepth);
    }
    
    public bool IsSymmetric(Row row, Point tile){
        int col = tile.y;
        return col >= row.depth * row.startSlope.EvaluateFraction() && col <= row.depth * row.endSlope.EvaluateFraction();
    }

    private bool IsTileExisting(Point tile, Quadrant quadrant){
        Point globalTile = quadrant.Transform(tile);
        if(globalTile.x >= 0 && globalTile.x < map.GetLength(0) && globalTile.y >= 0 && globalTile.y < map.GetLength(1)){
            return true;
        } else return false;
    }
}

//private void ScanRecursive(Row row, Quadrant quadrant){
    //     Point prevTile = null;
    //     foreach(Point tile in row.Tiles()){
    //         if(IsWall(tile, quadrant) || IsSymmetric(row, tile))
    //             Reveal(tile, quadrant);
    //         if(IsWall(prevTile, quadrant) && IsFloor(tile, quadrant))
    //             row.startSlope = Slope(tile);
    //         if(IsFloor(prevTile,quadrant) && IsWall(tile, quadrant)){
    //             Row nextRow = row.Next();
    //             nextRow.endSlope = Slope(tile);
    //             ScanRecursive(nextRow, quadrant);
    //         }
    //         prevTile = tile;
    //     }
    //     if(IsFloor(prevTile, quadrant))
    //         ScanRecursive(row.Next(), quadrant);
    // }