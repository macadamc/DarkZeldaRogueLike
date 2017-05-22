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
            cam = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            if (shakeTime > 0)
            {
                shakeTime -= Time.deltaTime;
                SetRandomPos(shakeStrength);
                shakeStrength = Mathf.Lerp(shakeStrength, 0, shakeTime);
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
            shakeStrength = str;
            shakeTime = time;
        }
    }
}