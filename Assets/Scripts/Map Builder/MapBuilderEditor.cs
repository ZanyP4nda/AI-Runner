using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapBuilder))]
public class MapBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MapBuilder mapBuilder = (MapBuilder)target;
        if(GUILayout.Button("Save Map"))
        {
            mapBuilder.SaveMap();
        }
    }
}