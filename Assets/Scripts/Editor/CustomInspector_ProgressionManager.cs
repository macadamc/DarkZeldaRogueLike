using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ShadyPixel.CameraSystem
{
    [CustomEditor(typeof(ProgressionManager))]
    public class CustomInspector_ProgressionManager : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            ProgressionManager myScript = (ProgressionManager)target;

            //myScript.keyName = EditorGUILayout.TextField("Key Name", myScript.keyName);
            //myScript.keyValue = EditorGUILayout.TextField("Key Value", myScript.keyValue);

            if (GUILayout.Button("Add/Set (Key: "+myScript.keyName+") | (Value: "+myScript.keyValue+")") && myScript.keyName.Length > 0 && myScript.keyValue.Length > 0)
            {
                myScript.gameProgression[myScript.keyName] = myScript.keyValue;
                myScript.ResetFields();
            }
            if (GUILayout.Button("Remove (Key: " + myScript.keyName + ")") && myScript.keyName.Length > 0)
            {
                myScript.gameProgression.RemoveKeyValuePair(myScript.keyName);
                myScript.ResetFields();
            }
            if (GUILayout.Button("Clear All Key Value Pairs"))
            {
                myScript.gameProgression.ClearAll();
                myScript.ResetFields();
            }
        }


    }
}
