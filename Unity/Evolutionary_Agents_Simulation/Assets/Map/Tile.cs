using UnityEngine;

public class Tile : MapObject
{
   public Tile(Vector2Int arrayPosition, TileConfig tileConfig) 
   {
      ArrayPosition = arrayPosition;
      Type = ObjectType.Tile;
   }
}