using UnityEngine;
using UnityEditor;
using System.IO;

public class EntityPostProcessor : AssetPostprocessor
{
    public static string watchPath = "/Resources/";

    public static string[] WatchFolders = new string[2]
    {
        "Enemys",
        "Passives"
    };

    public static string fileName = "EntityMetaData.asset";
    public static string filePath = "Assets/GameData/" + fileName;


    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string str in importedAssets)
        {
            if (IsWatchedPath(str) && !str.Contains(fileName)) { OnImport(str); }

        }

        for (int i = 0; i < movedAssets.Length; i++)
        {
            if (IsWatchedPath(movedAssets[i]) && !movedAssets[i].Contains(fileName))
            {
                OnImport(movedAssets[i]);
            }

        }

        foreach (string str in deletedAssets)
        {
            if (IsWatchedPath(str) && !str.Contains(fileName)) { OnDeleted(str); }

        }

        for (int i = 0; i < movedFromAssetPaths.Length; i++)
        {
            if (IsWatchedPath(movedFromAssetPaths[i]) && !movedFromAssetPaths[i].Contains(fileName))
            {
                OnDeleted(movedFromAssetPaths[i]);
            }

        }

    }
    static void OnImport(string Path)
    {
        EntityMetaDataSO enemyMetaData = AssetDatabase.LoadAssetAtPath<EntityMetaDataSO>(filePath);
        if (enemyMetaData == null)
        {
            enemyMetaData = ScriptableObject.CreateInstance<EntityMetaDataSO>();
            enemyMetaData.name = "EntityMetaData";
            AssetDatabase.CreateAsset(enemyMetaData, filePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        if (enemyMetaData.enemysData == null)
        {
            enemyMetaData.enemysData = new System.Collections.Generic.List<EnemyMetaData>();
            enemyMetaData.enemyKeys = new System.Collections.Generic.List<string>();

            enemyMetaData.passivesData = new System.Collections.Generic.List<EntityMetaData>();
            enemyMetaData.passiveKeys = new System.Collections.Generic.List<string>();
        }

        foreach (string type in WatchFolders)
        {
            if (Path.Contains(type))
            {
                switch (type)
                {
                    case "Enemys":
                        CreateEnemyMetaData(Path, enemyMetaData);
                        break;
                    case "Passives":
                        CreatePassiveMetaData(Path, enemyMetaData);
                        break;
                }
            }
        }

    }
    static void OnDeleted(string Path)
    {
        foreach(string type in WatchFolders)
        {
            if (Path.Contains(type))
            {
                switch(type)
                {
                    case "Enemys":
                        DeleteEnemyMetaData(Path);
                        break;
                    case "Passives":
                        DeletePassiveMetaData(Path);
                        break;
                }
            }
        }
    }

    static bool IsWatchedPath(string str)
    {
        foreach(string folder in WatchFolders)
        {
            if (str.Contains(folder)) { return true; }
            
        }
        return false;
    }

    static void CreateEnemyMetaData(string path, EntityMetaDataSO data)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab == null) { return; }
        if (!data.enemyKeys.Contains(prefab.name))
        {
            EnemyMetaData eData = new EnemyMetaData();
            eData.name = prefab.name;
            eData.firstLvl = 1;
            eData.cost = 1;
            eData.prefab = prefab;

            data.enemysData.Add(eData);
            data.enemyKeys.Add(prefab.name);
            //AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(data);
        }
    }
    static void DeleteEnemyMetaData (string Path)
    {
        EntityMetaDataSO data = AssetDatabase.LoadAssetAtPath<EntityMetaDataSO>(filePath);
        string name = System.IO.Path.GetFileNameWithoutExtension(Path);
        if (data.enemyKeys.Contains(name))
        {
            int index = data.enemyKeys.IndexOf(name);

            data.enemyKeys.RemoveAt(index);
            data.enemysData.RemoveAt(index);
            //AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(data);
        }
    } 

    static void CreatePassiveMetaData(string path, EntityMetaDataSO data)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab == null) { return; }
        if (!data.passiveKeys.Contains(prefab.name))
        {
            EntityMetaData eData = new EntityMetaData();
            eData.name = prefab.name;
            eData.firstLvl = 1;
            eData.prefab = prefab;

            data.passivesData.Add(eData);
            data.passiveKeys.Add(prefab.name);
            //AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(data);
        }
    }
    static void DeletePassiveMetaData(string Path)
    {
        EntityMetaDataSO data = AssetDatabase.LoadAssetAtPath<EntityMetaDataSO>(filePath);
        string name = System.IO.Path.GetFileNameWithoutExtension(Path);
        if (data.passiveKeys.Contains(name))
        {
            int index = data.passiveKeys.IndexOf(name);

            data.passiveKeys.RemoveAt(index);
            data.passivesData.RemoveAt(index);
            //AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(data);
        }
    }

}