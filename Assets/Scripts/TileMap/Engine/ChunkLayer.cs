using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChunkLayer
{
    public bool Collisions = false;
    public bool emptyTileCollisionRule = false;

    public GameObject gameobject;
    public GameObject collision;
    public PolygonCollider2D collider;

    public List<SubLayer> sublayersList;
    public List<string> sublayerPointers;

    public sMap map;
    public Layer layerInfo;

    public ChunkLayer(GameObject Parent, sMap Map, Layer LayerInfo)
    {
        sublayersList = new List<SubLayer>();
        sublayerPointers = new List<string>();

        map = Map;
        layerInfo = LayerInfo;

        gameobject = new GameObject(layerInfo.name);
        gameobject.transform.parent = Parent.transform;

        if (layerInfo.useCollisions)
        {
            Collisions = layerInfo.useCollisions;
            emptyTileCollisionRule = false;

            collision = new GameObject("Collision");
            collision.transform.parent = gameobject.transform;
            collider = collision.AddComponent<PolygonCollider2D>();
            collider.pathCount = 0;
            collision.layer = LayerMask.NameToLayer(layerInfo.unityLayer);
            collider.isTrigger = layerInfo.isTrigger;
        }
        foreach(Tileset t in map.tilesetConfig)
        {
            getSubLayer(t.image.name);
        }
        //if (useAutoTile)
        //    AutoTile = useAutoTile;

    }

    public SubLayer getSubLayer(string Name)
    {
        if (!sublayerPointers.Contains(Name))
        {
            sublayersList.Add(new SubLayer(gameobject, Name, map, layerInfo.sortingLayer, layerInfo.sortingOrder));
            sublayerPointers.Add(Name);
        }
        return sublayersList[sublayerPointers.IndexOf(Name)];
    }

    public List<string> subLayerNames()
    {
        return sublayerPointers;
    }


}