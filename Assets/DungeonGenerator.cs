using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuadTree;

[CreateAssetMenu]
public class DungeonGenerator : MapGenerator {

    public static int RoomWidth = 16;
    public static int roomHeight = 10;
    
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

        for (int x = 0; x < mapSizeInRooms.x; x++)
        {
            for(int y = 0; y < mapSizeInRooms.y; y++)
            {
                DungeonRoom r = new DungeonRoom((RoomSize)rng.Next(4), new Vector2(x * RoomWidth, y * roomHeight), this);

                if (Utility.Contains(new Rect(0,0, map.width, map.height), r.rect) == false)
                {
                    Debug.Log("Room Doesnt Fit in map");
                    continue;
                } 

                bool collision = false;

                res.Clear();
                qTree.GetObjects(r.rect, ref res);

                if (res.Count > 0)
                {
                    foreach (DungeonRoom room in res)
                    {
                        if (room.rect.Overlaps(r.rect))
                        {
                            
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
                    r.Initalize(map);
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
    public RoomConnections connections;

    public Rect rect;
    Rect IHasRectangle.Rect
    {
        get
        {
            return rect;
        }
    }

    public void Initalize(sMap map)
    {
        connections = new RoomConnections(roomSize);

        GameObject cameraZone = GameObject.Instantiate(Resources.Load("Bounds")) as GameObject;
        BoxCollider2D coll = cameraZone.GetComponent<BoxCollider2D>();
        coll.size = rect.size;
        cameraZone.transform.position = new Vector3(-(map.width / 2) + rect.position.x + (rect.size.x / 2), -(map.height / 2) + rect.position.y + (rect.size.y / 2));

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

    public bool isDoorPosition (int x,  int y)
    {
        int halfWidth = DungeonGenerator.RoomWidth / 2;
        int halfHeight = DungeonGenerator.roomHeight / 2;

        switch (roomSize)
        {
            case RoomSize.Small:
                return
                    (x == rect.position.x + halfWidth - 1|| x == rect.position.x + halfWidth) && (y == rect.position.y || y ==  rect.position.y + DungeonGenerator.roomHeight - 1)
                    || (y == rect.position.y + halfHeight  - 1 || y == rect.position.y + halfHeight) && (x == rect.position.x || x ==  rect.position.x + DungeonGenerator.RoomWidth - 1);
            
            case RoomSize.Large:
                return
                    (x == rect.position.x + halfWidth - 1 || x == rect.position.x + halfWidth || x == rect.position.x + DungeonGenerator.RoomWidth + halfWidth - 1 || x == rect.position.x + DungeonGenerator.RoomWidth + halfWidth) && (y == rect.position.y || y == rect.position.y + (DungeonGenerator.roomHeight * 2) - 1)
                    || (y == rect.position.y + halfHeight - 1 || y == rect.position.y + halfHeight || y == rect.position.y + DungeonGenerator.roomHeight + halfHeight - 1 || y == rect.position.y + DungeonGenerator.roomHeight + halfHeight) && (x == rect.position.x || x == rect.position.x + (DungeonGenerator.RoomWidth * 2) - 1);

            case RoomSize.Wide:
                return
                    (x == rect.position.x + halfWidth - 1 || x == rect.position.x + halfWidth || x == rect.position.x + DungeonGenerator.RoomWidth + halfWidth - 1 || x == rect.position.x + DungeonGenerator.RoomWidth + halfWidth) && (y == rect.position.y || y == rect.position.y + DungeonGenerator.roomHeight - 1)
                    || (y == rect.position.y + halfHeight - 1 || y == rect.position.y + halfHeight) && (x == rect.position.x || x == rect.position.x + (DungeonGenerator.RoomWidth * 2) - 1);
            
            case RoomSize.Long:
                return
                    (x == rect.position.x + halfWidth - 1 || x == rect.position.x + halfWidth) && (y == rect.position.y || y == rect.position.y + (DungeonGenerator.roomHeight * 2) - 1)
                    || (y == rect.position.y + halfHeight - 1 || y == rect.position.y + halfHeight || y == rect.position.y + DungeonGenerator.roomHeight + halfHeight - 1 || y == rect.position.y + DungeonGenerator.roomHeight + halfHeight)  && (x == rect.position.x || x == rect.position.x + DungeonGenerator.RoomWidth - 1);
        }

        return false;
    }

    public void Draw(sMap map)
    {
        for (int x = Mathf.FloorToInt(rect.position.x); x < rect.position.x + rect.size.x; x++)
        {
            for (int y = Mathf.FloorToInt(rect.position.y); y < rect.position.y + rect.size.y; y++)
            {
                if (map.inBounds(x, y))
                {
                    if (isEdgePosition(x, y))
                    {
                        if (isDoorPosition(x, y))
                        {
                            map["Walls", x, y] = GameManager.GM.mapManager.GetTilesetByName("forestTileset").firstTileID + 49;
                        }
                        else
                        {
                            map["Walls", x, y] = GameManager.GM.mapManager.GetTilesetByName("forestTileset").firstTileID;
                        }
                        
                    }
                    else
                    {
                        map["Walls", x, y] = GameManager.GM.mapManager.GetTilesetByName("forestTileset").firstTileID + 49;
                    }
                }              
            }
        }
    }
}

public class RoomConnections
{
    int[] top;
    int[] bottom;
    int[] right;
    int[] left;

    public RoomConnections(RoomSize roomSize)
    {
        switch(roomSize)
        {
            case RoomSize.Small:
                top = new int[1];
                bottom = new int[1];
                left = new int[1];
                right = new int[1];
                break;
            case RoomSize.Large:
                top = new int[2];
                bottom = new int[2];
                left = new int[2];
                right = new int[2];
                break;
            case RoomSize.Wide:
                top = new int[2];
                bottom = new int[2];
                left = new int[1];
                right = new int[1];
                break;
            case RoomSize.Long:
                top = new int[1];
                bottom = new int[1];
                left = new int[2];
                right = new int[2];
                break;
        }
    }
}
