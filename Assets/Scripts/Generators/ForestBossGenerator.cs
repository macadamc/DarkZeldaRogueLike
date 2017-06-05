using UnityEngine;
using System.Collections.Generic;
using CoreGame.Helper;
using QuadTree;
using ShadyPixel.CameraSystem;
using System;

[CreateAssetMenu()]
public class ForestBossGenerator : MapGenerator
{
    GameObject TerrainGameObjects;
    GameObject Chests;
    GameObject Spawners;
    TileMapManager mManager;

    public Graph Connections;
    [NonSerialized]
    public Dictionary<int, Circle> Zones;
    [NonSerialized]
    public List<int> zIds;

    QuadTree<Circle> qTree;

    public Dictionary<int, List<Vector3>> zoneSpawnPoints;

    float MAXDISTFORCONNECTION = 3;

    public override void Generate(sMap map, DefaultRNG rng, EntityMetaDataSO entityData, LevelGenerator lvlGenerator)
    {
        InGameObjectManager ObjManager = GameManager.GM.InGameObjectManager;
        mManager = GameManager.GM.mapManager;

        TerrainGameObjects = ObjManager.GetContainer("TerrainObjects");
        Chests = ObjManager.GetContainer("Chests");
        Spawners = ObjManager.GetContainer("Spawners");

        Connections = new Graph();
        Zones = new Dictionary<int, Circle>();
        zIds = new List<int>();
        qTree = new QuadTree<Circle>(0, 0, map.width, map.height);

        zoneSpawnPoints = new Dictionary<int, List<Vector3>>();

        int id = Connections.createNode();

        Circle sC = new Circle(-Vector2.one, 14);
        sC.ID = id;

        sC.centerPos = new Vector2(map.width / 2f, map.height / 2f);

        qTree.Insert(sC);
        Zones.Add(id, sC);
        zIds.Add(id);
        
        int sID = 0;

        try
        {
            int tID = createRoom(rng, "Empty", 6, 4, 1);
            tID = createRoom(rng, "Empty", 4, 4, tID);
            tID = createRoom(rng, "Empty", 4, 2, tID);
            sID = createRoom(rng, "Start", 3, 3, tID);
        }
        catch
        {
            Generate(map, rng, entityData, lvlGenerator);
        }

        CreateWalls(map, rng);

        //List<int> path = new List<int>();
        //Utility.GetPath(sID, 1, Connections, Zones, ref path);
        ////path.Insert(0,1);
        //Tunneler tunneler = new Tunneler(map, "Bg", Connections, Zones);
        //tunneler.DrawPath(path);

        CreateFloor(map, rng);

        GenerateSpawnPoints(id, map, rng, 150);

        SpawnBigTrees(id, 32, 1000, map, rng);

        SpawnBushes(id, rng);

        //set the players position to the first room.
        Circle sZone = Zones[sID];
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = new Vector3(sZone.centerPos.x - (map.width / 2), sZone.centerPos.y - (map.height / 2), player.transform.position.z);
    }

    void CreateFloor(sMap Map, DefaultRNG Rng)
    {
        //ground tileIDs; TILEID START AT 1 FOR EACH TILESET!
        int[] grass = new int[3] { 49, 50, 51 };
        int[] flowers = new int[2] { 52, 53 };
        int[] mushrooms = new int[2] { 54, 55 };

        Tileset ground = mManager.GetTilesetByName("forestTileset");

        for (int y = 0; y < Map.height; y++)
        {
            for (int x = 0; x < Map.width; x++)
            {
                Map["Floor", x, y] = ground.firstTileID + 48;

                if (Rng.NextDouble() > 1f - .5f)
                    continue;

                if (Map["Walls", x, y] == 0)
                {
                    if (Rng.NextDouble() <= .1f)
                    {
                        Map["Bg", x, y] = ground.firstTileID + flowers[Rng.Next(0, flowers.Length)];
                    }
                    else if (Rng.NextDouble() <= .1f)
                    {
                        Map["Bg", x, y] = ground.firstTileID + mushrooms[Rng.Next(0, mushrooms.Length)];
                    }
                    else if (Rng.NextDouble() <= .75)
                    {
                        Map["Bg", x, y] = ground.firstTileID + grass[Rng.Next(0, grass.Length)];
                    }
                }
                else
                {
                    if (Rng.NextDouble() <= .25f)
                    {
                        Map["Bg", x, y] = ground.firstTileID + grass[Rng.Next(0, grass.Length)];
                    }
                }

            }
        }
    }

    void CreateWalls(sMap Map, DefaultRNG Rng)
    {
        int fid = mManager.GetTilesetByName("forestTileset").firstTileID;

        int[] smallTrees = new int[4] { fid, fid + 1, fid + 2, fid + 3 };
        List<int> weights = new List<int>(new int[4] { 3, 3, 10, 1 });
        int totalweight = 17;

        //Fill the map with wall tiles.
        for (int y = 0; y < Map.height; y++)
        {
            for (int x = 0; x < Map.width; x++)
            {
                if (Rng.NextDouble() <= .05f && ((x != 0 && x != Map.width - 1) && (y != 0 && y != Map.height - 1)))
                {
                    Map["Walls", x, y] = 0;
                }
                else
                {
                    Map["Walls", x, y] = Rng.WeightedChoice(smallTrees, weights, totalweight);
                }
            }
        }

        // Dig out rooms and passages.
        Tunneler tunneler = new Tunneler(Map, "Walls", Connections, Zones);
        tunneler.Tunnel();


        //Draw BigTrees over over other wall tiles that are not big trees randomly.
        Tileset t = mManager.GetTilesetByName("forestTileset");

        List<int> spriteIDs = new List<int>();

        spriteIDs.AddRange(new int[4] {
            t.firstTileID + 18,
            t.firstTileID + 19,
            t.firstTileID + 10,
            t.firstTileID + 11
        });

        int trys = 1000;// teehee..

        for (int i = 0; i < trys; i++)
        {
            int rX = Rng.Next(0, Map.width);
            int rY = Rng.Next(0, Map.height);

            bool inbounds = true;

            for (int x = rX; x <= rX + 1; x++)
            {
                for (int y = rY; y <= rY + 1; y++)
                {
                    if (Map.inBounds(x, y) == false || (Map["Walls", x, y] <= 0 || (spriteIDs.Contains(Map["Walls", x, y]) || spriteIDs.Contains(Map["Fg", x, y])) || (rX == 0 || rX + 1 == Map.width - 1) || (rY == 0 || rY + 1 == Map.height - 1)))
                    {
                        inbounds = false;
                        break;
                    }
                }
                if (inbounds == false) { break; }
            }

            if (inbounds == true)
            {
                DrawBigTree(rX, rY, Map);
            }
        }
    }

    void DrawBigTree(int x, int y, sMap Map)
    {
        Tileset t = mManager.GetTilesetByName("forestTileset");

        //bottomleft
        Map["Walls", x, y] = t.firstTileID + 18;
        //bottomRight
        Map["Walls", x + 1, y] = t.firstTileID + 19;
        //topLeft
        Map["Fg", x, y + 1] = t.firstTileID + 10;
        Map["Walls", x, y + 1] = 0;
        //potRight
        Map["Fg", x + 1, y + 1] = t.firstTileID + 11;
        Map["Walls", x + 1, y + 1] = 0;
    }

    int createRoom(DefaultRNG rng, string Tag, float maxRadius, float minRadius, int parentID)
    {
        int trys = 1000;

        bool created = false;

        while (created == false && trys > 0)
        {
            trys--;
            Vector2 RVector = new Vector2(rng.Rand(), rng.Rand()).normalized;
            float radius = Mathf.RoundToInt((float)(rng.NextDouble() * (maxRadius - minRadius)) + minRadius);

            RVector = Zones[parentID].centerPos + (RVector * (Zones[parentID].radius + radius));

            Circle c = new Circle(new Vector2(RVector.x, RVector.y), radius);
            c.centerPos = Utility.DDALine(Zones[parentID], c);// sets the position to the closest intger position that doesnt collide with the other circle..

            List<Circle> res = new List<Circle>();
            qTree.GetObjects(new Rect((c.centerPos.x - radius) - MAXDISTFORCONNECTION, (c.centerPos.y - radius) - MAXDISTFORCONNECTION, (radius * 2) + MAXDISTFORCONNECTION, radius * 2 + MAXDISTFORCONNECTION), ref res);

            bool collision = false;
            foreach (Circle circle in res)
            {
                if (Circle.Intersect(c, circle))
                {
                    collision = true;
                    break;
                }
            }
            // if the new circle is in the rect of the quad tree and threres no collisions with other circles,
            // we add the new circle to the quadtree, create a new node in the graph, and store the circle in the zones dictionary with the nodes id.
            // then finaly we add a connection between the 2 circles.(a connection stores each nodes id in the conections dictionary so we can look up what each nodes individual connections are.)
            if (Utility.Contains(qTree.QuadRect, c) && !collision)
            {
                qTree.Insert(c);

                int nID = Connections.createNode();
                c.ID = nID;
                Zones.Add(nID, c);
                zIds.Add(nID);

                Connections.addConnection(parentID, nID);

                c.Tag = Tag;
                created = true;
                return nID;
            }
        }
        
        throw new Exception();
    }

    Vector3 GetRandomSpawnPoint(int id, DefaultRNG Rng)
    {
        List<Vector3> points = zoneSpawnPoints[id];
        int index = Rng.Next(0, points.Count);
        Vector3 pos = points[index];
        points.RemoveAt(index);

        return pos;
    }

    void SpawnBigTrees(int id, int maxPerZone, int maxTrys, sMap Map, DefaultRNG Rng)
    {
        Circle zone = Zones[id];
        List<Vector3> SpawnedObjectPos = new List<Vector3>();

        List<Vector3> spawnPoints = zoneSpawnPoints[id];

        int numberofTrees = 0;
        int count = 0;

        while (numberofTrees < maxPerZone && count < maxTrys)
        {
            count++;

            int index = Rng.Next(0, spawnPoints.Count);
            Vector3 pos = spawnPoints[index];

            int xActual = Mathf.FloorToInt(pos.x + (Map.width / 2));
            int yActual = Mathf.FloorToInt(pos.y + (Map.height / 2));
            if (Vector2.Distance(new Vector2(xActual, yActual), zone.centerPos) < zone.radius / 2f)
            {
                continue;
            }
            int wallCount = 0;

            // 5x3 grid check.
            for (int x = xActual - 1; x <= xActual + 1; x++)
            {
                for (int y = yActual - 1; y <= yActual + 1; y++)
                {
                    if (x == xActual && y == yActual) { continue; }
                    else if (Map.inBounds(x, y) && Map["Walls", x, y] != 0) { wallCount++; }
                }
            }
            if (wallCount == 0)
            {
                bool noObjectsInRange = true;

                foreach (Vector3 spawnedPos in SpawnedObjectPos)
                {
                    if (Vector3.Distance(spawnedPos, pos) < 2f)
                    {
                        noObjectsInRange = false;
                        break;
                    }
                }

                if (noObjectsInRange == true)
                {
                    spawnPoints.RemoveAt(index);

                    GameObject tree = (GameObject)GameObject.Instantiate(Resources.Load("BigTree"));
                    tree.transform.position = new Vector3(pos.x, pos.y, tree.transform.position.z);
                    numberofTrees++;
                    SpawnedObjectPos.Add(tree.transform.position);
                    tree.transform.parent = TerrainGameObjects.transform;
                }


            }
        }

    }

    void GenerateSpawnPoints(int id, sMap Map, DefaultRNG Rng, int MaxPointsPerZone)
    {
        bool intPos = true;
        Circle zone = Zones[id];
        List<Vector3> spawnPoints = new List<Vector3>();

        int trys = 0;

        for (int n = 0; n < MaxPointsPerZone && trys < MaxPointsPerZone;)
        {
            trys++;
            Vector3 pos = Rng.PointInCircle(zone.centerPos.x, zone.centerPos.y, zone.radius);

            int posX = Mathf.FloorToInt(pos.x);
            int posY = Mathf.FloorToInt(pos.y);
            if (Map["Walls", posX, posY] == 0 && Map["Bg", posX, posY] != mManager.GetTilesetByName("forestTileset").firstTileID + 59)
            {
                Vector3 point = new Vector3(pos.x - (Map.width / 2f), pos.y - (Map.height / 2f), pos.z);
                if (intPos)
                {
                    point = new Vector3(Mathf.Floor(point.x) + .5f, Mathf.Floor(point.y) + .5f, point.z);
                }
                if (!spawnPoints.Contains(point))
                {
                    spawnPoints.Add(point);
                    n++;
                }

            }

        }

        zoneSpawnPoints.Add(id, spawnPoints);
    }

    void SpawnBushes(int id, DefaultRNG Rng)
    {
        List<Vector3> spawnPoints = zoneSpawnPoints[id];
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            GameObject bush;
            if (Rng.NextDouble() <= 0.25f)
            {
                bush = (GameObject)GameObject.Instantiate(Resources.Load("GlowingMushroom"));
            }
            else
            {
                bush = (GameObject)GameObject.Instantiate(Resources.Load("bush"));
            }
            bush.transform.position = spawnPoints[i];
            bush.transform.parent = TerrainGameObjects.transform;
        }
        spawnPoints.Clear();
    }
}