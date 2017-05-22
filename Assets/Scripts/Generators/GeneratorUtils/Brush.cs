using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brush
{

    string layer;
    sMap map;
    public int TileID;

    public Brush(sMap Map, string LayerName, int TileID)
    {
        layer = LayerName;
        map = Map;
        this.TileID = TileID;
    }

    public void DrawCircle(Vector2 centerPos, float radius)
    {
        for (int y = (int)(centerPos.y - radius); y <= centerPos.y + radius; y++)
        {
            for (int x = (int)(centerPos.x - radius); x <= centerPos.x + radius; x++)
            {
                float dist = ((centerPos.x - x) * (centerPos.x - x) + (centerPos.y - y) * (centerPos.y - y));
                if (dist < (radius) * (radius))
                {
                    try
                    {
                        map[layer, x, y] = TileID;
                    }
                    catch
                    {
                        continue;
                    }

                }
            }
        }
    }

    public void DrawSquare(int sx, int sy, int width, int height)
    {
        for (int x = sx; x <= sx + width; x++)
        {
            for (int y = sy; y <= sy + height; y++)
            {
                map[layer, x, y] = TileID;
            }
        }
    }

    public void DrawLine(int x, int y, int x2, int y2)
    {
        int w = x2 - x;
        int h = y2 - y;
        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
        if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
        if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
        if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
        int longest = Mathf.Abs(w);
        int shortest = Mathf.Abs(h);
        if (!(longest > shortest))
        {
            longest = Mathf.Abs(h);
            shortest = Mathf.Abs(w);
            if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
            dx2 = 0;
        }
        int numerator = longest >> 1;
        for (int i = 0; i <= longest; i++)
        {
            map[layer, x, y] = TileID;
            numerator += shortest;
            if (!(numerator < longest))
            {
                numerator -= longest;
                x += dx1;
                y += dy1;
            }
            else {
                x += dx2;
                y += dy2;
            }
        }
    }

    //public void DrawLine(Vector2 start, Vector2 end, int width)
    //{
    //    // direction of the line.
    //    Vector3 dir = start - end;
    //    dir.Normalize();

    //    var quat = Quaternion.AngleAxis(90, Vector3.forward);
    //    Debug.Log(string.Format("{0}, {1}", dir, quat * dir));
    //    quat = Quaternion.AngleAxis(180, Vector3.forward);
    //    Debug.Log(string.Format("{0}, {1}", dir, quat * dir));
    //    quat = Quaternion.AngleAxis(270, Vector3.forward);
    //    Debug.Log(string.Format("{0}, {1}", dir, quat * dir));

    //    // corners of the rect.
    //    Vector3 s1;
    //    Vector3 s2;
    //    Vector3 e1;
    //    Vector3 e2;

    //    quat = Quaternion.AngleAxis(90, Vector3.forward);
    //    s1 = quat * dir;
    //    s1.Set(s1.x + start.x, s1.y + start.y, 0);

    //    quat = Quaternion.AngleAxis(270, Vector3.forward);
    //    s2 = quat * dir;
    //    s2.Set(s2.x + start.x, s2.y + start.y, 0);


    //    //we need to create a square or points of thickness width and with aligned with the line..
}