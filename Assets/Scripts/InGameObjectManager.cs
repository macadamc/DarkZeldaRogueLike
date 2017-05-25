using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameObjectManager : MonoBehaviour {
    public static DefaultRNG rng;

    [HideInInspector]
    public Transform[] types;
    public List<string> ObjectTypeNames;

    public void Awake()
    {
        types = new Transform[ObjectTypeNames.Count];
        InitalizeGameObjectContainers();
        if (rng == null)
        {
            rng = new DefaultRNG(System.DateTime.Now.GetHashCode());
        }
        
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


    public static GameObject CreatePickup(string name, Vector3 Position, float MinForce, float ForceRange)
    {

        GameObject item = (GameObject)Instantiate(Resources.Load(name));
        item.transform.position = Position;
        item.transform.parent = GameManager.GM.InGameObjectManager.GetContainer("TempGameObjects").transform;

        //item.GetComponent<Coin>().amount = 1;
        item.GetComponent<Rigidbody2D>().AddForce(rng.PointOnCircle((rng.NextFloat() * ForceRange) + MinForce));
        Shadow shadow = item.GetComponent<Shadow>();
        shadow.yVel = -(.2f + rng.NextFloat() * .2f);

        return item;
    }

    public static GameObject CreatePickup(UnityEngine.Object prefab, Vector3 Position, float MinForce, float ForceRange)
    {
        GameObject item = (GameObject)Instantiate(prefab);
        item.transform.position = Position;
        item.transform.parent = GameManager.GM.InGameObjectManager.GetContainer("TempGameObjects").transform;

        //item.GetComponent<Coin>().amount = 1;
        item.GetComponent<Rigidbody2D>().AddForce(rng.PointOnCircle((rng.NextFloat() * ForceRange) + MinForce));
        Shadow shadow = item.GetComponent<Shadow>();
        shadow.yVel = -(.2f + rng.NextFloat() * .2f);

        return item;
    }
}
