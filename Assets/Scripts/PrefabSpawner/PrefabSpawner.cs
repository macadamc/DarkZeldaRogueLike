using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CnControls;
using ShadyPixel.CameraSystem;

[System.Serializable]
public enum SpawnType { EnterZone, Distance, OnMessageReceived };


[RequireComponent(typeof(Transform))]
public class PrefabSpawner : MonoBehaviour {

    static GameObject EnemysObj;
    public GameObject player;

    public SpawnObject[] spawnObjects;
    [HideInInspector]
    public float spawnDistance;
    [HideInInspector]
    public Zone cameraZone;
    [HideInInspector]
    public string waitForMessage;
    public List<GameObject> spawnedObjects = new List<GameObject>();

    bool spawned;

    public SpawnType spawnType;

    void Start()
    {
        cameraZone.spawners.Add(this);
        if (EnemysObj == null)
        {
            EnemysObj = GameManager.GM.InGameObjectManager.GetContainer("Enemys");
        }

    }

    void FixedUpdate()
    {
        if (spawnType == SpawnType.Distance)
            CheckDistance();
    }

    public void Spawn()
    {
        if (spawned)
            return;

        for (int i = 0; i < spawnObjects.Length; i++)
        {
            Vector2 randomVector = Random.insideUnitCircle * spawnObjects[i].positionRandomness;
            Vector3 spawnPos = (Vector3)(randomVector + spawnObjects[i].positionOffset) + transform.position;

            GameObject obj = Instantiate(spawnObjects[i].objectToSpawn, spawnPos, transform.rotation) as GameObject;

            spawnedObjects.Add(obj);
            obj.transform.parent = EnemysObj.transform;

        }

        spawned = true;
    }

    public void Despawn()
    {
        spawned = false;
        if (spawnedObjects.Count == 0)
            return;

        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            Destroy(spawnedObjects[i]);
        }
        spawnedObjects.Clear();
    }

    public void ReceiveSpawnMessage(string message)
    {
        if (spawnType != SpawnType.OnMessageReceived)
            return;

        if(message == waitForMessage)
        {
            Spawn();
        }
    }

    public void CheckDistance()
    {
        if (Vector2.Distance(transform.position, player.transform.position) < spawnDistance)
            if (!spawned)
            {
                Spawn();
            }
            else
            {
                foreach(GameObject go in spawnedObjects)
                {
                    Entity goEntity = go.GetComponent<Entity>();
                    if(go.activeInHierarchy == false)
                        if(goEntity != null && goEntity.health > 0)
                        {
                            go.SetActive(true);
                        }
                }
            }
    }
}
