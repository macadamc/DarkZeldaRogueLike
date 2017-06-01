using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class TileMapEditorWindow : EditorWindow
{
    public Vector2 scrollPos;
    public static TileMapManager tManager;

    [MenuItem("Window/TileMap Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(TileMapEditorWindow));
    }

    void OnGUI ()
    {
        int tWidth;//width in tiles.
        int tHeight;// height in tiles.
        float texUnitX;//width of one tile in Texture Units.
        float texUnitY;//height of one tile in Texture Units.
        int TILESIZE = 16;
        int ZoomSize = 64;

        if (tManager == null)
        {
            tManager = MonoBehaviour.FindObjectOfType<TileMapManager>();
        }
        Rect scrollViewPos = new Rect(0, 0, tManager.map.width * ZoomSize, tManager.map.height * ZoomSize);
        scrollPos = GUI.BeginScrollView(scrollViewPos, scrollPos, new Rect(0, 0, 100, 100));

        for (int x = 0; x < tManager.map.width;x++)
        {
            for(int y = 0; y < tManager.map.height;y++)
            {
                int tileID = tManager.map["Walls", x, y];
                if (tileID == 0) { continue; }
                Tileset t = tManager.GetTilesetByTileID(tileID);
                int posInTileset = (int)tManager.globalIdToLocalId(tileID) - 1;

                tWidth = (t.image.width / TILESIZE);
                tHeight = (t.image.height / TILESIZE);

                texUnitX = 1f / tWidth;
                texUnitY = 1f / tHeight;


                float tilesetPosX = (posInTileset % tWidth) / (float)tWidth;// X position in tilespace.
                float tilesetPosY = (posInTileset / tWidth) / (float)tHeight;// Y position in tilespace.
                Rect UVPos = new Rect(tilesetPosX, tilesetPosY, texUnitX, texUnitY);
                Rect TexturePos = new Rect(x * ZoomSize, y * ZoomSize, ZoomSize, ZoomSize);
                GUI.DrawTextureWithTexCoords(TexturePos, t.image, UVPos);
            }
        }
        GUI.EndScrollView();
    }

}