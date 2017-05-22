using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadyPixel.CameraSystem
{
    public static class CameraManager
    {
        public static Bounds currentBounds;
        public static Zone currentZone;
        public static Zone lastZone;


        public static void SetCameraBounds(Zone cameraZone)
        {
            lastZone = currentZone;
            currentZone = cameraZone;
            currentBounds = currentZone.bounds;

            if (lastZone != null)
                SpawnManager.DespawnOldSpawners(lastZone);

            SpawnManager.EnterZone(currentZone);

            //Debug.Log(currentZone.name);
        }

    }
}