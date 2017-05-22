using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ShadyPixel.CameraSystem
{
    [CustomEditor(typeof(PrefabSpawner))]
    public class CustomInspector_PrefabSpawner : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            PrefabSpawner myScript = (PrefabSpawner)target;

            if (myScript.spawnType == SpawnType.Distance)
                myScript.spawnDistance = EditorGUILayout.FloatField("Distance To Spawn At", myScript.spawnDistance);

            if (myScript.spawnType == SpawnType.EnterZone)
                myScript.cameraZone = EditorGUILayout.ObjectField("Camera Zone", myScript.cameraZone, typeof(Zone), true) as Zone;

            if (myScript.spawnType == SpawnType.OnMessageReceived)
                myScript.waitForMessage = EditorGUILayout.TextField("Wait For Message", myScript.waitForMessage);

            if (GUILayout.Button("Force Spawn"))
            {
                myScript.Spawn();
            }
            if (GUILayout.Button("Force Despawn"))
            {
                myScript.Despawn();
            }
        }


    }
}
