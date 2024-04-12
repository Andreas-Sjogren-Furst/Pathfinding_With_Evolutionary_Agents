using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnPointConfig", menuName = "Config/SpawnPoint", order = 4)]
public class SpawnPointConfig : ScriptableObject
{
    public GameObject Prefab;
}
