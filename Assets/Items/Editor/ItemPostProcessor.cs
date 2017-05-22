using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

public class ItemPostProcessor : AssetPostprocessor
{

    public static string watchPath = "/Items";

    public static string fileName = "ItemsMetaData.asset";
    public static string filePath = "Assets/" + fileName;
    public static Assembly asm = Assembly.Load("Assembly-CSharp");


    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string str in importedAssets)
        {
            if (str.Contains(watchPath) && !str.Contains(fileName)) { OnImport(str); }
        }

        for (int i = 0; i < movedAssets.Length; i++)
        {
            if (movedAssets[i].Contains(watchPath) && !movedAssets[i].Contains(fileName))
            {
                OnImport(movedAssets[i]);
            }

        }

        foreach (string str in deletedAssets)
        {
            if (str.Contains(watchPath) && !str.Contains(fileName)) { OnDeleted(str); }

        }

        for (int i = 0; i < movedFromAssetPaths.Length; i++)
        {
            if (movedFromAssetPaths[i].Contains(watchPath) && !movedFromAssetPaths[i].Contains(fileName))
            {
                OnDeleted(movedFromAssetPaths[i]);
            }

        }

    }
    static void OnImport(string Path)
    {
        if (System.IO.Path.GetFileName(Path).EndsWith(".cs") == false) { return; }
        ItemsMetaData ItemData = GetItemMetaData();

        string typeName = System.IO.Path.GetFileNameWithoutExtension(Path);
        Item i = null;
        try
        {
            i = (Item)asm.CreateInstance(typeName);
        }
        catch
        {
            //Debug.Log("CreateInstance Failed.");
        }
        
        if (i != null && i is Item)
        {
            ItemData.AddItem(i, System.IO.Path.GetDirectoryName(Path));
            EditorUtility.SetDirty(ItemData);
        }
    }
    static void OnDeleted(string Path)
    {
        if (System.IO.Path.GetFileName(Path).EndsWith(".cs") == false) { return; }
        ItemsMetaData ItemData = GetItemMetaData();
        string name = System.IO.Path.GetFileNameWithoutExtension(Path);
        ItemData.DeleteItem(name);
        EditorUtility.SetDirty(ItemData);
    }

    static ItemsMetaData GetItemMetaData()
    {
        ItemsMetaData ItemData = null;
        string[] paths = AssetDatabase.FindAssets("t:ItemsMetaData");
        if (paths.Length > 0)
        {
            ItemData = AssetDatabase.LoadAssetAtPath<ItemsMetaData>(AssetDatabase.GUIDToAssetPath(paths[0]));
        }
        if (ItemData == null)
        {
            ItemData = ScriptableObject.CreateInstance<ItemsMetaData>();
            ItemData.name = "ItemsMetaData";
            AssetDatabase.CreateAsset(ItemData, filePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        return ItemData;
    }
}


