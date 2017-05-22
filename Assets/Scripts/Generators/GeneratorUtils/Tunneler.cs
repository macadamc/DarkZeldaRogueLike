using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoreGame.Helper;

public class Tunneler
{

    sMap map;
    string layer;

    Vector2 Pos;
    Dictionary<int, Circle> Zones;
    Graph Connections;
    List<int> visited;
    List<int> fringe;
    Brush brush;

    //float step = 1f;

    public Tunneler(sMap Map, string LayerName, Graph conn, Dictionary<int, Circle> Zones)
    {
        map = Map;
        layer = LayerName;

        Connections = conn;
        this.Zones = Zones;

        fringe = new List<int>();
        visited = new List<int>();

        brush = new Brush(map, layer, 0);
    }

    public void Tunnel()
    {
        fringe.Add(1);

        while (fringe.Count != 0)
        {
            Circle parent = Zones[NextID()];
            visited.Add(parent.ID);

            foreach (int cID in Connections.GetAdjecent(parent.ID))
            {
                Pos = parent.centerPos;

                if (!visited.Contains(cID))
                {
                    Circle child = Zones[cID];

                    //float distance = Vector2.Distance(parent.centerPos, child.centerPos);

                    while (Pos != child.centerPos)
                    {
                        //float curDistance = Vector2.Distance(Pos, parent.centerPos);

                        //float percentage = (distance - curDistance) / distance;

                        //float radius = Mathf.Lerp(parent.radius, child.radius, percentage);

                        Pos = Vector2.MoveTowards(Pos, child.centerPos, 1f);
                        brush.DrawCircle(Pos, 2f);
                    }

                    brush.DrawCircle(Pos, child.radius - .5f);// -1 the zones can be right nect to each other 
                    fringe.Add(cID);
                }
            }
        }
    }

    public void DrawPath(List<int> ZoneIDs)
    {
        int index = 1; // dont draw the starting room;

        brush.TileID = map.GetTilesetByName("forestTileset").firstTileID + 59;

        while (ZoneIDs[index] != ZoneIDs[ZoneIDs.Count - 1])
        {
            Circle parent = Zones[ZoneIDs[index]];
            Pos = parent.centerPos;

            Circle child = Zones[ZoneIDs[index + 1]];

            while (Pos != child.centerPos)
            {
                Pos = Vector2.MoveTowards(Pos, child.centerPos, .25f);
                brush.DrawSquare(Mathf.FloorToInt(Pos.x), Mathf.FloorToInt(Pos.y), 1, 1);
            }
            index++;

        }
    }

    int NextID()
    {
        // randomly picking a id from the fringe would be more expensive but may look a litile nicer too.
        int id = fringe[fringe.Count - 1];
        fringe.RemoveAt(fringe.Count - 1);
        return id;
    }

}
