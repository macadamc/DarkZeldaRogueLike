using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameObjectManager : MonoBehaviour {

    [HideInInspector]
    public Transform[] types;
    public List<string> ObjectTypeNames;

    public void Awake()
    {
        types = new Transform[ObjectTypeNames.Count];
        InitalizeGameObjectContainers();
    }

    public void InitalizeGameObjectContainers()
    {
        
        for (int i = 0; i < ObjectTypeNames.Count; i++)
        {
            Transform child = gameObject.transform.FindChild(ObjectTypeNames[i]);
            if (child == null)
            {
                GameObject go = new GameObject(ObjectTypeNames[i]);
                go.transform.parent = gameObject.transform;
                types[i] = go.transform;
            }
        }
    }

    public void DestroyAllGameObjects ()
    {
        foreach(Transform t in types)
        {
            for(int i = 0; i < t.childCount; i++)
            {
                Destroy(t.GetChild(i).gameObject);
            }
        }
    }

    public GameObject GetContainer (string name)
    {
        int i = ObjectTypeNames.IndexOf(name);
        if (i >= 0)
        {
            return types[i].gameObject;
        }
        else
        {
            return null;
        }
        
    }
}
