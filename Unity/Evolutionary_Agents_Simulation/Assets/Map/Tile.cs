using UnityEngine;

public class Tile : MapObject
{
   public Tile Create(Vector2Int arrayPosition, TileConfig tileConfig) 
   {
      ArrayPosition = arrayPosition;
      Object = Instantiate(tileConfig.Prefab, ConvertArrayToMap(arrayPosition), Quaternion.identity);
      Tile tileComponent = Object.AddComponent<Tile>();
      return tileComponent;
   }
}