using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Entity))]
[CanEditMultipleObjects]
public class CustomInspector_Entity : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Entity entity = (Entity)target;

        if (GUILayout.Button("Hurt (1 Damage)"))
        {
            entity.ModifyHealth(-1.0f);
        }
    }



}
