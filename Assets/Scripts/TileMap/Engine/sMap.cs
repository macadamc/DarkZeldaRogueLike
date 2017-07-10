using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;
using System.IO;
using ClipperLib;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class sMap : ScriptableObject
{
    [HideInInspector]
    public int[] data;

    public int width;
    public int height;
    public int chunkSize;

    [HideInInspector]
    public ChunkManager cManager;

    [HideInInspector]
    public List<string> layerPointers;
    [HideInInspector]
    public List<string> tilesetPointers;

    public List<Layer> layerConfig;
    public List<Tileset> tilesetConfig;

    [HideInInspector]
    public int nextTileID;

     public sMap()
    {
        if (tilesetPointers == null)
        {
            tilesetPointers = new List<string>();
            tilesetConfig = new List<Tileset>();
        }

        if (layerPointers == null)
        {
            layerPointers = new List<string>();
            layerConfig = new List<Layer>();
        }
    }

    public int this[string layer, int x, int y]
    {
        get
        {
            if (inBounds(x,y))
            {
                return data[x + (y * height) + _getLayerStartIndex(layer)];
            }
            else
            {
                throw new System.Exception("tried to get a value outside the map bounds..");
            }
        }
        set
        {
            if (inBounds(x, y))
            {
                int i = x + (y * height) + _getLayerStartIndex(layer);

                if (data[i] != value)
                {
                    data[i] = value;
                    cManager.Dirty(cManager.GetChunkFromWorldPos(x, y, this), layer);
                }
                
            }
            else
            {
                throw new System.Exception("Out Of Bounds");
            }

        }
    }

    public int _getLayerStartIndex(string name)
    {
        int i = layerPointers.IndexOf(name);
        if (i == -1) { throw new System.Exception(string.Format ("No Layer named {0}", name)); }
        return i * (width * height);

    }

    public void Initalize ()
    {
        data = new int[layerConfig.Count * (width * height)];
    }
    public void SetLayerTo(string Layer, int tileID)
    {
        int startIndex = _getLayerStartIndex(Layer);
        int length = width * height;

        for (int i = 0; i < length; i++)
        {
            data[startIndex + i] = tileID;
        }
    }

    public bool isEdge(int x, int y)
    {
        return x == 0 || y == 0 || x == width - 1 || y == height - 1;
    }
    public bool inBounds (int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public void ClearData ()
    {
        data = new int[(width * height) * layerPointers.Count];
    }
}


