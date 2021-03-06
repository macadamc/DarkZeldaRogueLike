﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClipperLib;

[System.Serializable]
public class ChunkManager
{
    bool redraw;
    public List<DirtyInfo> dirtyList;

    public List<WorldPos> chunkPointers;
    public List<MapChunk> mapChunks;

    public WorldPos pointer;

    public GameObject mapChunksGameObject;

    bool triggered;

    public ChunkManager()
    {
        chunkPointers = new List<WorldPos>();
        mapChunks = new List<MapChunk>();
        dirtyList = new List<DirtyInfo>();
        
    }

    public MapChunk CreateChunk(int x, int y, sMap map)
    {
        MapChunk chunk = new MapChunk(x, y, map);

        chunkPointers.Add(chunk.pos);
        mapChunks.Add(chunk);

        chunk.gameobject.transform.parent = mapChunksGameObject.transform;
        chunk.gameobject.transform.localPosition = new Vector3(x * map.chunkWidth, y * map.chunkHeight, 0);
        return chunk;
    }

    public MapChunk GetChunkFromWorldPos(int x, int y, sMap map)
    {
        pointer.X = x / map.chunkWidth;

        pointer.Y = y / map.chunkHeight;

        if (chunkPointers.Contains(pointer))
            return mapChunks[chunkPointers.IndexOf(pointer)];

        return null;
    }

    public void SpawnChunks(sMap map)
    {
        int widthInChunks = map.width / map.chunkWidth;
        int heightInChunks = map.height / map.chunkHeight;
        MapChunk cChunk;

        for (int y = 0; y < heightInChunks; y++)
        {
            for (int x = 0; x < widthInChunks; x++)
            {
                pointer.X = x;
                pointer.Y = y;

                if (chunkPointers.Contains(pointer))
                {
                    cChunk = mapChunks[chunkPointers.IndexOf(pointer)];
                }
                else
                {
                    cChunk = CreateChunk(x, y, map);
                }
            }
        }
    }

    public void Dirty(MapChunk chunk, string LayerName)
    {
        if (chunk == null)
            return;

        DirtyInfo dirty = new DirtyInfo(chunk, LayerName);

        if (!dirtyList.Contains(dirty))
        {
            dirtyList.Add(dirty);
            redraw = true;
        }
    }

    public void UpdateChunks(sMap map, bool force = false)
    {
        if (force == false)
        {
            if (redraw == false)
                return;

            foreach (DirtyInfo dirtyInfo in dirtyList)
            {
                PolyGen.GenerateLayer(dirtyInfo, map);
            }
            dirtyList.Clear();
            redraw = false;
        }
        else
        {

            foreach (MapChunk chunk in mapChunks)
            {
                foreach (string LayernName in chunk.layerPointers)
                {
                    PolyGen.GenerateLayer(new DirtyInfo(chunk, LayernName), map);
                }
            }
            
        }

        dirtyList.Clear();
    }

    public void ClearData()
    {
        dirtyList.Clear();
        chunkPointers.Clear();
        mapChunks.Clear();
        GameObject.Destroy(mapChunksGameObject);
    }
}