using UnityEngine;

public class Tile : MapObject
{
   public Tile Create(Vector2Int arrayPosition, TileConfig tileConfig)
   {

      Type = ObjectType.Tile;
      ArrayPosition = arrayPosition;
      Object = Instantiate(tileConfig.Prefab, ConvertArrayToMap(arrayPosition), Quaternion.identity);
      Tile tileComponent = Object.AddComponent<Tile>();
      return tileComponent;
   }


}