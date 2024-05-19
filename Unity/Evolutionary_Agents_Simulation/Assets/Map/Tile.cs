using UnityEngine;

public class Tile : MapObject
{
   public Tile(Vector2Int arrayPosition) 
   {
      ArrayPosition = arrayPosition;
      Type = ObjectType.Tile;
   }
}