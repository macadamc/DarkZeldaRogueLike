using System.Collections.Generic;
using UnityEngine;

using System.Xml;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class Tileset {

    [HideInInspector]
    public string name;
    public Texture2D image;
    public Material material;
    public TextAsset TilesetXml;
    public bool useAutoTile;

    [HideInInspector]
    public int tileCount;
    [HideInInspector]
    public int firstTileID;
    [HideInInspector]
    public string colliderDataType;

    public ColliderInfo[] colliderInfo;

    public List<string> propKeys;
    public List<string> propValues;

    ColliderInfo[] createColliderData(XmlDocument xml)
    {
        ColliderInfo[] data = new ColliderInfo[tileCount];
        foreach (XmlNode Tile in xml.SelectNodes("tileset/tile"))
        {
            XmlNode obj = Tile.SelectSingleNode("objectgroup/object");

            List<Vector2> points = new List<Vector2>();
            foreach (string pos in Tile.SelectSingleNode("objectgroup/object/polygon").Attributes["points"].Value.Split(' '))
            {
                string[] vals = pos.Split(',');
                points.Add(new Vector2(Mathf.RoundToInt(float.Parse(vals[0]) + int.Parse(obj.Attributes["x"].Value)), Mathf.RoundToInt((16 - (int)float.Parse(vals[1]) + int.Parse(obj.Attributes["y"].Value)))));
            }

            int ID = int.Parse(Tile.Attributes["id"].Value);
            data[ID] = new ColliderInfo();
            data[ID].points = points;
        }
        return data;
    }

#if UNITY_EDITOR
    public void Initalize(sMap Map)
    {

        XmlDocument xml = new XmlDocument();
        xml.LoadXml(TilesetXml.text);

        XmlNode tileset = xml.SelectSingleNode("tileset");

        name = image.name;
        tileCount = int.Parse(tileset.Attributes["tilecount"].Value);

        firstTileID = Map.nextTileID;
        Map.nextTileID += tileCount;

        //Set tileset Properties;
        foreach (XmlNode n in xml.SelectNodes("tileset/properties/property"))
        {
            string name = n.Attributes["name"].Value;
            string value = n.Attributes["value"].Value;
            if (propKeys.Contains(name))
            {
                propValues[propKeys.IndexOf(name)] = value;
            }
            else
            {
                propKeys.Add(name);
                propValues.Add(value);
            }


        }

        // collision data type is  == "self" if this tileset holds the actual collider points
        // or if its equal to another anything else then when making the colliders it will look at the other tileset collider info instead.
        colliderDataType = propValues[propKeys.IndexOf("Collision")];

        if (material == null)
        {
            material = new Material(Shader.Find("Sprites/Default"));
            material.mainTexture = image;
            AssetDatabase.CreateAsset(material, string.Format("Assets/{0}.asset", image.name));
        }
        if (colliderDataType == "self")
        {
            colliderInfo = createColliderData(xml);
        }

        // create Colliderdata

        //switch (colliderDataType)
        //{
        //    case "self": // this tileset contains the actual collider data that its going to use so we need to create it.
        //        if (!_colliderData.ContainsKey(name))
        //            _colliderData.Add(name, createColliderData(xml));
        //        break;
        //    case "": // This tileset Has No colliderData.
        //        break;
        //    default:
        //        if (!_colliderData.ContainsKey(colliderDataType))//tilesetInfo.ContainsKey(info.ColliderData))
        //        {
        //            // if the current tilesets colliderdata isnt in the colliderdata dictionary we need to create it...
        //            // this just straight up loads the other tileset.
        //            //TODO: just load the collider data and store that somehow.

        //            //string cPathToPng = string.Format("{0}{1}", pathToPng, Name);
        //            //string cPathToTsx = string.Format("{0}{1}", pathToTsx, Name);
        //            //new Tileset(cPathToPng, cPathToTsx, colliderDataType, autoTile, sortingOrder);
        //        }
        //        break;
        //}

        //if (!pointers.Contains(name))
        //{
        //    pointers.Add(name);
        //    Tilesets.Add(name, this);
        //}
    }
#endif

}
