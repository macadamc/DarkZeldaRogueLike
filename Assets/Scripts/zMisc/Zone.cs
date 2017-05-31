using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadyPixel.CameraSystem
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Zone : MonoBehaviour
    {
        public bool cameraZone;
        public bool forceCameraZoneAtStart;

        public string sendMessageOnZoneEnter;

        BoxCollider2D boundsCollider2D;
        public Bounds bounds;

        public List<PrefabSpawner> spawners = new List<PrefabSpawner>();

        // Use this for initialization
        void Start()
        {
            boundsCollider2D = GetComponent<BoxCollider2D>();
            bounds = boundsCollider2D.bounds;

            if (forceCameraZoneAtStart && cameraZone)
                EnterZone();
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            if (bounds == CameraManager.currentBounds)
                return;

            if (col.gameObject.name == "Player")
            {
                EnterZone();
            }
        }

        void OnTriggerStay2D(Collider2D col)
        {
            if (bounds == CameraManager.currentBounds)
            {
                return;
            }

            if (col.gameObject.tag == "Player")
            {
                EnterZone();
            }
        }

        void EnterZone()
        {
            if(cameraZone)
                CameraManager.SetCameraBounds(this);

            if(sendMessageOnZoneEnter != null && sendMessageOnZoneEnter.Length > 0)
            {
                SpawnManager.SendMessageToSpawnersInZone(sendMessageOnZoneEnter);
            }
        }
    }
}