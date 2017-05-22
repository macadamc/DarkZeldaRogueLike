using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using QuadTree;
using CoreGame.Helper;
using System;

[System.Serializable]
public class LayoutGenerator {

    sMap Map;
    DefaultRNG Rng;
    [System.NonSerialized]
    public Graph Connections;
    [System.NonSerialized]
    public Dictionary<int, Circle> Zones;
    [System.NonSerialized]
    public List<int> zIds;

    QuadTree<Circle> qTree;
    [NonSerialized]
    public QuadTree<QtreeObj> RoomCollisions;
    [NonSerialized]
    public List<int> leafNodes;
    [NonSerialized]
    public List<int> longestPath;
    [NonSerialized]
    public float lPathDist;

    public int maxConnectionsPerZone;
    public float minZoneRadius;
    public float maxZoneRadius;
    public float MAXDISTFORCONNECTION;
    public int MINLENGTHOFLOOP;

    public List<ConstraintList> constraints;

    public void Init()
    {
        Map = GameManager.GM.mapManager.map;
        Rng = GameManager.GM.Rng;

        Connections = new Graph();
        Zones = new Dictionary<int, Circle>();
        zIds = new List<int>();

        //create the quadtrees
        qTree = new QuadTree<Circle>(new Rect(Vector2.zero, new Vector2(Map.width, Map.height)));
        //RoomCollisions = new QuadTree<QtreeObj>(new Rect(0, 0, Map.width, Map.height));

        longestPath = new List<int>();
        leafNodes = new List<int>();

        lPathDist = 0;
    }

    public void Generate()
    {
        // finge shoule be a list so we can randomly pick any of the items on the fringe. should effect how the levels look. worth a try,
        Queue<int> fringe = new Queue<int>();

        int width = Map.width;
        int height = Map.height;

        // create and initalize the first Zone and add it to the fringe.
        int id = Connections.createNode();

        //float sx = Rng.Next(0, width + 1);
        //float sy = Rng.Next(0, height + 1);
        float sr = Mathf.RoundToInt((float)(Rng.NextDouble() * (maxZoneRadius - minZoneRadius)) + minZoneRadius);

        Circle sC = new Circle(-Vector2.one, sr);
        sC.ID = id;

        while (!Utility.Contains(qTree.QuadRect, sC))
        {
            sC.centerPos = new Vector2(Rng.Next(0, width + 1), Rng.Next(0, height + 1));
        }

        qTree.Insert(sC);
        Zones.Add(id, sC);
        zIds.Add(id);
        fringe.Enqueue(id);

        setZoneTag(sC);

        // Create The "Tree" Layout of the level.
        while (fringe.Count != 0)
        {
            int sID = fringe.Dequeue();
            int trys = 50;

            while (Connections.connectionCount(sID) < maxConnectionsPerZone && trys > 0)
            {
                trys--;
                Vector2 RVector = new Vector2(Rng.Rand(), Rng.Rand()).normalized;
                float radius = Mathf.RoundToInt((float)(Rng.NextDouble() * (maxZoneRadius - minZoneRadius)) + minZoneRadius);

                RVector = Zones[sID].centerPos + (RVector * (Zones[sID].radius + radius));

                Circle c = new Circle(new Vector2(RVector.x, RVector.y), radius);
                c.centerPos = Utility.DDALine(Zones[sID], c);// sets the position to the closest intger position that doesnt collide with the other circle..

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

                    Connections.addConnection(sID, nID);

                    fringe.Enqueue(nID);

                    setZoneTag(c);
                }


            }
        }

        // get a list of all leafnodes.
        foreach (int nodeID in Connections.keys)
        {
            if (Connections.GetAdjecent(nodeID).Count == 1)
            {
                leafNodes.Add(nodeID);
            }
        }

        //Create The STart and end points on the Map
        int StartID = leafNodes[Rng.Next(0, leafNodes.Count - 1)];

        foreach (int cID in leafNodes)
        {
            if (StartID == cID)
                continue;

            List<int> path = new List<int>();
            Utility.GetPath(StartID, cID, Connections, Zones, ref path); // pathfinding bugged maybe? pathfinding uses manhatten distance for huesetic score...

            float pDist = 0;
            for (int i = 0; i < path.Count - 1; i += 2)
            {
                Vector2 p1 = Zones[path[i]].centerPos;
                Vector2 p2 = Zones[path[i + 1]].centerPos;
                pDist += Vector2.Distance(p1, p2); //get the distance between the 2 points and add it to the path distance. (pDist)
            }

            if (pDist >= lPathDist) // should be the distance between the nodes not how many nodes there are in the path 
            {
                longestPath = path;
                lPathDist = pDist;
            }
        }
        // set the start and end tags for the generator.;
        Zones[longestPath[0]].Tag = "Start";
        Zones[longestPath[longestPath.Count - 1]].Tag = "End";

        List<int> pathIDs = new List<int>();

        //create Loops
        foreach (int nodeID in Connections.keys)
        {
            //continue;// <---------- DEBUG!!!

            if (Connections.GetAdjecent(nodeID).Count == maxConnectionsPerZone)
            {
                continue;
            }

            Vector2 pos = Zones[nodeID].centerPos;
            float r = Zones[nodeID].radius;

            List<Circle> objs = new List<Circle>();
            qTree.GetObjects(new Rect(pos.x - MAXDISTFORCONNECTION, pos.y - MAXDISTFORCONNECTION, (r * 2) + MAXDISTFORCONNECTION, (r * 2) + MAXDISTFORCONNECTION), ref objs);
            foreach (Circle c in objs)
            {
                if (longestPath.Contains(nodeID) || longestPath.Contains(c.ID) || nodeID == c.ID || Connections.GetAdjecent(nodeID).Contains(c.ID) || Connections.GetAdjecent(c.ID).Count >= maxConnectionsPerZone)
                {
                    continue;
                }

                float dist = Mathf.Sqrt(Mathf.Pow((Zones[nodeID].centerPos.x - c.centerPos.x), 2) + Mathf.Pow((Zones[nodeID].centerPos.y - c.centerPos.y), 2)) - (Zones[nodeID].radius + c.radius);

                if (dist <= MAXDISTFORCONNECTION)
                {
                    pathIDs.Clear();
                    Utility.GetPath(nodeID, c.ID, Connections, Zones, ref pathIDs);
                    if (pathIDs.Count >= MINLENGTHOFLOOP)
                    {
                        Connections.addConnection(nodeID, c.ID);
                    }
                }

            }
        }

        PostProcessMinSpawnTags();
    }

    void setZoneTag(Circle zone)
    {
        List<string> tags = new List<string>();
        List<int> weights = new List<int>();
        int totalWeight = 0;

        foreach (ConstraintList constraint in constraints)
        {
            if (constraint.IsValid(zone, this))
            {
                foreach (Tag tag in constraint.tagList)
                {
                    tags.Add(tag.value);
                    weights.Add(tag.weight);
                    totalWeight += tag.weight;
                }
            }
        }
        zone.Tag = Rng.WeightedChoice<string>(tags, weights, totalWeight);
        if (MinSpawnConstraint.SpawnCounts.ContainsKey(zone.Tag))
        {
            MinSpawnConstraint.SpawnCounts[zone.Tag] += 1;
        }

    }

    void PostProcessMinSpawnTags ()
    {
        foreach(MinSpawnConstraint constraint in MinSpawnConstraint._SpawnConstraints)
        {
            for(int tagIndex = 0; tagIndex < constraint.parentList.tagList.Count; tagIndex++)
            {
                string tag = constraint.parentList.tagList[tagIndex].value;

                if (MinSpawnConstraint.SpawnCounts[tag] < constraint.MinSpawnCounts[tagIndex])
                {
                    // these are the tags that dont meet the min amount.
                    foreach(int id in zIds)
                    {
                        if (MinSpawnConstraint.SpawnCounts[tag] >= constraint.MinSpawnCounts[tagIndex])
                        {
                            break;
                        }

                        if (constraint.IsValid(Zones[id], constraint.parentList, this) && Zones[id].Tag != tag)
                        {
                            Zones[id].Tag = tag;
                            MinSpawnConstraint.SpawnCounts[tag] += 1;
                        }
                    }
                }

            }
        }
    }
}

[System.Serializable]
public class Tag
{
    public string value;
    public int weight;
}

public interface ITagConstraint
{
    bool IsValid(Circle zone, ConstraintList parentList, LayoutGenerator layout);
}

[Serializable]
public class SizeConstraint : ITagConstraint
{
    [Range(0, 1f)]
    public float minSizeThreshold;
    [Range(0, 1f)]
    public float maxSizeThreshhold;

    public bool IsValid(Circle zone, ConstraintList parentList, LayoutGenerator layout)
    {
        float size = zone.radius / layout.maxZoneRadius;// needs to be maxZoneRadius
        return size >= minSizeThreshold && size <= maxSizeThreshhold;
    }
}

[Serializable]
public class MinSpawnConstraint : ITagConstraint
{
    // returns true when there are tagged rooms that are lower than there minimum spawnCount;

    public static Dictionary<string, int> SpawnCounts;
    public static List<MinSpawnConstraint> _SpawnConstraints;
    [NonSerialized]
    public ConstraintList parentList;

    [Range(0, 10)]
    public List<int> MinSpawnCounts;

    [NonSerialized]
    public bool init = false;

    public bool IsValid(Circle zone, ConstraintList parentList, LayoutGenerator layout)
    {
        if (init == false)
        {
            init = true;
            
            if (SpawnCounts == null)
            {
                SpawnCounts = new Dictionary<string, int>();
                _SpawnConstraints = new List<MinSpawnConstraint>();
            }

            _SpawnConstraints.Add(this);
            this.parentList = parentList;
        }

        for (int i = 0; i < parentList.tagList.Count;i++)
        {
            Tag t = parentList.tagList[i];
            if (SpawnCounts.ContainsKey(t.value) == false)
            {
                SpawnCounts.Add(t.value, 0);
            }

            if (SpawnCounts[t.value] < MinSpawnCounts[i])
            {
                return true;
            }
        }
        return false;
    }
}

[Serializable]
public class ConstraintList
{
    public bool useSize;
    public SizeConstraint sizeConstraint;
    public bool useMinSpawn;
    public MinSpawnConstraint minSpawnConstraint;
    public List<Tag> tagList;

    private List<bool> checks;

    public bool IsValid(Circle zone, LayoutGenerator layout)
    {
        if(checks == null) { checks = new List<bool>();} 
        else { checks.Clear(); }
        if (useSize == true) { checks.Add(sizeConstraint.IsValid(zone, this, layout)); }
        if (useMinSpawn == true) { checks.Add(minSpawnConstraint.IsValid(zone, this, layout)); }
        foreach(bool check in checks)
        {
            if (check == false) { return false; }
        }
        return true;
    }
}

