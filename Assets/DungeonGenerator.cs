using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuadTree;

[CreateAssetMenu]
public class DungeonGenerator : MapGenerator {

    public static int RoomWidth = 26;
    public static int roomHeight = 16;
    
    [NonSerialized]
    public Graph Connections;
    [NonSerialized]
    public Dictionary<int, DungeonRoom> Rooms;
    [NonSerialized]
    public List<int> zIds;
    [NonSerialized]
    QuadTree<DungeonRoom> qTree;

    void Initalize(sMap Map)
    {
        Connections = new Graph();
        Rooms = new Dictionary<int, DungeonRoom>();
        zIds = new List<int>();
        qTree = new QuadTree<DungeonRoom>(new Rect(Vector2.zero, new Vector2(Map.width, Map.height)));
    }

    public override void Generate(sMap map, DefaultRNG rng, EntityMetaDataSO entityData, LevelGenerator LvlGenerator)
    {

        Initalize(map);

        Vector2 mapSizeInRooms = new Vector2(map.width / RoomWidth, map.height / roomHeight);

        Debug.Log(string.Format("W: {0} H: {1}", mapSizeInRooms.x, mapSizeInRooms.y));
        List<DungeonRoom> res = new List<DungeonRoom>();

        for(int x = 0; x < mapSizeInRooms.x; x++)
        {
            for(int y = 0; y < mapSizeInRooms.y; y++)
            {
                DungeonRoom r = new DungeonRoom((RoomSize)rng.Next(4), new Vector2(x * RoomWidth, y * roomHeight), this);
                bool collision = false;

                res.Clear();
                qTree.GetObjects(r.rect, ref res);

                if (res.Count > 0)
                {
                    foreach (DungeonRoom room in res)
                    {
                        ;
                        if (room.rect.Overlaps(r.rect))
                        {
                            //Debug.Log(string.Format("r1: {0}, {1},  r2: {2}, {3}", r.rect.position, r.rect.size, room.rect.position, room.rect.size));
                            collision = true;
                            break;
                            
                        }
                    }
                }
                
                if (collision == false)
                {
                    qTree.Insert(r);

                    r.ID = Connections.createNode();

                    Rooms.Add(r.ID, r);
                    zIds.Add(r.ID);
                    r.Draw(map);
                }

            }
        }
    }

}

public enum RoomSize { Small, Large, Long, Wide};

public class DungeonRoom : IHasRectangle
{
    public int ID = -1;
    public RoomSize roomSize;

    public Rect rect;
    Rect IHasRectangle.Rect
    {
        get
        {
            return rect;
        }
    }

    public DungeonRoom(RoomSize roomSize, Vector2 Position, DungeonGenerator generator)
    {
        rect = new Rect();
        rect.position = Position;
        this.roomSize = roomSize;

        switch (roomSize)
        {
            case RoomSize.Small:
                rect.size = new Vector2(DungeonGenerator.RoomWidth, DungeonGenerator.roomHeight);
                break;
            case RoomSize.Large:
                rect.size = new Vector2(DungeonGenerator.RoomWidth * 2, DungeonGenerator.roomHeight * 2);
                break;
            case RoomSize.Long:
                rect.size = new Vector2(DungeonGenerator.RoomWidth, DungeonGenerator.roomHeight * 2);
                break;
            case RoomSize.Wide:
                rect.size = new Vector2(DungeonGenerator.RoomWidth * 2, DungeonGenerator.roomHeight);
                break;
        }
    }

    public bool isEdgePosition(int x, int y)
    {
        return x == rect.position.x || y == rect.position.y || x == rect.position.x + rect.size.x - 1 || y == rect.position.y + rect.size.y - 1;
    }

    public bool inBounds(int x, int y)
    {
        return (x >= rect.position.x && x < rect.position.x + rect.size.x) && (y >= rect.position.y && y < rect.position.y + rect.size.y);
    }

    public void Draw(sMap map)
    {
        Debug.Log(map.width + "," + map.height);
        for (int x = Mathf.FloorToInt(rect.position.x); x < rect.position.x + rect.size.x; x++)
        {
            for (int y = Mathf.FloorToInt(rect.position.y); y < rect.position.y + rect.size.y; y++)
            {
                if (map.inBounds(x, y))
                {
                    if (map.isEdge(x, y))
                    {
                        map["Walls", x, y] = GameManager.GM.mapManager.GetTilesetByName("forestTileset").firstTileID + 3;
                    }

                    if (isEdgePosition(x, y))
                    {
                        map["Walls", x, y] = GameManager.GM.mapManager.GetTilesetByName("forestTileset").firstTileID;
                    }
                    else
                    {
                        //map["Walls", x, y] = GameManager.GM.mapManager.GetTilesetByName("forestTileset").firstTileID + 49;
                    }
                }
                
                
            }
        }
    }
}
