using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMapTest : MonoBehaviour {

    public TextAsset MapFile;
    public Map map;

    void Start ()
    {
        map = Map.Load(MapFile);
    }
}
