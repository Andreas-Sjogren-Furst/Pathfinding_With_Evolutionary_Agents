using UnityEngine;

public class Tile : MapObject
{

   public Tile(Vector2Int arrayPosition) : 
   base(arrayPosition)
   {
      Type = ObjectType.Tile;
      Prefab = AssetManager.tilePrefab;
      Tag = AssetManager.tilePrefab.tag;
   }
}