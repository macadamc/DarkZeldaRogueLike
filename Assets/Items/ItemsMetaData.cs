using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class ItemsMetaData : ScriptableObject {
    //[HideInInspector]
    public List<string> activeItemKeys;
    public List<ActiveItemSO> activeItems;

    public List<string> passiveItemKeys;
    public List<PassiveItemSO> passiveItems;


    public void Awake ()
    {
        if (activeItemKeys == null)
        {
            activeItemKeys = new List<string>();
            activeItems = new List<ActiveItemSO>();
        }
        if (passiveItemKeys == null)
        {
            passiveItemKeys = new List<string>();
            passiveItems = new List<PassiveItemSO>();
        }

    }

#if UNITY_EDITOR
    public void AddItem(Item item, string Path)
    {
        string name = item.GetType().Name;

        if (item is ActiveItem)
        {
            if (activeItemKeys.Contains(name) == false)
            {
                activeItemKeys.Add(name);
                Type type = Type.GetType(name + "SO");
                ActiveItemSO wep = (ActiveItemSO)ScriptableObject.CreateInstance(type);
                wep.name = name;
                activeItems.Add(wep);

                AssetDatabase.CreateAsset(wep, Path + "/" + wep.name + ".asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        else if (item is PassiveItem)
        {
            passiveItemKeys.Add(name);
            Type type = Type.GetType(name + "SO");
            PassiveItemSO wep = (PassiveItemSO)ScriptableObject.CreateInstance(type);
            wep.name = name;
            passiveItems.Add(wep);

            AssetDatabase.CreateAsset(wep, Path + "/" + wep.name + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        EditorUtility.SetDirty(this);
    }

    public void DeleteItem(string itemName)
    {
        if (activeItemKeys.Contains(itemName))
        {
            int i = activeItemKeys.IndexOf(itemName);
            activeItemKeys.RemoveAt(i);
            activeItems.RemoveAt(i);
        }
        else if (passiveItemKeys.Contains(itemName))
        {
            int i = passiveItemKeys.IndexOf(itemName);
            passiveItemKeys.RemoveAt(i);
            passiveItems.RemoveAt(i);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.SetDirty(this);
    }
#endif
}
