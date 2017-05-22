using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShadyPixel.CameraSystem;

public static class SpawnManager {

    public static void CheckForDistanceSpawnersInZone(Zone cameraZone)
    {
        for (int i = 0; i < cameraZone.spawners.Count; i++)
        {
            if (cameraZone.spawners[i].spawnType == SpawnType.Distance)
                cameraZone.spawners[i].CheckDistance();
        }
    }

    public static void EnterZone(Zone cameraZone)
    {
        for (int i = 0; i < cameraZone.spawners.Count; i++)
        {
            if(cameraZone.spawners[i].spawnType == SpawnType.EnterZone)
                cameraZone.spawners[i].Spawn();
        }
    }

    public static void DespawnOldSpawners(Zone cameraZone)
    {
        for (int i = 0; i < cameraZone.spawners.Count; i++)
        {
            cameraZone.spawners[i].Despawn();
        }
    }

    public static void SendMessageToSpawnersInZone(Zone cameraZone, string message)
    {
        for (int i = 0; i < cameraZone.spawners.Count; i++)
        {
            if (cameraZone.spawners[i].spawnType == SpawnType.OnMessageReceived)
                cameraZone.spawners[i].ReceiveSpawnMessage(message);
        }
    }
    public static void SendMessageToSpawnersInZone(string message)
    {
        PrefabSpawner[] spawners = GameObject.FindObjectsOfType<PrefabSpawner>();

        for (int i = 0; i < spawners.Length; i++)
        {
            if (spawners[i].spawnType == SpawnType.OnMessageReceived)
                spawners[i].ReceiveSpawnMessage(message);
        }
    }
}
