using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Layer {

    public string name;
    public bool useCollisions;
    public bool fullTileCollisions;
    public bool emptyTileCollision;
    public bool useAutoTile;
    public string unityLayer;
    //public LayerMask unityLayer;
    public string sortingLayer;
    public int sortingOrder;
    public bool isTrigger;
}
