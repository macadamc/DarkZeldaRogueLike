using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClipperLib;

[System.Serializable]
public class MapChunk
{
    public WorldPos pos;
    public GameObject gameobject;
    public PolygonCollider2D collider;

    public List<ChunkLayer> layerList;
    public List<string> layerPointers;

    public MapChunk(int x, int y, sMap Map)
    {
        layerList = new List<ChunkLayer>();
        layerPointers = new List<string>();

        gameobject = new GameObject(string.Format("{0},{1}", x, y));
        pos = new WorldPos(x, y);

        foreach (Layer layer in Map.layerConfig)
        {
            ChunkLayer L = new ChunkLayer(gameobject, Map, layer);
            layerList.Add(L);
            layerPointers.Add(layer.name);

        }
    }

    public ChunkLayer GetLayer(string Name)
    {
        int i = layerPointers.IndexOf(Name);
        if (i == -1) { throw new System.Exception(" No Layer Named " + Name + "in the current chunk.."); }
        return layerList[i];
    }

}
