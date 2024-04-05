using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectRemover : MonoBehaviour
{
    public static void ClearObjects(List<string> tagArray, MapObject[,] Map){

        foreach (string tag in tagArray){
            DestroyObjects(GameObject.FindGameObjectsWithTag(tag));
        } 
    }

    private static void DestroyObjects(GameObject[] objects){
        foreach (GameObject obj in objects){
            Destroy(obj);
        }
    }
}
