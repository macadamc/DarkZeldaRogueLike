using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClipperLib;

public static class PolyGen
{

    static Dictionary<string, Clipper> clippers = new Dictionary<string, Clipper>();
    static Dictionary<string, List<List<IntPoint>>> solutions = new Dictionary<string, List<List<IntPoint>>>();

    //public static void Generate(MapChunk chunk, MapData Map)
    //{
    //    string curSubLayerName;

    //    int cx = (int)chunk.pos.X * MapChunk.chunkSize;
    //    int cy = (int)chunk.pos.Y * MapChunk.chunkSize;

    //    for (int py = cy; py < cy + MapChunk.chunkSize; py++)
    //    {
    //        if (py >= Map.height)
    //            break;

    //        for (int px = cx; px < cx + MapChunk.chunkSize; px++)
    //        {
    //            if (px >= Map.width)
    //                break;

    //            foreach (string layername in Map.layerNames)
    //            {
    //                map = Map.GetLayer(layername);
    //                Tileset t = Tileset.Get(map[py, px]);
    //                curSubLayerName = t.name;

    //                SubLayer subLayer = null;

    //                if (map[py, px] != 0)
    //                {
    //                    subLayer = chunk.layers[layername].getSubLayer(curSubLayerName);
    //                }

    //                if (map[py, px] == 0)
    //                {
    //                    if (chunk.layers[layername].Collisions && chunk.layers[layername].emptyTileCollisionRule)
    //                    {
    //                        if (!clippers.ContainsKey(layername))
    //                        {
    //                            clippers.Add(layername, new Clipper());
    //                            solutions.Add(layername, new List<List<IntPoint>>());
    //                        }
    //                        int x = px;
    //                        int y = py;
    //                        List<IntPoint> points = new List<IntPoint>();
    //                        points.Add(new IntPoint(x, y + 1));
    //                        points.Add(new IntPoint(x, y));
    //                        points.Add(new IntPoint(x + 1, y));
    //                        points.Add(new IntPoint(x + 1, y + 1));

    //                        clippers[layername].AddPath(points, PolyType.ptSubject, true);
    //                    }
    //                }

    //                else

    //                {
    //                    if (subLayer != null)
    //                    {
    //                        GenTile(px, py, map[py, px], subLayer);
    //                    }


    //                    if (chunk.layers[layername].Collisions)
    //                    {
    //                        if (!clippers.ContainsKey(layername))
    //                        {
    //                            clippers.Add(layername, new Clipper());
    //                            solutions.Add(layername, new List<List<IntPoint>>());
    //                        }
    //                        List<IntPoint> ColliderInfo = Tileset.getTileColliderInfo(map[py, px]);
    //                        List<IntPoint> pointList = new List<IntPoint>();
    //                        for (int i = 0; i < ColliderInfo.Count; i++)
    //                        {
    //                            pointList.Add(new IntPoint((ColliderInfo[i].X) + (px * 16), (ColliderInfo[i].Y) + (py * 16)));
    //                        }

    //                        if (ColliderInfo.Count > 2)
    //                        {
    //                            clippers[layername].AddPath(pointList, PolyType.ptSubject, true);
    //                        }
    //                    }

    //                }
    //            }

    //        }
    //    }

    //    foreach (string layerName in clippers.Keys)
    //    {
    //        clippers[layerName].Execute(ClipType.ctUnion, solutions[layerName]);

    //        chunk.layers[layerName].collider.pathCount = solutions[layerName].Count;

    //        for (int i = 0; i < solutions[layerName].Count; i++)
    //        {
    //            List<Vector2> p = new List<Vector2>();

    //            foreach (IntPoint vert in solutions[layerName][i])
    //            {
    //                p.Add(new Vector2((vert.X / 16f) - chunk.gameobject.transform.position.x, (vert.Y / 16f) - chunk.gameobject.transform.position.y));
    //            }

    //            chunk.layers[layerName].collider.SetPath(i, p.ToArray());
    //        }
    //    }

    //    foreach (string layerName in chunk.layers.Keys)
    //    {
    //        foreach (string name in chunk.layers[layerName].subLayerNames())
    //        {
    //            SubLayer sub = chunk.layers[layerName].getSubLayer(name);

    //            sub.mesh.Clear();
    //            sub.mesh.vertices = sub.newVertices.ToArray();
    //            sub.mesh.triangles = sub.newTriangles.ToArray();
    //            sub.mesh.uv = sub.newUV.ToArray();
    //            sub.mesh.RecalculateNormals();

    //            sub.newVertices.Clear();
    //            sub.newTriangles.Clear();
    //            sub.newUV.Clear();
    //            sub.TileCount = 0;
    //        }
    //    }

    //    solutions.Clear();
    //    clippers.Clear();
    //}

    static void GenTile(long x, long y, int ID, SubLayer sub, sMap Map)
    {
        sub.newVertices.Add(new Vector3(x, y, 0));// (0,0) 0
        sub.newVertices.Add(new Vector3(x, y + 1, 0));// (0,1) 1
        sub.newVertices.Add(new Vector3(x + 1, y + 1, 0));// (1,1) 2
        sub.newVertices.Add(new Vector3(x + 1, y, 0));// (1,0) 3

        sub.newTriangles.Add((sub.TileCount * 4) + 0);
        sub.newTriangles.Add((sub.TileCount * 4) + 1);
        sub.newTriangles.Add((sub.TileCount * 4) + 2);
        sub.newTriangles.Add((sub.TileCount * 4) + 0);
        sub.newTriangles.Add((sub.TileCount * 4) + 2);
        sub.newTriangles.Add((sub.TileCount * 4) + 3);

        Vector2 vMin = TileMin(ID, sub, Map);
        sub.newUV.Add(new Vector2(vMin.x, vMin.y));
        sub.newUV.Add(new Vector2(vMin.x, vMin.y + sub.texUnitY));
        sub.newUV.Add(new Vector2(vMin.x + sub.texUnitX, vMin.y + sub.texUnitY));
        sub.newUV.Add(new Vector2(vMin.x + sub.texUnitX, vMin.y));

        sub.TileCount++;
    }

    public static Vector2 TileMin(int id, SubLayer sub, sMap Map)
    {
        int? idExists = Map.globalIdToLocalId(id);
        if (idExists != null)
        {
            id = (int)idExists;
        }

        float x = (id % sub.tWidth) / (float)sub.tWidth;// X position in tilespace.
        float y = (id / sub.tWidth) / (float)sub.tHeight;// Y position in tilespace.

        y = (1f - y) - sub.texUnitY;// tiled's tilesets cordanates are top left while unity texture cordanates are bottom left
                                    // so  we need to flip the Y position and offset it by one tile.

        return new Vector2(x, y);
    }

    public static void GenerateLayer(DirtyInfo dirtyInfo, sMap Map)
    {
        string curSubLayerName;
        string layername = dirtyInfo.layerName;
        MapChunk chunk = dirtyInfo.chunk;

        int cx = (int)chunk.pos.X * Map.chunkSize;
        int cy = (int)chunk.pos.Y * Map.chunkSize;


        // for each position in the current chunk.
        for (int py = cy; py < cy + Map.chunkSize; py++)
        {
            for (int px = cx; px < cx + Map.chunkSize; px++)
            {

                if (!Map.inBounds(px, py)) continue;

                Tileset t = Map.GetTilesetByTileID(Map[layername, px, py]);
                curSubLayerName = t.name;

                SubLayer subLayer = null;

                ChunkLayer layer = chunk.GetLayer(layername);
                subLayer = layer.getSubLayer(curSubLayerName);

                // update Tile info.
                if (Map[layername, px, py] > 0)
                {
                    GenTile(px - (chunk.pos.X * Map.chunkSize), py - (chunk.pos.Y * Map.chunkSize), Map[layername, px, py], subLayer, Map);
                }

                UpdateCollisions(layer, layername, px, py, Map);

            }
        }

        CreateColliders(chunk);
        DrawTiles(chunk, layername);

        solutions.Clear();
        clippers.Clear();
    }

    static void UpdateCollisions (ChunkLayer layer, string layername, int px, int py, sMap Map)
    {
        if (layer.layerInfo.useCollisions)
        {
            if (!clippers.ContainsKey(layername))
            {
                clippers.Add(layername, new Clipper());
                solutions.Add(layername, new List<List<IntPoint>>());
            }

            if (layer.layerInfo.emptyTileCollision && Map[layername, px, py] == 0)
            {

                int x = px * 16;
                int y = py * 16;
                List<IntPoint> points = new List<IntPoint>();
                points.Add(new IntPoint(x, y + 16));
                points.Add(new IntPoint(x, y));
                points.Add(new IntPoint(x + 16, y));
                points.Add(new IntPoint(x + 16, y + 16));

                clippers[layername].AddPath(points, PolyType.ptSubject, true);
            }

            // if fulltilecollisions is true OR if the value at this map pos is -1;
            if (layer.layerInfo.fullTileCollisions || Map[layername, px, py] == -1)
            {
                int x = px * 16;
                int y = py * 16;
                List<IntPoint> points = new List<IntPoint>();
                points.Add(new IntPoint(x, y + 16));
                points.Add(new IntPoint(x, y));
                points.Add(new IntPoint(x + 16, y));
                points.Add(new IntPoint(x + 16, y + 16));

                clippers[layername].AddPath(points, PolyType.ptSubject, true);
            }

            if (layer.layerInfo.emptyTileCollision == false && layer.layerInfo.fullTileCollisions == false && Map[layername, px, py] > 0)
            {
                List<Vector2> ColliderInfo = Map.getTileColliderInfo(Map[layername, px, py]);
                List<IntPoint> pointList = new List<IntPoint>();
                for (int i = 0; i < ColliderInfo.Count; i++)
                {
                    pointList.Add(new IntPoint((ColliderInfo[i].x) + (px * 16), (ColliderInfo[i].y) + (py * 16)));
                }

                if (ColliderInfo.Count > 2)
                {
                    clippers[layername].AddPath(pointList, PolyType.ptSubject, true);
                }
            }

        }
    }

    static void CreateColliders(MapChunk chunk)
    {
        foreach (string layerName in clippers.Keys)
        {
            clippers[layerName].Execute(ClipType.ctUnion, solutions[layerName]);

            chunk.GetLayer(layerName).collider.pathCount = solutions[layerName].Count;

            for (int i = 0; i < solutions[layerName].Count; i++)
            {
                List<Vector2> p = new List<Vector2>();

                foreach (IntPoint vert in solutions[layerName][i])
                {
                    p.Add(new Vector2((vert.X / 16f) - chunk.gameobject.transform.localPosition.x, (vert.Y / 16f) - chunk.gameobject.transform.localPosition.y));
                }

                chunk.GetLayer(layerName).collider.SetPath(i, p.ToArray());
            }
        }
    }

    static void DrawTiles(MapChunk chunk, string layername)
    {
        foreach (string name in chunk.GetLayer(layername).subLayerNames())
        {
            SubLayer sub = chunk.GetLayer(layername).getSubLayer(name);

            sub.mesh.Clear();
            sub.mesh.vertices = sub.newVertices.ToArray();
            sub.mesh.triangles = sub.newTriangles.ToArray();
            sub.mesh.uv = sub.newUV.ToArray();
            sub.mesh.RecalculateNormals();

            sub.newVertices.Clear();
            sub.newTriangles.Clear();
            sub.newUV.Clear();
            sub.TileCount = 0;
        }
    }
}