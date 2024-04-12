using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "TileConfig", menuName = "Config/Tile", order = 1)]
public class TileConfig : ScriptableObject
{
    public GameObject Prefab;
}
