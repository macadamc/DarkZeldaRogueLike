using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class ItemsMetaData : ScriptableObject {
    //[HideInInspector]
    public List<string> wepKeys;
    public List<ActiveItemSO> weapons;


    public void Awake ()
    {
        if (wepKeys == null)
        {
            wepKeys = new List<string>();
            weapons = new List<ActiveItemSO>();
        }
        
    }

#if UNITY_EDITOR
    public void AddItem(Item item, string Path)
    {
        if (item is ActiveItem)
        {
            string name = item.GetType().Name;
            if (wepKeys.Contains(name) == false)
            {
                wepKeys.Add(name);
                Type type = Type.GetType(name + "SO");
                ActiveItemSO wep = (ActiveItemSO)ScriptableObject.CreateInstance(type);
                wep.name = name;
                weapons.Add(wep);

                AssetDatabase.CreateAsset(wep, Path + "/" + wep.name + ".asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }

    public void DeleteItem(string itemName)
    {
        if (wepKeys.Contains(itemName))
        {
            int i = wepKeys.IndexOf(itemName);
            wepKeys.RemoveAt(i);
            weapons.RemoveAt(i);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.SetDirty(this);
    }
#endif
}
