using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class MazeGenerator : MapGenerator
{
    public class IntPoint
    {
        public int x;
        public int y;
        public IntPoint(int X, int Y)
        {
            x = X;
            y = Y;
        }
    }

    int TileID;
    TileMapManager mManager;

    public override void Generate(sMap map, DefaultRNG rng, EntityMetaDataSO entityData)
    {
        mManager = GameManager.GM.mapManager;
        TileID = mManager.GetTilesetByName("forestTileset").firstTileID + 2;

        Visited = new bool[map.width, map.height];
        Fringe = new List<IntPoint>();

        string Layer = "Walls";
        int startX = 1;
        int startY = 1;

        map.SetLayerTo(Layer, TileID);

        IntPoint start = new IntPoint(startX, startY);

        Fringe.Add(start);

        while (Fringe.Count > 0)
        {
            IntPoint selected = pickNextFringe(rng);
            map[Layer, selected.x, selected.y] = 0;
            Visited[selected.x, selected.y] = true;

            List<IntPoint> adjacent = findNeighbors(selected, TileID, map);
            if (adjacent.Count > 0)
            {
                IntPoint adjPos = adjacent[rng.Next(0, adjacent.Count - 1)];
                if (rng.NextDouble() >= loopPercent)
                {
                    map[Layer, adjPos.x, adjPos.y] = 0;
                }
                setPassage(selected, adjPos, map);
                Fringe.Add(adjPos);
            }
            else
            {
                Fringe.Remove(selected);
            }
        }
        return;
    }

    public bool[,] Visited;
    List<IntPoint> Fringe;

    [Range(0f, 1f)]
    public float loopPercent;
    [Range(0f, 1f)]
    public float windyOrRandomPercent;

    public string Layer;

    IntPoint pickNextFringe(DefaultRNG Rng)
    {
        if (Rng.NextDouble() <= windyOrRandomPercent)
        {
            return Fringe[Fringe.Count - 1];
        }
        else
        {
            return Fringe[Rng.Next(0, Fringe.Count - 1)];
        }
    }

    public List<IntPoint> findNeighbors(IntPoint Pos, int TileState, sMap map)
    {
        List<IntPoint> ret = new List<IntPoint>();

        foreach (IntPoint index in Utility.dirs)
        {
            int x = Pos.x + (index.x * 2);
            int y = Pos.y + (index.y * 2);
            
            if (map.inBounds(x, y) == false || map.isEdge(x, y)) { continue; }

            if (map[Layer, x, y] == TileState && Visited[x, y] == false)
            {
                ret.Add(new IntPoint(x, y));
            }
        }
        return ret;
    }

    void setPassage(IntPoint a, IntPoint b, sMap Map)
    {
        int x = (a.x + b.x) / 2;
        int y = (a.y + b.y) / 2;

        Map[Layer, x, y] = 0;
    }

    public void sparsify(sMap Map)
    {
        int count;
        int state;

        for (int x = 0; x < Map.width; x++)
        {
            for (int y = 0; y < Map.height; y++)
            {
                if (Map[Layer, x, y] == 1)
                {
                    state = 0;
                }
                else
                {
                    state = 1;
                }

                count = 0;
                foreach (IntPoint i in Utility.dirs)
                {
                    int nx = i.x + x;
                    int ny = i.y + y;

                    if (nx < 0 || nx > Map.width - 1 || ny < 0 || ny > Map.height - 1)
                    {
                        continue;
                    }

                    if (Map[Layer, nx, ny] == state)
                    {
                        count++;
                    }
                }
                if (Map[Layer, x, y] == 0)
                {
                    if (count > 2)
                    {
                        Map[Layer, x, y] = TileID;
                    }

                }

            }
        }
    }

}