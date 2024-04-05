using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapModel))]
public class MapModelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // Draw the default inspector

        MapModel mapModel = (MapModel)target;

        if (GUILayout.Button("Generate Map"))
        {  
            MapController.CreateMap(mapModel);
        }
    }
}
