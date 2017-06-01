using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SubLayer
{
    
    public GameObject gameobject;
    public List<Vector3> newVertices;
    public List<int> newTriangles;
    public List<Vector2> newUV;
    public int TileCount;
    public Mesh mesh;

    public int tWidth;//width in tiles.
    public int tHeight;// height in tiles.
    public float texUnitX;//width of one tile in Texture Units.
    public float texUnitY;//height of one tile in Texture Units.
    public bool autoTile;

    public int TILESIZE = 16;

    public SubLayer(GameObject parent, string MaterialName, sMap map, string SortingLayerName, int SortingOrder)
    {
        Tileset t = GameManager.GM.mapManager.GetTilesetByName(MaterialName);

        gameobject = new GameObject(MaterialName);

        gameobject.transform.parent = parent.transform;

        mesh = new Mesh();
        gameobject.AddComponent<MeshFilter>().mesh = mesh;
        mesh.MarkDynamic();
        Renderer rend = gameobject.AddComponent<MeshRenderer>();
        rend.sortingLayerName = SortingLayerName;
        rend.sortingOrder = SortingOrder;
        Material mat = t.material;
        rend.material = mat;

        tWidth = mat.mainTexture.width / TILESIZE;
        tHeight = mat.mainTexture.height / TILESIZE;
        texUnitX = (float)TILESIZE / mat.mainTexture.width;
        texUnitY = (float)TILESIZE / mat.mainTexture.height;

        newVertices = new List<Vector3>();
        newTriangles = new List<int>();
        newUV = new List<Vector2>();
        TileCount = 0;
    }
}