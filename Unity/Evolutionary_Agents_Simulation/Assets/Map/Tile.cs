using UnityEngine;
// Written by: Andreas Sjögren Fürst (s201189)
public class Tile : MapObject
{
   public Tile(Vector2Int arrayPosition) 
   {
      ArrayPosition = arrayPosition;
      Type = ObjectType.Tile;
   }
}