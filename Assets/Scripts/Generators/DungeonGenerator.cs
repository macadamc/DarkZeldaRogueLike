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

        map.SetLayerTo("Walls", 2);

        Vector2 mapSizeInRooms = new Vector2(map.width / RoomWidth, map.height / roomHeight);
        Rect mapRect = new Rect(0, 0, map.width, map.height);

        dungeon = new DungeonGraph(map);
        Queue<DungeonRoom> fringe = new Queue<DungeonRoom>();

        DungeonRoom r = dungeon.CreateRoom((RoomSize)rng.Next(4), new Vector2( rng.Next((int)mapSizeInRooms.x)  * RoomWidth, rng.Next((int)mapSizeInRooms.y) * roomHeight), this);
        while (Utility.Contains(mapRect, r.rect) == false || dungeon.RoomCanBePlaced(r) == false)
        {
            r.rect.position = new Vector2(rng.Next((int)mapSizeInRooms.x)  * RoomWidth, rng.Next((int)mapSizeInRooms.y) * roomHeight);
            r.roomSize = (RoomSize)rng.Next(4);
        }

        dungeon.AddRoom(r);
        r.Initalize(map);
        fringe.Enqueue(r);

        while (fringe.Count != 0)
        {
            DungeonRoom room = fringe.Dequeue();

            for (int xi = 0; xi < room.rect.width / RoomWidth; xi++)
            {
                for(int yi = 0; yi < room.rect.height / roomHeight; yi ++)
                {
                    RoomSize size = (RoomSize)rng.Next(4);
                    //RoomSize size = RoomSize.Wide;
                    //size = RoomSize.Small;
                    int cX = xi + ((int)room.rect.position.x / RoomWidth);
                    int cy = yi + ((int)room.rect.position.y / roomHeight);
                    // UP
                    DungeonRoom neibour;
                    if ((cX < mapSizeInRooms.x && cy + 1 < mapSizeInRooms.y) && dungeon.RoomUIDSByPos[cX, cy + 1] == 0)
                    {
                        neibour = dungeon.CreateRoom(size, new Vector2(cX * RoomWidth, (cy + 1) * roomHeight), this);
                        if (Utility.Contains(mapRect, neibour.rect) && dungeon.RoomCanBePlaced(neibour))
                        {
                            dungeon.AddRoom(neibour);
                            neibour.Initalize(map);
                            dungeon.CreateConnection(room, neibour);

                            fringe.Enqueue(neibour);

                        }
                    }
                    // RIGHT
                    if ((cX + 1 < mapSizeInRooms.x && cy < mapSizeInRooms.y) && dungeon.RoomUIDSByPos[cX + 1, cy] == 0)
                    {
                        neibour = dungeon.CreateRoom(size, new Vector2((cX + 1) * RoomWidth, cy * roomHeight), this);
                        if (Utility.Contains(mapRect, neibour.rect) && dungeon.RoomCanBePlaced(neibour))
                        {
                            dungeon.AddRoom(neibour);
                            neibour.Initalize(map);
                            dungeon.CreateConnection(room, neibour);

                            fringe.Enqueue(neibour);
                        }
                    }
                    // DOWN
                    if ((cX < mapSizeInRooms.x && cy - 1 >= 0) && dungeon.RoomUIDSByPos[cX, cy - 1] == 0)
                    {
                        neibour = dungeon.CreateRoom(size, new Vector2(cX * RoomWidth, (cy - 1) * roomHeight), this);
                        if (Utility.Contains(mapRect, neibour.rect) && dungeon.RoomCanBePlaced(neibour))
                        {
                            dungeon.AddRoom(neibour);
                            neibour.Initalize(map);
                            dungeon.CreateConnection(room, neibour);

                            fringe.Enqueue(neibour);
                        }
                    }
                    // LEFT
                    if ((cX - 1 >= 0 && cy < mapSizeInRooms.y) && dungeon.RoomUIDSByPos[cX - 1, cy] == 0)
                    {
                        neibour = dungeon.CreateRoom(size, new Vector2((cX - 1) * RoomWidth, cy * roomHeight), this);
                        if (Utility.Contains(mapRect, neibour.rect) && dungeon.RoomCanBePlaced(neibour))
                        {
                            dungeon.AddRoom(neibour);
                            neibour.Initalize(map);
                            dungeon.CreateConnection(room, neibour);

                            fringe.Enqueue(neibour);
                        }
                    }
                }
            }


        }

        foreach (DungeonRoom room in dungeon.rooms.Values)
        {
            room.Draw(map, this);

        }
    }
}

public class DungeonGraph
{
    static RoomConnection _edgePointer;
    static void _SetEdgePointer(int UID1, int UID2)
    {
        _edgePointer.room1 = UID1;
        _edgePointer.room2 = UID2;
    }

    public int NextUID;
    public Dictionary<int, DungeonRoom> rooms;
    public int[,] RoomUIDSByPos;

    public QuadTree<DungeonRoom> qTree;

    public DungeonGraph(sMap Map)
    {
        if (_edgePointer == null) { _edgePointer = new RoomConnection(-1, -1); }
        NextUID = 0;
        rooms = new Dictionary<int, DungeonRoom>();
        qTree = new QuadTree<DungeonRoom>(new Rect(Vector2.zero, new Vector2(Map.width, Map.height)));
        RoomUIDSByPos = new int[Map.width / DungeonGenerator.RoomWidth, Map.height / DungeonGenerator.roomHeight];
    }

    public DungeonRoom CreateRoom(RoomSize size, Vector2 Pos, DungeonGenerator Generator)
    {
        return new DungeonRoom(size, Pos, Generator);
    }
    public void RemoveRoom(DungeonRoom Room)
    {
        for(int i = 0; i < Room.connections.Count;i++)
        {
            RoomConnection con = Room.connections[i];
            int otherID = con.room1 == Room.UID ? con.room2 : con.room1;
            rooms[otherID].connections.Remove(con);
        }
        Room.connections.Clear();
        rooms.Remove(Room.UID);
        qTree.Delete(Room);
    }

    public void AddRoom(DungeonRoom room)
    {
        NextUID++;
        room.UID = NextUID;
        rooms.Add(room.UID, room);
        qTree.Insert(room);

        for(int x = 0; x < room.rect.width/DungeonGenerator.RoomWidth;x++)
        {
            for(int y = 0; y < room.rect.height / DungeonGenerator.roomHeight; y++)
            {
                RoomUIDSByPos[x + (int)room.rect.position.x / DungeonGenerator.RoomWidth, y + (int)room.rect.position.y / DungeonGenerator.roomHeight] = room.UID;
            }
        }
    }

    public void CreateConnection (DungeonRoom room1, DungeonRoom room2)
    {
        
        if (isConnected(room1, room2) == false)
        {
            RoomConnection connection = new RoomConnection(room1.UID, room2.UID);
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

        switch (doorID)
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

    public RoomConnection(int RoomUID, int OtherRoomUID)
    {
        room1 = RoomUID;
        room2 = OtherRoomUID;
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

        

    }

    public DungeonRoom(RoomSize roomSize, Vector2 Position, DungeonGenerator generator)
    {
        rect = new Rect();
        rect.position = Position;
        this.roomSize = roomSize;

        connections = new List<RoomConnection>();

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

        int halfWidth = Mathf.FloorToInt(DungeonGenerator.RoomWidth / 2 );
        int halfHeight = Mathf.FloorToInt(DungeonGenerator.roomHeight / 2);

        int rectX = Mathf.FloorToInt(rect.position.x);
        int rectY = Mathf.FloorToInt(rect.position.y);

        switch (roomSize)
        {
            case RoomSize.Small:
                return
                    (x == rectX + halfWidth - 1 || x == rectX + halfWidth)
                    && (y == rectY || y == rectY + DungeonGenerator.roomHeight - 1)
                    || (y == rectY + halfHeight - 1 || y == rectY + halfHeight)
                    && (x == rectX || x == rectX + DungeonGenerator.RoomWidth - 1);

            case RoomSize.Large:
                return
                    (x == rectX + halfWidth - 1 || x == rectX + halfWidth || x == rectX + DungeonGenerator.RoomWidth + halfWidth - 1 || x == rectX + DungeonGenerator.RoomWidth + halfWidth) 
                    && (y == rectY || y == rectY + (DungeonGenerator.roomHeight * 2) - 1)
                    || (y == rectY + halfHeight - 1 || y == rectY + halfHeight || y == rectY + DungeonGenerator.roomHeight + halfHeight - 1 || y == rectY + DungeonGenerator.roomHeight + halfHeight)
                    && (x == rectX || x == rectX + (DungeonGenerator.RoomWidth * 2) - 1);

            case RoomSize.Wide:
                return
                    (x == rectX + halfWidth - 1 || x == rectX + halfWidth || x == rectX + DungeonGenerator.RoomWidth + halfWidth - 1 || x == rectX + DungeonGenerator.RoomWidth + halfWidth)
                    && (y == rectY || y == rectY + DungeonGenerator.roomHeight - 1)
                    || (y == rectY + halfHeight - 1 || y == rectY + halfHeight)
                    && (x == rectX || x == rectX + (DungeonGenerator.RoomWidth * 2) - 1);

            case RoomSize.Long:
                return
                    (x == rectX + halfWidth - 1 || x == rectX + halfWidth)
                    && (y == rectY || y == rectY + (DungeonGenerator.roomHeight * 2) - 1)
                    || (y == rectY + halfHeight - 1 || y == rectY + halfHeight || y == rectY + DungeonGenerator.roomHeight + halfHeight - 1 || y == rectY + DungeonGenerator.roomHeight + halfHeight)
                    && (x == rectX || x == rectX + DungeonGenerator.RoomWidth - 1);
        }

        return false;
    }// somethings wrong with this maybe???

    public virtual void Draw(sMap map, DungeonGenerator genereator)
    {
        DungeonGraph dungeon = genereator.dungeon;
        int roomWidth = DungeonGenerator.RoomWidth;
        int roomHeight = DungeonGenerator.roomHeight;

        int roomStartChunkX = (int)rect.position.x / roomWidth;
        int RoomStartChunkY = (int)rect.position.y / roomHeight;

        int roomChunkWidth = (int)rect.size.x / roomWidth;
        int roomChunkHeight = (int)rect.size.y / roomHeight;

        for (int chunkX = roomStartChunkX; chunkX < roomChunkWidth + roomStartChunkX; chunkX++)
        {
            for (int chunkY = RoomStartChunkY; chunkY < roomChunkHeight + RoomStartChunkY; chunkY++)
            {
                for (int x = chunkX * roomWidth; x < (chunkX * roomWidth) + roomWidth; x++)
                {
                    for (int y = chunkY * roomHeight; y < (chunkY * roomHeight) + roomHeight; y++)
                    {

                        if (isEdgePosition(x, y))
                        {
                            if (isDoorPosition(x, y))
                            {
                                //UP
                                if (y == (chunkY * roomHeight) + roomHeight - 1 && x != chunkX * roomWidth && x != (chunkX * roomWidth) + roomWidth - 1)
                                {

                                    if (chunkY + 1 < map.height / roomHeight && dungeon.RoomUIDSByPos[chunkX, chunkY + 1] != 0 && dungeon.RoomUIDSByPos[chunkX, chunkY + 1] != UID && genereator.dungeon.isConnected(this, dungeon.rooms[dungeon.RoomUIDSByPos[chunkX, chunkY + 1]]))
                                    {
                                        map["Walls", x, y] = GameManager.GM.mapManager.GetTilesetByName("forestTileset").firstTileID + 48;
                                    }
                                    else
                                    {
                                        map["Walls", x, y] = GameManager.GM.mapManager.GetTilesetByName("forestTileset").firstTileID + 3;
                                    }

                                }
                                //RIGHT
                                else if (x == (chunkX * roomWidth) + roomWidth - 1 && y != chunkY * roomHeight && y != (chunkY * roomHeight) + roomHeight - 1)
                                {
                                    if (chunkX + 1 < map.width / roomWidth && dungeon.RoomUIDSByPos[chunkX + 1, chunkY] != 0 && dungeon.RoomUIDSByPos[chunkX + 1, chunkY] != UID && dungeon.isConnected(this, dungeon.rooms[dungeon.RoomUIDSByPos[chunkX + 1, chunkY]]))
                                    {
                                        map["Walls", x, y] = GameManager.GM.mapManager.GetTilesetByName("forestTileset").firstTileID + 48;
                                    }
                                    else
                                    {
                                        map["Walls", x, y] = GameManager.GM.mapManager.GetTilesetByName("forestTileset").firstTileID + 3;
                                    }

                                }
                                //DOWN
                                else if (y == chunkY * roomHeight && x != chunkX * roomWidth && x != (chunkX * roomWidth) + roomWidth - 1)
                                {
                                    if (chunkY - 1 >= 0 && dungeon.RoomUIDSByPos[chunkX, chunkY - 1] != 0 && dungeon.RoomUIDSByPos[chunkX, chunkY - 1] != UID && dungeon.isConnected(this, dungeon.rooms[genereator.dungeon.RoomUIDSByPos[chunkX, chunkY - 1]]))
                                    {
                                        map["Walls", x, y] = GameManager.GM.mapManager.GetTilesetByName("forestTileset").firstTileID + 48;
                                    }
                                    else
                                    {
                                        map["Walls", x, y] = GameManager.GM.mapManager.GetTilesetByName("forestTileset").firstTileID + 3;
                                    }

                                }
                                //Left
                                else if (x == chunkX * roomWidth && y != chunkY * roomHeight && y != (chunkY * roomHeight) + roomHeight - 1)
                                {
                                    if (chunkX - 1 >= 0 && dungeon.RoomUIDSByPos[chunkX - 1, chunkY] != 0 && dungeon.RoomUIDSByPos[chunkX - 1, chunkY] != UID && dungeon.isConnected(this, dungeon.rooms[dungeon.RoomUIDSByPos[chunkX - 1, chunkY]]))
                                    {
                                        map["Walls", x, y] = GameManager.GM.mapManager.GetTilesetByName("forestTileset").firstTileID + 48;
                                    }
                                    else
                                    {
                                        map["Walls", x, y] = GameManager.GM.mapManager.GetTilesetByName("forestTileset").firstTileID + 3;
                                    }
                                }
                            }
                            else
                            {
                                map["Walls", x, y] = GameManager.GM.mapManager.GetTilesetByName("forestTileset").firstTileID;

                                if(isDoorPosition(x,y))
                                {
                                    map["Walls", x, y] = GameManager.GM.mapManager.GetTilesetByName("forestTileset").firstTileID + 10;
                                }
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
}
