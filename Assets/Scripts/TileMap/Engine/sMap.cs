using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClipperLib;

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

    public List<Layer> layerConfig;
    public List<Tileset> tilesetConfig;

    [HideInInspector]
    public int nextTileID;

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
                    MapChunk chunk = cManager.GetChunkFromWorldPos(x, y);
                    cManager.Dirty(chunk, layer);
                }
                
            }
            else
            {
                throw new System.Exception();
            }

        }
    }

    int _getLayerStartIndex(string name)
    {
        int i = layerPointers.IndexOf(name);
        if (i == -1) { throw new System.Exception(string.Format ("No Layer named {0}", name)); }
        return i * (width * height);

    }

    public void Initalize ()
    {
        data = new int[layerConfig.Count * (width * height)];
    }

    public bool inBounds (int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public void ClearData ()
    {
        data = new int[(width * height) * layerPointers.Count];
    }

    //Tileset Stuff
    public Tileset GetTilesetByName(string TilesetName)
    {
        foreach(Tileset t in tilesetConfig)
        {
            if (t.name == TilesetName)
                return t;
        }
        throw new System.Exception("No Tileset Named " + TilesetName);
    }

    public Tileset GetTilesetByTileID (int TileID)
    {
        int? id = tileIdToTilesetId(TileID);
        if (id == null) { throw new System.Exception(" Tile ID doesnt belong to any tileset."); }
        return tilesetConfig[(int)id];
    }

    public int? globalIdToLocalId(int globalID)
    {
        int? tilesetID = tileIdToTilesetId(globalID);
        if (tilesetID == null)
            return null;//global id does not belong to any tileset.
        Tileset t = tilesetConfig[(int)tilesetID];

        int localID = (globalID - t.firstTileID);
        //Debug.Log(string.Format("GID {0} belongs to tileset {1} Tileset Tile Id is {2}", globalID, tilesetID, localID));
        return localID;
    }

    int? tileIdToTilesetId(int id)
    {
        Tileset t;
        for (int i = 0; i < tilesetConfig.Count; i++)
        {
            t = tilesetConfig[i];
            if (id <= t.tileCount + t.firstTileID)
                return i;
        }
        return null;
    }

    public  List<Vector2> getTileColliderInfo(int tileID)
    {
        string target;
        Tileset t = GetTilesetByTileID(tileID);
        if (t.colliderDataType == "")
            return new List<Vector2>();

        if (t.colliderDataType == "self")
        {
            target = t.image.name;
        }
        else
        {
            target = t.colliderDataType;
        }
        //                  [Tileset name that contains the colliderData for this tileset][the TileID that we want to acsess.]
        return GetTilesetByName(target).colliderInfo[tileID - t.firstTileID].points;
    }
}
