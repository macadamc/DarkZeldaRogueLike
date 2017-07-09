using UnityEngine;
using System.Collections.Generic;
using CoreGame.Helper;
using QuadTree;
using ShadyPixel.CameraSystem;

[CreateAssetMenu()]
public class ForestGeneratorSO : MapGenerator
{
    InGameObjectManager ObjManager;
    GameObject TerrainGameObjects;
    GameObject Chests;
    GameObject Spawners;
    TileMapManager mManager;

    public LayoutGenerator layout;

    public Dictionary<int, List<Vector3>> zoneSpawnPoints;

    //generator settings

    public int MinEnemysPerZoneBase;
    public int MaxEnemysPerZoneBase;
    public int MaxPassivesPerZoneBase;
    public int maxCost;

    public float perlinSize;
    public float bushMod;

    public float chanceToSkip;
    public float flowerMod;
    public float rockMod;
    public float grassMod;
    public float TreeGrassMod;

    public override void Generate(sMap map, DefaultRNG rng, EntityMetaDataSO entityData, LevelGenerator lvlGenerator)
    {
        InitGenerator();
        layout.Generate();
        InitStartEndObjects(map);

        CreateWalls(map, rng);
        CreateFloor(map, rng);

        foreach (int id in layout.zIds)
        {
            string tag = layout.Zones[id].Tag;

            if (tag == "Empty")
            {
                continue;
            }

            GenerateSpawnPoints(id, map, rng);

            if (tag != "Start" && tag != "Campfire")
            {
                PlaceEnemys(id, map, rng, entityData);
                PlacePassives(id, map, rng, entityData);
            }

            if (tag == "TreasureRoom")
            {
                SpawnChest(id, rng);
            }

            if (tag == "BigTree")
            {
                SpawnBigTrees(id, 10, 100, map, rng);
            }

            if (tag == "Campfire")
            {
                SpawnBigTrees(id, 10, 100, map, rng);
                SpawnCampfire(id, rng);
                int cCount = rng.Next(1, 4);
                for (int i = 0; i < cCount; i++)
                {
                    SpawnChest(id, rng);
                }
            }
            if (tag == "Shrine")
            {
                GameObject go = (GameObject)Instantiate(Resources.Load("SpeedBuffInteractable"));
                go.transform.position = GetRandomSpawnPoint(id, rng);
                go.transform.parent = TerrainGameObjects.transform;
            }
            if (tag == "Boss")
            {

            }
            if (tag == "Shop")
            {

            }

            if(tag == "Lore")
            {

            }
            if (tag == "Trap")
            {

            }
            if (tag == "Enemy")
            {

            }

            SpawnBushes(id, rng);

        }

    }

    void InitGenerator()
    {
        layout.Init();
        zoneSpawnPoints = new Dictionary<int, List<Vector3>>();

        ObjManager = GameManager.GM.InGameObjectManager;
        mManager = GameManager.GM.mapManager;

        TerrainGameObjects = ObjManager.GetContainer("TerrainObjects");
        Chests = ObjManager.GetContainer("Chests");
        Spawners = ObjManager.GetContainer("Spawners");
    }

    //void CreateLayout()
    //{
    //    // finge shoule be a list so we can randomly pick any of the items on the fringe. should effect how the levels look. worth a try,
    //    Queue<int> fringe = new Queue<int>();

    //    int width = Map.width;
    //    int height = Map.height;

    //    float sx = Rng.Next(0, width + 1);
    //    float sy = Rng.Next(0, height + 1);

    //    // create and initalize the first Zone and add it to the fringe.
    //    int id = Connections.createNode();
    //    Circle sC = new Circle(-Vector2.one, Mathf.RoundToInt((float)(Rng.NextDouble() * (layout.maxZoneRadius - layout.minZoneRadius)) + layout.minZoneRadius));
    //    sC.ID = id;

    //    while (!Utility.Contains(qTree.QuadRect, sC))
    //    {
    //        sC.centerPos.Set(Rng.Next(0, width + 1), Rng.Next(0, height + 1));
    //    }

    //    qTree.Insert(sC);
    //    Zones.Add(id, sC);
    //    zIds.Add(id);
    //    fringe.Enqueue(id);

    //    // Create The "Tree" Layout of the level.
    //    while (fringe.Count != 0)
    //    {
    //        int sID = fringe.Dequeue();
    //        int trys = 30;

    //        while (Connections.connectionCount(sID) < layout.maxConnectionsPerZone && trys > 0)
    //        {
    //            trys--;
    //            Vector2 RVector = new Vector2(Rng.Rand(), Rng.Rand()).normalized;
    //            float radius = Mathf.RoundToInt((float)(Rng.NextDouble() * (maxZoneRadius - minZoneRadius)) + minZoneRadius);

    //            RVector = Zones[sID].centerPos + (RVector * (Zones[sID].radius + radius));

    //            Circle c = new Circle(new Vector2(RVector.x, RVector.y), radius);
    //            c.centerPos = Utility.DDALine(Zones[sID], c);// sets the position to the closest intger position that doesnt collide with the other circle..

    //            List<Circle> res = new List<Circle>();
    //            qTree.GetObjects(new Rect((c.centerPos.x - radius) - MAXDISTFORCONNECTION, (c.centerPos.y - radius) - MAXDISTFORCONNECTION, (radius * 2) + MAXDISTFORCONNECTION, radius * 2 + MAXDISTFORCONNECTION), ref res);

    //            bool collision = false;
    //            foreach (Circle circle in res)
    //            {
    //                if (Circle.Intersect(c, circle))
    //                {
    //                    collision = true;
    //                    break;
    //                }
    //            }
    //            // if the new circle is in the rect of the quad tree and threres no collisions with other circles,
    //            // we add the new circle to the quadtree, create a new node in the graph, and store the circle in the zones dictionary with the nodes id.
    //            // then finaly we add a connection between the 2 circles.(a connection stores each nodes id in the conections dictionary so we can look up what each nodes individual connections are.)
    //            if (Utility.Contains(qTree.QuadRect, c) && !collision)
    //            {
    //                qTree.Insert(c);

    //                int nID = Connections.createNode();
    //                c.ID = nID;
    //                Zones.Add(nID, c);
    //                zIds.Add(nID);

    //                Connections.addConnection(sID, nID);

    //                fringe.Enqueue(nID);
    //            }


    //        }
    //    }

    //    // get a list of all leafnodes.
    //    foreach (int nodeID in Connections.keys)
    //    {
    //        if(Connections.GetAdjecent(nodeID).Count == 1)
    //        {
    //            leafNodes.Add(nodeID);
    //        }
    //    }

    //    //Create The STart and end points on the Map
    //    int StartID = leafNodes[Rng.Next(0, leafNodes.Count - 1)];

    //    foreach(int cID in leafNodes)
    //    {
    //        if (StartID == cID)
    //            continue;

    //        List<int> path = new List<int>();
    //        Utility.GetPath(StartID, cID, Connections, Zones, ref path); // pathfinding bugged maybe? pathfinding uses manhatten distance for huesetic score...

    //        float pDist = 0;
    //        for (int i = 0; i < path.Count - 1; i += 2)
    //        {
    //            Vector2 p1 = Zones[path[i]].centerPos;
    //            Vector2 p2 = Zones[path[i + 1]].centerPos;
    //            pDist += Vector2.Distance(p1, p2); //get the distance between the 2 points and add it to the path distance. (pDist)
    //        }

    //        if (pDist >= lPathDist) // should be the distance between the nodes not how many nodes there are in the path 
    //        {
    //            longestPath = path;
    //            lPathDist = pDist;
    //        }
    //    }

    //    List<int> pathIDs = new List<int>();

    //    //create Loops
    //    foreach (int nodeID in Connections.keys)
    //    {
    //        //continue;// <---------- DEBUG!!!

    //        if (Connections.GetAdjecent(nodeID).Count == maxConnectionsPerZone)
    //        {
    //            continue;
    //        }

    //        Vector2 pos = Zones[nodeID].centerPos;
    //        float r = Zones[nodeID].radius;

    //        List<Circle> objs = new List<Circle>();
    //        qTree.GetObjects(new Rect(pos.x - MAXDISTFORCONNECTION, pos.y - MAXDISTFORCONNECTION, (r * 2) + MAXDISTFORCONNECTION, (r * 2) + MAXDISTFORCONNECTION), ref objs);
    //        foreach (Circle c in objs)
    //        {
    //            if (longestPath.Contains(nodeID) || longestPath.Contains(c.ID)|| nodeID == c.ID || Connections.GetAdjecent(nodeID).Contains(c.ID) || Connections.GetAdjecent(c.ID).Count >= maxConnectionsPerZone)
    //            {
    //                continue;
    //            }

    //            float dist = Mathf.Sqrt(Mathf.Pow((Zones[nodeID].centerPos.x - c.centerPos.x), 2) + Mathf.Pow((Zones[nodeID].centerPos.y - c.centerPos.y), 2)) - (Zones[nodeID].radius + c.radius);

    //            if (dist <= MAXDISTFORCONNECTION)
    //            {
    //                pathIDs.Clear();
    //                Utility.GetPath(nodeID, c.ID, Connections, Zones, ref pathIDs);
    //                if (pathIDs.Count >= MINLENGTHOFLOOP)
    //                {
    //                    Connections.addConnection(nodeID, c.ID);
    //                }
    //            }

    //        }
    //    }
    //}

    void InitStartEndObjects(sMap Map)
    {
        List<int> longestPath = layout.longestPath;
        Dictionary<int, Circle> Zones = layout.Zones;
        //get start and end locations.
        Circle sZone = layout.Zones[longestPath[0]];
        Circle eZone = Zones[longestPath[longestPath.Count - 1]];

        // sets camera to Starting pos;
        //Camera.main.transform.parent.transform.position = new Vector3(sZone.centerPos.x, sZone.centerPos.y, Camera.main.transform.position.z);

        // set the player to its starting position.
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = new Vector3(sZone.centerPos.x - (Map.width / 2), sZone.centerPos.y - (Map.height / 2), player.transform.position.z);

        //place Map End "Stairs"
        GameObject End = GameObject.Instantiate(Resources.Load("LevelEnd", typeof(GameObject))) as GameObject;
        End.transform.position = new Vector3(eZone.centerPos.x - (Map.width / 2), eZone.centerPos.y - (Map.height / 2), End.transform.position.z);
        End.transform.parent = GameManager.GM.InGameObjects.transform.FindChild("TerrainObjects");
    }

    void CreateFloor(sMap Map, DefaultRNG Rng)
    {
        //ground tileIDs; TILEID START AT 1 FOR EACH TILESET!
        int[] grass = new int[3] { 49, 50, 51};
        int[] flowers = new int[2] { 52, 53 };
        int[] mushrooms = new int[2] { 54, 55 };

        Tileset ground = mManager.GetTilesetByName("forestTileset");

        for (int y = 0; y < Map.height; y++)
        {
            for (int x = 0; x < Map.width; x++)
            {
                Map["Floor", x, y] = ground.firstTileID + 48;

                if (Rng.NextDouble() > 1f - chanceToSkip)
                    continue;

                if (Map["Walls", x, y] == 0)
                {
                    if (Rng.NextDouble() <= flowerMod)
                    {
                        Map["Bg", x, y] = ground.firstTileID + flowers[Rng.Next(0, flowers.Length)];
                    }
                    else if (Rng.NextDouble() <= rockMod)
                    {
                        Map["Bg", x, y] = ground.firstTileID + mushrooms[Rng.Next(0, mushrooms.Length)];
                    }
                    else if (Rng.NextDouble() <= grassMod)
                    {
                        Map["Bg", x, y] = ground.firstTileID + grass[Rng.Next(0, grass.Length)];
                    }
                }
                else
                {
                    if (Rng.NextDouble() <= TreeGrassMod)
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
        Tunneler tunneler = new Tunneler(Map, "Walls", layout.Connections, layout.Zones);
        tunneler.Tunnel();

        tunneler = new Tunneler(Map, "Bg", layout.Connections, layout.Zones);
        tunneler.DrawPath(layout.longestPath);

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

    void PlaceEnemys(int id, sMap Map, DefaultRNG Rng, EntityMetaDataSO EntityData)
    {
        int curCost = 0;

        List<EnemyMetaData> currentLvlEnemys = EntityData.GetEnemyByFirstLvlRange(1, 1);

        Circle zone = layout.Zones[id];

        float percent = (zone.radius - layout.minZoneRadius) / (layout.maxZoneRadius - layout.minZoneRadius);
        int maxEnemys = Mathf.RoundToInt(MaxEnemysPerZoneBase * percent);
        int maxEnemysPerZone = Rng.Next(MinEnemysPerZoneBase, maxEnemys + 1);

        GameObject spawner = (GameObject)GameObject.Instantiate(Resources.Load("PrefabSpawner"));
        spawner.transform.position = new Vector3(zone.centerPos.x - (Map.width / 2f), zone.centerPos.y - (Map.height / 2f), spawner.transform.position.z);
        spawner.transform.parent = Spawners.transform;
        PrefabSpawner ps = spawner.GetComponent<PrefabSpawner>();
        ps.spawnObjects = new SpawnObject[maxEnemysPerZone];

        if (currentLvlEnemys.Count == 0)
            return;

        for (int i = 0; i < maxEnemysPerZone && curCost < maxCost; i++)
        {
            SpawnObject s = new SpawnObject();
            EnemyMetaData enemyData = currentLvlEnemys[Rng.Next(0, currentLvlEnemys.Count)];
            
            s.objectToSpawn = enemyData.prefab;
            s.positionOffset = GetRandomSpawnPoint(id, Rng) - spawner.transform.position;
            ps.player = Camera.main.gameObject;
            ps.cameraZone = GameObject.Find("Bounds").GetComponent<Zone>();
            

            ps.spawnObjects[i] = s;
            ps.spawnType = SpawnType.Distance;
            ps.spawnDistance = (Camera.main.orthographicSize * 2) + zone.radius;

            curCost += enemyData.cost;
        }
    }

    void PlacePassives(int id, sMap Map, DefaultRNG Rng, EntityMetaDataSO EntityData)
    {
        List<EntityMetaData> currentLvlEnemys = EntityData.GetPassiveByFirstLvlRange(1, 1);

        Circle zone = layout.Zones[id];

        float percent = (zone.radius - layout.minZoneRadius) / (layout.maxZoneRadius - layout.minZoneRadius);
        int maxEnemys = Mathf.RoundToInt(MaxPassivesPerZoneBase * percent);
        int maxEnemysPerZone = Rng.Next(0, maxEnemys + 1);

        if (maxEnemysPerZone == 0) { return; }

        GameObject spawner = (GameObject)GameObject.Instantiate(Resources.Load("PrefabSpawner"));
        spawner.transform.position = new Vector3(zone.centerPos.x - (Map.width / 2f), zone.centerPos.y - (Map.height / 2f), spawner.transform.position.z);
        spawner.transform.parent = Spawners.transform;
        PrefabSpawner ps = spawner.GetComponent<PrefabSpawner>();
        ps.spawnObjects = new SpawnObject[maxEnemysPerZone];


        if (currentLvlEnemys.Count == 0)
            return;

        for (int i = 0; i < maxEnemysPerZone; i++)
        {
            SpawnObject s = new SpawnObject();
            
            s.objectToSpawn = currentLvlEnemys[Rng.Next(0, currentLvlEnemys.Count)].prefab;
            s.positionOffset = GetRandomSpawnPoint(id, Rng) - spawner.transform.position;
            ps.player = Camera.main.gameObject;
            ps.cameraZone = GameObject.Find("Bounds").GetComponent<Zone>();

            ps.spawnObjects[i] = s;
            ps.spawnType = SpawnType.Distance;
            ps.spawnDistance = (Camera.main.orthographicSize * 2) + zone.radius;
        }
    }

    void GenerateSpawnPoints(int id, sMap Map, DefaultRNG Rng)
    {
        bool intPos = true;
        Circle zone = layout.Zones[id];
        List<Vector3> spawnPoints = new List<Vector3>();

        float zoneSize = (layout.maxZoneRadius - (layout.maxZoneRadius - zone.radius)) / layout.maxZoneRadius;
        int MaxPointsPerZone = 50;
        int trys = 0;

        for (int n = 0; n < Mathf.Round(zoneSize * MaxPointsPerZone) && trys < MaxPointsPerZone;)
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

    void SpawnChest(int zoneID, DefaultRNG Rng)
    {
        Circle zone = layout.Zones[zoneID];
        GameObject chest = (GameObject)GameObject.Instantiate(Resources.Load("Chest"));

        chest.transform.position = GetRandomSpawnPoint(zoneID, Rng);
        chest.transform.GetChild(0).GetComponent<Chest>().seed = Rng.Next(0, 100000);
        chest.transform.parent = Chests.transform;
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
    }// Draws the tiles in the tilemap.
    void SpawnBigTrees(int id, int maxPerZone, int maxTrys, sMap Map, DefaultRNG Rng)
    {
        Circle zone = layout.Zones[id];
        List<Vector3> SpawnedObjectPos = new List<Vector3>();

        if ((layout.maxZoneRadius - (layout.maxZoneRadius - zone.radius)) / layout.maxZoneRadius > .0f)
        {
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

    }// Creates a gameobject at a spawnPoint. (has better collider... also fixes the problem with sword slahing through it that the tilemap version has..)

    void SpawnBushes(int id, DefaultRNG Rng)
    {
        if (layout.Zones[id].Tag == "Empty") { return; }

        List<Vector3> spawnPoints = zoneSpawnPoints[id];
        for (int i = 0; i < spawnPoints.Count; i++)
        {

            if (Rng.NextDouble() > 1f - chanceToSkip)
                continue;

            GameObject bush;
            if (Rng.NextDouble() <= 0.15)
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

    //void DrawDebug()
    //{
    //    GameObject go = new GameObject("Zone Connections");
    //    go.SetActive(false);
    //    GameObject zBounds = new GameObject("Zone Bounds.");
    //    foreach (Circle c in layout.Zones.Values)
    //    {
    //        GameObject z = new GameObject("Zone" + c.ID);
    //        CircleCollider2D coll = z.AddComponent<CircleCollider2D>();
    //        coll.radius = c.radius - 1;
    //        coll.isTrigger = true;
    //        z.transform.parent = zBounds.transform;
    //        z.transform.position = c.centerPos;

    //    }

    //    List<int> visited = new List<int>();
    //    List<QtreeObj> roomsVisited = new List<QtreeObj>();
    //    Queue<int> f = new Queue<int>();
    //    f.Enqueue(1);
    //    while (f.Count != 0)
    //    {
    //        int id = f.Dequeue();

    //        foreach (QtreeObj obj in layout.Zones[id].rooms)
    //        {
    //            GameObject lObj = new GameObject("Zone2Room");
    //            LineRenderer lRend = lObj.AddComponent<LineRenderer>();
    //            lObj.transform.parent = go.transform;

    //            lRend.SetPositions(new Vector3[2] { layout.Zones[id].centerPos, obj.Rect.center });
    //            lRend.material = Resources.Load<Material>("DefaultMat");
    //            lRend.SetColors(Color.green, Color.green);
    //            lRend.SetWidth(1f, 1f);
    //            lRend.sortingOrder = 1;

    //            if (roomsVisited.Contains(obj))
    //            {
    //                continue;
    //            }

    //            int MaxConnectRadius = 2;

    //            List<QtreeObj> res = new List<QtreeObj>();
    //            Rect r = new Rect(obj.Rect);
    //            r.x -= MaxConnectRadius;
    //            r.y -= MaxConnectRadius;
    //            r.width += MaxConnectRadius;
    //            r.height += MaxConnectRadius;
    //            layout.RoomCollisions.GetObjects(r, ref res);

    //            foreach (QtreeObj room in res)
    //            {
    //                if (room.ZoneID == obj.ZoneID || layout.Connections.hasConnection(obj.ZoneID, room.ZoneID))
    //                    continue;

    //                lObj = new GameObject("Room2Room");
    //                lRend = lObj.AddComponent<LineRenderer>();
    //                lObj.transform.parent = go.transform;

    //                lRend.SetPositions(new Vector3[2] { obj.Rect.center, room.Rect.center });
    //                lRend.material = Resources.Load<Material>("DefaultMat");
    //                lRend.SetColors(Color.magenta, Color.magenta);
    //                lRend.SetWidth(1f, 1f);
    //                lRend.sortingOrder = 1;
    //            }
    //        }

    //        foreach (int cid in layout.Connections.GetAdjecent(id))
    //        {
    //            if (visited.Contains(cid))
    //            {
    //                continue;
    //            }
    //            GameObject lObj = new GameObject("LineObj");
    //            LineRenderer lRend = lObj.AddComponent<LineRenderer>();
    //            lObj.transform.parent = go.transform;

    //            lRend.SetPositions(new Vector3[2] { layout.Zones[id].centerPos, layout.Zones[cid].centerPos });
    //            lRend.material = Resources.Load<Material>("DefaultMat");

    //            //Color start = new Color(DifficultyLvl[id] / HighestDifficultyLVL, 1f - (DifficultyLvl[id] / HighestDifficultyLVL), 0);
    //            //Color end = new Color(DifficultyLvl[cid] / HighestDifficultyLVL, 1f - (DifficultyLvl[cid] / HighestDifficultyLVL), 0);
    //            //lRend.SetColors(start, end);

    //            if (layout.longestPath.Contains(cid) && layout.longestPath.Contains(id))
    //            {
    //                lRend.SetColors(Color.yellow, Color.yellow);
    //            }
    //            else
    //            {
    //                lRend.SetColors(Color.blue, Color.blue);
    //            }

    //            lRend.SetWidth(1f, 1f);
    //            lRend.sortingOrder = 1;
    //            visited.Add(id);


    //            if (!f.Contains(cid) && !visited.Contains(cid))
    //            {
    //                f.Enqueue(cid);
    //            }
    //        }
    //    }
    //}

    void SpawnCampfire(int id, DefaultRNG Rng)
    {
        GameObject go = (GameObject)GameObject.Instantiate(Resources.Load("CampFire"));
        go.transform.position = GetRandomSpawnPoint(id, Rng);
        go.transform.parent = TerrainGameObjects.transform;
    }

    Vector3 GetRandomSpawnPoint(int id, DefaultRNG Rng)
    {
        List<Vector3> points = zoneSpawnPoints[id];
        int index = Rng.Next(0, points.Count);
        Vector3 pos = points[index];
        points.RemoveAt(index);

        return pos;
    }
}

public static class GeneratorTools
{
    static void PlaceEnemys(int minBase, int maxBase, int maxCost, int id, sMap Map, DefaultRNG Rng, EntityMetaDataSO EntityData, LayoutGenerator layout, Dictionary<int, List<Vector3>> zoneSpawnPoints, GameObject Spawners)
    {
        int curCost = 0;

        List<EnemyMetaData> currentLvlEnemys = EntityData.GetEnemyByFirstLvlRange(1, 1);

        Circle zone = layout.Zones[id];

        float percent = (zone.radius - layout.minZoneRadius) / (layout.maxZoneRadius - layout.minZoneRadius);
        int maxEnemys = Mathf.RoundToInt(minBase * percent);
        int maxEnemysPerZone = Rng.Next(maxBase, maxEnemys + 1);

        GameObject spawner = (GameObject)GameObject.Instantiate(Resources.Load("PrefabSpawner"));
        spawner.transform.position = new Vector3(zone.centerPos.x - (Map.width / 2f), zone.centerPos.y - (Map.height / 2f), spawner.transform.position.z);
        spawner.transform.parent = Spawners.transform;
        PrefabSpawner ps = spawner.GetComponent<PrefabSpawner>();
        ps.spawnObjects = new SpawnObject[maxEnemysPerZone];

        if (currentLvlEnemys.Count == 0)
            return;

        for (int i = 0; i < maxEnemysPerZone && curCost < maxCost; i++)
        {
            SpawnObject s = new SpawnObject();
            EnemyMetaData enemyData = currentLvlEnemys[Rng.Next(0, currentLvlEnemys.Count)];

            s.objectToSpawn = enemyData.prefab;
            s.positionOffset = GetRandomSpawnPoint(id, Rng, zoneSpawnPoints) - spawner.transform.position;
            ps.player = Camera.main.gameObject;
            ps.cameraZone = GameObject.Find("Bounds").GetComponent<Zone>();


            ps.spawnObjects[i] = s;
            ps.spawnType = SpawnType.Distance;
            ps.spawnDistance = (Camera.main.orthographicSize * 2) + zone.radius;

            curCost += enemyData.cost;
        }
    }

    static void PlacePassives(int maxBase, int id, sMap Map, DefaultRNG Rng, EntityMetaDataSO EntityData, LayoutGenerator layout, Dictionary<int, List<Vector3>> zoneSpawnPoints, GameObject Spawners)
    {
        List<EntityMetaData> currentLvlEnemys = EntityData.GetPassiveByFirstLvlRange(1, 1);

        Circle zone = layout.Zones[id];

        float percent = (zone.radius - layout.minZoneRadius) / (layout.maxZoneRadius - layout.minZoneRadius);
        int maxEnemys = Mathf.RoundToInt(maxBase * percent);
        int maxEnemysPerZone = Rng.Next(0, maxEnemys + 1);

        if (maxEnemysPerZone == 0) { return; }

        GameObject spawner = (GameObject)GameObject.Instantiate(Resources.Load("PrefabSpawner"));
        spawner.transform.position = new Vector3(zone.centerPos.x - (Map.width / 2f), zone.centerPos.y - (Map.height / 2f), spawner.transform.position.z);
        spawner.transform.parent = Spawners.transform;
        PrefabSpawner ps = spawner.GetComponent<PrefabSpawner>();
        ps.spawnObjects = new SpawnObject[maxEnemysPerZone];


        if (currentLvlEnemys.Count == 0)
            return;

        for (int i = 0; i < maxEnemysPerZone; i++)
        {
            SpawnObject s = new SpawnObject();

            s.objectToSpawn = currentLvlEnemys[Rng.Next(0, currentLvlEnemys.Count)].prefab;
            s.positionOffset = GetRandomSpawnPoint(id, Rng, zoneSpawnPoints) - spawner.transform.position;
            ps.player = Camera.main.gameObject;
            ps.cameraZone = GameObject.Find("Bounds").GetComponent<Zone>();

            ps.spawnObjects[i] = s;
            ps.spawnType = SpawnType.Distance;
            ps.spawnDistance = (Camera.main.orthographicSize * 2) + zone.radius;
        }
    }

    static void GenerateSpawnPoints(int id, sMap Map, DefaultRNG Rng, LayoutGenerator layout, Dictionary<int, List<Vector3>> zoneSpawnPoints)
    {
        bool intPos = true;
        Circle zone = layout.Zones[id];
        List<Vector3> spawnPoints = new List<Vector3>();

        float zoneSize = (layout.maxZoneRadius - (layout.maxZoneRadius - zone.radius)) / layout.maxZoneRadius;
        int MaxPointsPerZone = 50;
        int trys = 0;

        for (int n = 0; n < Mathf.Round(zoneSize * MaxPointsPerZone) && trys < MaxPointsPerZone;)
        {
            trys++;
            Vector3 pos = Rng.PointInCircle(zone.centerPos.x, zone.centerPos.y, zone.radius);

            int posX = Mathf.FloorToInt(pos.x);
            int posY = Mathf.FloorToInt(pos.y);
            if (Map["Walls", posX, posY] == 0 && Map["Bg", posX, posY] != GameManager.GM.mapManager.GetTilesetByName("forestTileset").firstTileID + 59)
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

    static void SpawnChest(int zoneID, DefaultRNG Rng, LayoutGenerator layout, Dictionary<int, List<Vector3>> zoneSpawnPoints, GameObject Chests)
    {
        Circle zone = layout.Zones[zoneID];
        GameObject chest = (GameObject)GameObject.Instantiate(Resources.Load("Chest"));

        chest.transform.position = GetRandomSpawnPoint(zoneID, Rng, zoneSpawnPoints);
        chest.transform.GetChild(0).GetComponent<Chest>().seed = Rng.Next(0, 100000);
        chest.transform.parent = Chests.transform;
    }

    static void DrawBigTree(int x, int y, sMap Map)
    {
        Tileset t = GameManager.GM.mapManager.GetTilesetByName("forestTileset");

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
    }// Draws the tiles in the tilemap.

    static void SpawnBigTrees(int id, int maxPerZone, int maxTrys, sMap Map, DefaultRNG Rng, LayoutGenerator layout, Dictionary<int, List<Vector3>> zoneSpawnPoints, GameObject TerrainGameObjects)
    {
        Circle zone = layout.Zones[id];
        List<Vector3> SpawnedObjectPos = new List<Vector3>();

        if ((layout.maxZoneRadius - (layout.maxZoneRadius - zone.radius)) / layout.maxZoneRadius > .0f)
        {
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

    }

    static void SpawnBushes(float chanceToSkip, int id, DefaultRNG Rng, LayoutGenerator layout, Dictionary<int, List<Vector3>> zoneSpawnPoints, GameObject TerrainGameObjects)
    {
        if (layout.Zones[id].Tag == "Empty") { return; }

        List<Vector3> spawnPoints = zoneSpawnPoints[id];
        for (int i = 0; i < spawnPoints.Count; i++)
        {

            if (Rng.NextDouble() > 1f - chanceToSkip)
                continue;

            GameObject bush;
            if (Rng.NextDouble() <= 0.15)
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

    static void SpawnCampfire(int id, DefaultRNG Rng, Dictionary<int, List<Vector3>> zoneSpawnPoints, GameObject TerrainGameObjects)
    {
        GameObject go = (GameObject)GameObject.Instantiate(Resources.Load("CampFire"));
        go.transform.position = GetRandomSpawnPoint(id, Rng, zoneSpawnPoints);
        go.transform.parent = TerrainGameObjects.transform;
    }

    static Vector3 GetRandomSpawnPoint(int id, DefaultRNG Rng, Dictionary<int, List<Vector3>> zoneSpawnPoints)
    {
        List<Vector3> points = zoneSpawnPoints[id];
        int index = Rng.Next(0, points.Count);
        Vector3 pos = points[index];
        points.RemoveAt(index);

        return pos;
    }
}