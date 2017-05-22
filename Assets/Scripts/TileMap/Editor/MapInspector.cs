using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(sMap))]
public class MapInspector : Editor {

    public override void OnInspectorGUI()
    {
        sMap Map = (sMap)target;
        DrawDefaultInspector();

        if (GUILayout.Button("Initalize Tilesets."))
        {
            Debug.Log("Tilesets Initalized!!");

            Map.Initalize();
            Map.nextTileID = 1;
            foreach (Layer l in Map.layerConfig)
            {
                if (Map.layerPointers.Contains(l.name)) { continue; }
                Map.layerPointers.Add(l.name);
            }

            foreach (Tileset t in Map.tilesetConfig)
            {
                t.Initalize(Map);
            }
        }
    }
}
