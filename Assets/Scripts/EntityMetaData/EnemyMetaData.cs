using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyMetaData
{
    [HideInInspector]
    public string name;
    public int firstLvl;
    public int cost;
    public GameObject prefab;

}