using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadyPixel.CameraSystem
{
    public class CameraShake : MonoBehaviour
    {
        Camera cam;

        float shakeStrength;
        float shakeTime;

        // Use this for initialization
        void Awake()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (shakeTime > 0)
            {
                shakeTime -= Time.deltaTime;
                SetRandomPos(shakeStrength);
            }
            else
                shakeTime = 0f;

        }

        void SetRandomPos(float str)
        {
            Vector3 newPos;
            newPos = cam.transform.position + (Vector3)(Random.insideUnitCircle * str);
            cam.transform.position = newPos;
        }

        public void Shake(float str, float time)
        {
            cam = Camera.main;
            shakeStrength = str;
            shakeTime = time;
        }
        public void Shake(float str)
        {
            cam = Camera.main;
            shakeStrength = str;
            shakeTime = 0.05f;
        }
    }
}