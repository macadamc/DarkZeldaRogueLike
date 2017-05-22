using UnityEngine;
using System.Collections;

public static class Rule45
{
    static int[,] nMap;

    public static void Generate(int[,] map, DefaultRNG Rng, int TileID, int Passes)
    {

        for (int x = 0; x < map.GetLength(1); x++)
        {
            for (int y = 0; y < map.GetLength(0) ; y++)
            {
                if (Rng.Next(0,2) == 1)
                {
                    map[y, x] = TileID;
                }
            }
        }

        for (int i = 0; i < Passes; i++)
        {
            nMap = new int[map.GetLength(0), map.GetLength(1)];
            for  (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    //if (MapUtil.instance["Walls", y, x] != 0)
                    //    continue;

                    if(adjWallCount(x, y, map, TileID) >= 5)
                    {
                        nMap[y, x] = TileID;
                    } else
                    {
                        nMap[y, x] = TileID + Rng.Next(1,4);
                    }
                }
            }

            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    map[y, x] = nMap[y, x];
                }
            }
        }
    }

     static int adjWallCount(int x, int y, int[,] Map, int TileID)
    {
        int nCount = 0;

        for (int xi = x - 1; xi <= x + 1; xi++)
        {
            for (int yi = y - 1; yi <= y + 1; yi++)
            {
                if (inBounds(xi, yi, Map))
                {
                    if (Map[yi, xi] == TileID)
                    {
                        nCount++;
                    }
                }
            }
        }
        return nCount;
    }

    static bool inBounds(int x, int y, int[,] Map)
    {
        if (x < 0 || y < 0 || x > Map.GetLength(1) - 1 || y > Map.GetLength(0) - 1)
        {
            return false;
        }
        return true;
    }
}
