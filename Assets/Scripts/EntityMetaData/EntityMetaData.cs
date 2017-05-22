using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntityMetaData
{
    [HideInInspector]
    public string name;
    public int firstLvl;
    public GameObject prefab;

}