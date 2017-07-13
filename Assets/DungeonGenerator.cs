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

    void Initalize(sMap Map)
    {
        
    }

    public override void Generate(sMap map, DefaultRNG rng, EntityMetaDataSO entityData, LevelGenerator LvlGenerator)
    {

        Initalize(map);

        map.SetLayerTo("Walls", 1);

        Vector2 mapSizeInRooms = new Vector2(map.width / RoomWidth, map.height / roomHeight);
        Rect mapRect = new Rect(0, 0, map.width, map.height);

        dungeon = new DungeonGraph(map);

        for (int x = 0; x < mapSizeInRooms.x; x++)
        {
            for (int y = 0; y < mapSizeInRooms.y; y++)
            {
                DungeonRoom r = dungeon.CreateRoom((RoomSize)rng.Next(4), new Vector2(x * RoomWidth, y * roomHeight), this);

                if (Utility.Contains(mapRect, r.rect) == false || dungeon.RoomCanBePlaced(r) == false)
                {
                    dungeon.RemoveRoom(r);
                    continue;
                }

                else
                {
                    dungeon.qTree.Insert(r);
                    r.Initalize(map);
                }

            }
        }
        foreach (DungeonRoom room in dungeon.rooms.Values)
        {
            List<DungeonRoom> neibours = dungeon.GetAdjecentRooms(room);

            if (neibours.Count == 0 ) { continue; }

            DungeonRoom other = neibours[rng.Next(neibours.Count)];

            Vector2 dif = room.rect.position - other.rect.position;
            Debug.Log(new Vector2(dif.x/26, dif.y/16));

            //dungeon.CreateConnection(room, other, 0, 0);
            
        }

        foreach (DungeonRoom room in dungeon.rooms.Values)
        {
            room.Draw(map);
        }
    }


    //public override void Generate(sMap map, DefaultRNG rng, EntityMetaDataSO entityData, LevelGenerator LvlGenerator)
    //{

    //    Initalize(map);

    //    map.SetLayerTo("Walls", 1);

    //    Vector2 mapSizeInRooms = new Vector2(map.width / RoomWidth, map.height / roomHeight);
    //    Rect mapRect = new Rect(0, 0, map.width, map.height);

    //    dungeon = new DungeonGraph(map);

    //    int x = rng.Next((int)mapSizeInRooms.x);
    //    int y = rng.Next((int)mapSizeInRooms.y);

    //    DungeonRoom r = dungeon.CreateRoom((RoomSize)rng.Next(4), new Vector2(x * RoomWidth, y * roomHeight), this);
    //    List<int> doorids = null;

    //    switch(r.roomSize)
    //    {
    //        case RoomSize.Small:
    //            doorids = DungeonRoom.SmallDoorIDs;
    //            break;
    //        case RoomSize.Long:
    //            doorids = DungeonRoom.LongDoorIDs;
    //            break;
    //        case RoomSize.Wide:
    //            doorids = DungeonRoom.WideDoorIds;
    //            break;
    //        case RoomSize.Large:
    //            doorids = DungeonRoom.LargeDoorIds;
    //            break;
    //    }


    //    if (Utility.Contains(mapRect, r.rect) == false || dungeon.RoomCanBePlaced(r) == false)
    //    {
    //        dungeon.RemoveRoom(r);
    //    }

    //    else
    //    {
    //        dungeon.qTree.Insert(r);

    //        r.Initalize(map);
    //        r.Draw(map);


    //        Vector2 nPos = dungeon.GetAdjacentRoomPos(r, doorids[rng.Next(doorids.Count)]);
    //        r = dungeon.CreateRoom(RoomSize.Small, new Vector2(nPos.x * RoomWidth, nPos.y * roomHeight), this);
    //        r.Initalize(map);
    //        r.Draw(map);
    //    }


    //}

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
    static RoomConnection _edgePointer = new RoomConnection(-1, -1, -1, -1);
    static void _SetEdgePointer(int UID1, int UID2)
    {
        _edgePointer.room1 = UID1;
        _edgePointer.room2 = UID2;
    }

    public int NextUID;
    public Dictionary<int, DungeonRoom> rooms;

    public QuadTree<DungeonRoom> qTree;

    public DungeonGraph(sMap Map)
    {
        NextUID = 0;
        rooms = new Dictionary<int, DungeonRoom>();
        qTree = new QuadTree<DungeonRoom>(new Rect(Vector2.zero, new Vector2(Map.width, Map.height)));
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

    public void CreateConnection (DungeonRoom room1, DungeonRoom room2, int doorID1, int doorID2)
    {
        
        if (isConnected(room1, room2) == false)
        {
            RoomConnection connection = new RoomConnection(room1.UID, room2.UID, doorID1, doorID2);
            room1.connections.Add(connection);
            room2.connections.Add(connection);
        }
    }
    public void RemoveConnection(DungeonRoom room1, DungeonRoom room2)
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

    public RoomConnection GetConnection (DungeonRoom room1, DungeonRoom room2)
    {
        if (isConnected(room1, room2))
        {
            return room1.connections[room1.connections.IndexOf(_edgePointer)];
        }
        return null;
    }

    public List<DungeonRoom> GetAdjecentRooms (DungeonRoom room)
    {
        List<DungeonRoom> ret = new List<DungeonRoom>();
        qTree.GetObjects(new Rect(room.rect.position - Vector2.one, room.rect.size + Vector2.one), ref ret);
        ret.Remove(room);
        return ret;
    }

    public bool RoomCanBePlaced (DungeonRoom room)
    {
        List<DungeonRoom> res = new List<DungeonRoom>();
        qTree.GetObjects(room.rect, ref res);

        if (res.Count > 0)
        {
            foreach (DungeonRoom otherRoom in res)
            {
                if (room.rect.Overlaps(otherRoom.rect))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public Vector2 GetAdjacentRoomPos(DungeonRoom room, int doorID)
    {
        int roomX = (int)room.rect.position.x / 26;
        int roomY = (int)room.rect.position.y / 16;

        switch(doorID)
        {
            case 1:
                return new Vector2(roomX, roomY + 1);
            case 2:
                return new Vector2(roomX + 1, roomY);
            case 3:
                return new Vector2(roomX, roomY - 1);
            case 4:
                return new Vector2(roomX - 1, roomY);
            case 5:
                return new Vector2(roomX + 1, roomY + 1);
            case 6:
                return new Vector2(roomX + 2, roomY);
            case 7:
                return new Vector2(roomX + 1, roomY - 1);
            case 8:
                return new Vector2(roomX, roomY + 2);
            case 9:
                return new Vector2(roomX + 1, roomY + 1);
            case 10:
                return new Vector2(roomX - 1, roomY + 1);
            case 11:
                return new Vector2(roomX + 1, roomY + 2);
            case 12:
                return new Vector2(roomX + 2, roomY + 1);
        }
        throw new Exception("DoorID Doesnt Exist");
    }

}

public class RoomConnection
{
    public int[] nodeIds;
    public int room1;
    public int door1;
    public int room2;
    public int door2;

    public RoomConnection(int RoomUID, int OtherRoomUID, int DoorID, int OtherDoorID)
    {
        room1 = RoomUID;
        door1 = DoorID;

        room2 = OtherRoomUID;
        door2 = OtherDoorID;
    }

    public override bool Equals(object obj)
    {
        RoomConnection other = (RoomConnection)obj;
        if (other != null && ((room1 == other.room1 || room1 == other.room2) && (room2 == other.room1 || room2 == other.room2)) 
            )//&& ((door1 == other.door1 || door1 == other.door2) && (door2 == other.door1 || door2 == other.door2)))
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
    public static List<int> SmallDoorIDs = new List<int>() { 1, 2, 3, 4 };
    public static List<int> LongDoorIDs = new List<int>() { 2, 3, 4, 8, 9, 10 };
    public static List<int> WideDoorIds = new List<int>() { 1, 3, 4, 5, 6, 7 };
    public static List<int> LargeDoorIds = new List<int>() { 3, 4, 6, 7, 8, 10, 11, 12 };

    public static List<Vector2[]> DoorPositions = new List<Vector2[]>()
    {
        new Vector2[2] { new Vector2(13, 16), new Vector2(14, 16) },
        new Vector2[2] { new Vector2(26, 8), new Vector2(26, 9) },
        new Vector2[2] { new Vector2(13, 0), new Vector2(14, 0) },
        new Vector2[2] { new Vector2(0, 8), new Vector2(0, 9) },
        new Vector2[2] { new Vector2(26+13, 16), new Vector2(26+14, 16) },
        new Vector2[2] { new Vector2(52, 8), new Vector2(52, 9) },
        new Vector2[2] { new Vector2(26+13, 0), new Vector2(26+14, 0) },
        new Vector2[2] { new Vector2(13, 32), new Vector2(14, 32) },
        new Vector2[2] { new Vector2(26, 16+8), new Vector2(26, 16+9) },
        new Vector2[2] { new Vector2(0, 16+8), new Vector2(0, 16+9) },
        new Vector2[2] { new Vector2(26+13, 32), new Vector2(26+14, 32) },
        new Vector2[2] { new Vector2(52, 16+8), new Vector2(52, 16+9) }

    };

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
                if (isEdgePosition(x, y))
                {
                    map["Walls", x, y] = GameManager.GM.mapManager.GetTilesetByName("forestTileset").firstTileID;
                }
                else
                {
                    map["Walls", x, y] = GameManager.GM.mapManager.GetTilesetByName("forestTileset").firstTileID + 49;

                }
            }
        }

        foreach(RoomConnection connection in connections)
        {
            int doorID;
            if (connection.room1 == UID)
            {
                doorID = connection.door1;
            }
            else
            {
                doorID = connection.door2;
            }

            foreach(Vector2 pos in DungeonRoom.DoorPositions[doorID])
            {
                map["Walls", (int)pos.x, (int)pos.y] = GameManager.GM.mapManager.GetTilesetByName("forestTileset").firstTileID + 49;
            }
        }
    }
}
