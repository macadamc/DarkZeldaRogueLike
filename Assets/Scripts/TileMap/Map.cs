using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;
using System.IO;
using ClipperLib;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class Map
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

    public Map()
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
            if (inBounds(x, y))
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
        if (i == -1) { throw new System.Exception(string.Format("No Layer named {0}", name)); }
        return i * (width * height);

    }

    public void Initalize()
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
        return (x == 0 || y == 0) || (x == height - 1 || y == height - 1);
    }
    public bool inBounds(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public void ClearData()
    {
        data = new int[(width * height) * layerPointers.Count];
    }

    //Tileset Stuff
    public Tileset GetTilesetByName(string TilesetName)
    {
        foreach (Tileset t in tilesetConfig)
        {
            if (t.name == TilesetName)
                return t;
        }
        throw new System.Exception("No Tileset Named " + TilesetName);
    }

    public Tileset GetTilesetByTileID(int TileID)
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

    public List<Vector2> getTileColliderInfo(int tileID)
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


    public static Map Load(TextAsset TmxAsTxt)
    {
        Map map = new Map();

        XmlDocument xml = new XmlDocument();
        xml.LoadXml(TmxAsTxt.text);

        XmlNode mapNode = xml.SelectSingleNode("map");

        map.width = int.Parse(mapNode.Attributes["width"].Value);
        map.height = int.Parse(mapNode.Attributes["height"].Value);

        XmlNodeList tilesets = mapNode.SelectNodes("tileset");
        Debug.Log(tilesets.Count);
        TempTilesetList oldFirstGID = new TempTilesetList();

        foreach (XmlNode tilesetNode in tilesets)
        {
            string name = Path.GetFileNameWithoutExtension(tilesetNode.Attributes["source"].Value);

            if (map.tilesetPointers.IndexOf(name) < 0)
            {
                _createTileset(map, tilesetNode, oldFirstGID);
            }
        }

        XmlNodeList layers = mapNode.SelectNodes("layer");
        Debug.Log(layers.Count);
        map.data = new int[layers.Count * (map.width * map.height)];

        for (int i = 0; i < layers.Count; i++)
        {
            if (map.layerPointers.IndexOf(layers[i].Attributes["name"].Value) < 0)
            {
                _createLayer(map, layers[i], oldFirstGID);
            }
            string[] tiles = layers[i].SelectSingleNode("data").InnerText.Split(new char[1] { ',' });
            int layerStart = map._getLayerStartIndex(layers[i].Attributes["name"].Value);

            for (int tileIndex = 0; tileIndex < tiles.Length; tileIndex++)
            {
                //foreach tile
                int mapGID = int.Parse(tiles[tileIndex]);
                if (oldFirstGID.tileIdToTilesetId(mapGID) != null)
                {
                    TilesetTemp temp = oldFirstGID.Values[(int)oldFirstGID.tileIdToTilesetId(mapGID)];

                    map.data[layerStart + tileIndex] = map.GetTilesetByName(temp.name).firstTileID + (int)oldFirstGID.globalIdToLocalId(mapGID);
                }
                else
                {
                    map.data[layerStart + tileIndex] = mapGID;
                }
            }
        }

        return map;
    }

    static void _createLayer(Map map, XmlNode layer, TempTilesetList oldFirstGID)
    {
        Layer l = new Layer();
        l.name = layer.Attributes["name"].Value;

        map.layerPointers.Add(l.name);

        l.useCollisions = false;
        l.useAutoTile = false;
        l.unityLayer = "Default";
        l.sortingOrder = 0;
        l.sortingLayer = "Default";
        l.isTrigger = false;
        l.fullTileCollisions = false;
        l.emptyTileCollision = false;

        map.layerConfig.Add(l);
    }

    static Tileset _createTileset(Map map, XmlNode tilesetNode, TempTilesetList oldFirstGID)
    {
        string name = Path.GetFileNameWithoutExtension(tilesetNode.Attributes["source"].Value);

        Tileset t = new Tileset();

#if UNITY_EDITOR
        string GUID = AssetDatabase.FindAssets("t:TextAsset " + name)[0];
        t.TilesetXml = AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(GUID));

        GUID = AssetDatabase.FindAssets("t:Texture2D " + name)[0];
        t.image = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(GUID));
#endif

        if (t.TilesetXml == null)
        {
            t.TilesetXml = Resources.Load<TextAsset>(name);
        }

        if (t.image == null)
        {
            t.image = Resources.Load<Texture2D>(name);
        }

        if (t.image == null || t.TilesetXml == null)
        {
            throw new System.NullReferenceException(string.Format("the image or the collision data for the tileset {0} couldnt be found.", name));
        }

        t.Initalize(map);

        if (map.tilesetPointers.Contains(name) == false)
        {
            map.tilesetPointers.Add(name);
            map.tilesetConfig.Add(t);
        }
        else if (oldFirstGID.Keys.Contains(name) == false)
        {
            oldFirstGID.Keys.Add(name);
            oldFirstGID.Values.Add(new TilesetTemp(name, int.Parse(tilesetNode.Attributes["firstgid"].Value), t.tileCount));
        }

        return t;
    }

    class TilesetTemp
    {
        public string name;
        public int firstGID;
        public int tileCount;

        public TilesetTemp(string name, int firstGID, int tileCount)
        {
            this.name = name;
            this.firstGID = firstGID;
            this.tileCount = tileCount;
        }
    }

    class TempTilesetList
    {
        public List<string> Keys;
        public List<TilesetTemp> Values;

        public TempTilesetList()
        {
            Keys = new List<string>();
            Values = new List<TilesetTemp>();
        }

        public int? tileIdToTilesetId(int id)
        {

            for (int i = 0; i < Keys.Count; i++)
            {
                if (id <= Values[i].firstGID + Values[i].tileCount)
                    return i;
            }
            return null;
        }

        public int? globalIdToLocalId(int globalID)
        {
            int? tilesetID = tileIdToTilesetId(globalID);
            if (tilesetID == null)
                return null;//global id does not belong to any tileset.
            TilesetTemp t = Values[(int)tilesetID];

            int localID = (globalID - t.firstGID);
            //Debug.Log(string.Format("GID {0} belongs to tileset {1} Tileset Tile Id is {2}", globalID, tilesetID, localID));
            return localID;
        }

    }
}
