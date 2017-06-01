using UnityEngine;
using System.Xml;
using System.IO;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TileMapManager : MonoBehaviour {

    public sMap map;
    [HideInInspector]
    public ChunkManager cManager;

	// Use this for initialization
	public void Awake ()
    {
        map.cManager = cManager;

        if (cManager.mapChunksGameObject == null)
        {
            map.ClearData();
            cManager.chunkPointers.Clear();
            cManager.mapChunks.Clear();
            cManager.mapChunksGameObject = new GameObject("MapChunks");
            cManager.mapChunksGameObject.transform.position = new Vector3(-map.width / 2, -map.height / 2, 0);
            
        }
    }

    void Start()
    {
        cManager.SpawnChunks(map);
    }
    void LateUpdate ()
    {
        cManager.UpdateChunks(map);
    }

    //Tileset Stuff
    public Tileset GetTilesetByName(string TilesetName)
    {
        foreach (Tileset t in map.tilesetConfig)
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
        return map.tilesetConfig[(int)id];
    }

    public int? globalIdToLocalId(int globalID)
    {
        int? tilesetID = tileIdToTilesetId(globalID);
        if (tilesetID == null)
            return null;//global id does not belong to any tileset.
        Tileset t = map.tilesetConfig[(int)tilesetID];

        int localID = (globalID - t.firstTileID);
        //Debug.Log(string.Format("GID {0} belongs to tileset {1} Tileset Tile Id is {2}", globalID, tilesetID, localID));
        return localID;
    }

    int? tileIdToTilesetId(int id)
    {
        Tileset t;
        for (int i = 0; i < map.tilesetConfig.Count; i++)
        {
            t = map.tilesetConfig[i];
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



    public static sMap Load(TextAsset TmxAsTxt)
    {
        sMap map = ScriptableObject.CreateInstance<sMap>();

        XmlDocument xml = new XmlDocument();
        xml.LoadXml(TmxAsTxt.text);

        XmlNode mapNode = xml.SelectSingleNode("map");

        map.width = int.Parse(mapNode.Attributes["width"].Value);
        map.height = int.Parse(mapNode.Attributes["height"].Value);

        XmlNodeList tilesets = xml.SelectNodes("tileset");

        TempTilesetList oldFirstGID = new TempTilesetList();

        foreach (XmlNode tilesetNode in tilesets)
        {
            string name = Path.GetFileNameWithoutExtension(tilesetNode.Attributes["source"].Value);

            if (map.tilesetPointers.IndexOf(name) < 0)
            {
                _createTileset(map, tilesetNode, oldFirstGID);
            }
        }

        XmlNodeList layers = xml.SelectNodes("layer");
        map.data = new int[layers.Count * (map.width * map.height)];

        for (int i = 0; i < layers.Count; i++)
        {
            if (map.layerPointers.IndexOf(layers[i].Attributes["name"].Value) < 0)
            {
                _createLayer(map, layers[i], oldFirstGID);
            }

            string[] tiles = layers[i].SelectSingleNode("data").Value.Split(new char[1] { ',' });
            int layerStart = map._getLayerStartIndex(layers[i].Name);

            for (int tileIndex = 0; tileIndex < tiles.Length; tileIndex++)
            {
                //foreach tile
                int mapGID = int.Parse(tiles[tileIndex]);
                TilesetTemp temp = oldFirstGID.Values[(int)oldFirstGID.tileIdToTilesetId(mapGID)];

                map.data[layerStart + tileIndex] = GameManager.GM.mapManager.GetTilesetByName(temp.name).firstTileID + (int)oldFirstGID.globalIdToLocalId(mapGID);
            }
        }

        return map;
    }

    static void _createLayer(sMap map, XmlNode layer, TempTilesetList oldFirstGID)
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

    static Tileset _createTileset(sMap map, XmlNode tilesetNode, TempTilesetList oldFirstGID)
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

