using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(OptionsManager))]
[CanEditMultipleObjects]
public class CustomInspector_OptionsManager : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        OptionsManager OM = (OptionsManager)target;

        if (GUILayout.Button("Save Options"))
        {
            GameManager.GM.optionsManager.SaveOptions();
        }
    }



}
