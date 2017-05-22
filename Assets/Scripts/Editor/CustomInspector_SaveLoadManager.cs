using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ShadyPixel.SaveLoad
{
    [CustomEditor(typeof(SaveLoadManager))]
    public class CustomInspector_SaveLoadManager : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            SaveLoadManager myScript = (SaveLoadManager)target;

            //myScript.keyName = EditorGUILayout.TextField("Key Name", myScript.keyName);
            //myScript.keyValue = EditorGUILayout.TextField("Key Value", myScript.keyValue);

            if (GUILayout.Button("Save Game"))
            {
                myScript.SaveGame();
            }

            if (GUILayout.Button("Load Game"))
            {
                myScript.LoadGame();
            }

            if (GUILayout.Button("Delete Save"))
            {
                myScript.DeleteSave();
            }
        }


    }
}
