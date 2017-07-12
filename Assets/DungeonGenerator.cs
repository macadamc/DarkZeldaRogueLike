using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuadTree;

public enum RoomSize { Small, Large, Long, Wide };
public enum Sides { Top, Right, Bottom, Left };

[CreateAssetMenu]
public class DungeonGenerator : MapGenerator {

    public static int RoomWidth = 26;
    public static int roomHeight = 16;
    
    [NonSerialized]
    public DungeonGraph dungeon;

    [NonSerialized]
    QuadTree<DungeonRoom> qTree;

    void Initalize(sMap Map)
    {
        qTree = new QuadTree<DungeonRoom>(new Rect(Vector2.zero, new Vector2(Map.width, Map.height)));
    }

    public override void Generate(sMap map, DefaultRNG rng, EntityMetaDataSO entityData, LevelGenerator LvlGenerator)
    {
        Initalize(map);

        map.SetLayerTo("Walls", 1);
        Vector2 mapSizeInRooms = new Vector2(map.width / RoomWidth, map.height / roomHeight);
        List<DungeonRoom> res = new List<DungeonRoom>();

        dungeon = new DungeonGraph();

        for (int x = 0; x < mapSizeInRooms.x; x++)
        {
            for (int y = 0; y < mapSizeInRooms.y; y++)
            {
                DungeonRoom r = dungeon.CreateRoom((RoomSize)rng.Next(4), new Vector2(x * RoomWidth, y * roomHeight), this);

                if (Utility.Contains(new Rect(0, 0, map.width, map.height), r.rect) == false)
                {
                    dungeon.RemoveRoom(r);
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

                    r.Initalize(map);
                    r.Draw(map);

                }

            }
        }
    }

}

//public class RoomConnections
//{
//    public int[] top;
//    public int[] bottom;
//    public int[] right;
//    public int[] left;

//    public List<int[]> connections;

//    public RoomConnections(RoomSize roomSize)
//    {
//        switch(roomSize)
//        {
//            case RoomSize.Small:
//                top = new int[1];
//                bottom = new int[1];
//                left = new int[1];
//                right = new int[1];
//                break;
//            case RoomSize.Large:
//                top = new int[2];
//                bottom = new int[2];
//                left = new int[2];
//                right = new int[2];
//                break;
//            case RoomSize.Wide:
//                top = new int[2];
//                bottom = new int[2];
//                left = new int[1];
//                right = new int[1];
//                break;
//            case RoomSize.Long:
//                top = new int[1];
//                bottom = new int[1];
//                left = new int[2];
//                right = new int[2];
//                break;
//        }
//        connections = new List<int[]> ();
//        connections.Add(top);
//        connections.Add(right);
//        connections.Add(bottom);
//        connections.Add(left);
//        Debug.Log(connections[(int)Sides.Top].Length);
//    }
//}

public class DungeonGraph
{
    static RoomConnection _edgePointer = new RoomConnection(-1, -1);
    static void _SetEdgePointer(int UID1, int UID2)
    {
        _edgePointer.nodeIds[0] = UID1;
        _edgePointer.nodeIds[1] = UID2;
    }

    public int NextUID;
    public Dictionary<int, DungeonRoom> rooms;

    public DungeonGraph()
    {
        NextUID = 0;
        rooms = new Dictionary<int, DungeonRoom>();
    }

    public DungeonRoom CreateRoom(RoomSize size, Vector2 Pos, DungeonGenerator Generator)
    {
        DungeonRoom room = new DungeonRoom(size, Pos, Generator);
        NextUID++;
        room.UID = NextUID;
        rooms.Add(room.UID, room);

        return room;
    }
    public void RemoveRoom(DungeonRoom Room)
    {
        rooms.Remove(Room.UID);
    }

    public void CreateEdge (DungeonRoom room1, DungeonRoom room2)
    {
        
        if (isConnected(room1, room2) == false)
        {
            RoomConnection edge = new RoomConnection(room1.UID, room2.UID);
            room1.connections.Add(edge);
            room2.connections.Add(edge);
        }
    }
    public void RemoveEdge(DungeonRoom room1, DungeonRoom room2)
    {
        
        if (isConnected(room1, room2))
        {
            _SetEdgePointer(room1.UID, room2.UID);

            room1.connections.Remove(_edgePointer);
            room2.connections.Remove(_edgePointer);
        }
    }

    public bool isConnected(DungeonRoom room1, DungeonRoom room2)
    {
        _SetEdgePointer(room1.UID, room2.UID);
        return room1.connections.Contains(_edgePointer);
    }

}

public class RoomConnection
{
    public int[] nodeIds;

    public RoomConnection(int NodeID, int OtherNodeID)
    {
        nodeIds = new int[2] { NodeID, OtherNodeID };
    }

    public override bool Equals(object obj)
    {
        RoomConnection other = (RoomConnection)obj;

        // test to see if both ids are in the other object in any order. if they are then these objects are considered the same object.
        if (other is RoomConnection && Array.Exists<int>(other.nodeIds, element => element == nodeIds[0]) && Array.Exists<int>(other.nodeIds, element => element == nodeIds[1]))
        {
            return true;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

}

public class DungeonRoom : IHasRectangle
{
    public int UID;
    public List<RoomConnection> connections;
    public RoomSize roomSize;
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
        // create and set up this rooms cameraZone.
        GameObject cameraZone = GameObject.Instantiate(Resources.Load("Bounds")) as GameObject;
        BoxCollider2D coll = cameraZone.GetComponent<BoxCollider2D>();
        coll.size = rect.size;
        cameraZone.transform.position = new Vector3(-(map.width / 2) + rect.position.x + (rect.size.x / 2), -(map.height / 2) + rect.position.y + (rect.size.y / 2));

        connections = new List<RoomConnection>();

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

    public bool isDoorPosition(int x, int y)
    {

        int halfWidth = DungeonGenerator.RoomWidth / 2;
        int halfHeight = DungeonGenerator.roomHeight / 2;

        switch (roomSize)
        {
            case RoomSize.Small:
                return
                    (x == rect.position.x + halfWidth - 1 || x == rect.position.x + halfWidth) && (y == rect.position.y || y == rect.position.y + DungeonGenerator.roomHeight - 1)
                    || (y == rect.position.y + halfHeight - 1 || y == rect.position.y + halfHeight) && (x == rect.position.x || x == rect.position.x + DungeonGenerator.RoomWidth - 1);

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
                    || (y == rect.position.y + halfHeight - 1 || y == rect.position.y + halfHeight || y == rect.position.y + DungeonGenerator.roomHeight + halfHeight - 1 || y == rect.position.y + DungeonGenerator.roomHeight + halfHeight) && (x == rect.position.x || x == rect.position.x + DungeonGenerator.RoomWidth - 1);
        }

        return false;
    }

    public virtual void Draw(sMap map)
    {
        for (int x = Mathf.FloorToInt(rect.position.x); x < rect.position.x + rect.size.x; x++)
        {
            for (int y = Mathf.FloorToInt(rect.position.y); y < rect.position.y + rect.size.y; y++)
            {
                if (isEdgePosition(x, y) && isDoorPosition(x, y) == false)
                {
                    map["Walls", x, y] = GameManager.GM.mapManager.GetTilesetByName("forestTileset").firstTileID;
                }
                else
                {
                    if (map.isEdge(x, y, true))
                    {
                        map["Walls", x, y] = GameManager.GM.mapManager.GetTilesetByName("forestTileset").firstTileID;
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
