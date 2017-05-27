using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadyPixel.CameraSystem
{
    public class CameraFollow : MonoBehaviour
    {
        public List<Transform> targets = new List<Transform>();
        public float inBoundsLerpSpd = 0.5f;
        public float transitionLerpSpd = 0.9f;
        public Bounds curBounds;

        float lerpSpd;
        Vector2 targetPos;

        float height;
        float width;
        float heightExtents;
        float widthExtents;

        // Use this for initialization
        void Start()
        {
            FindCameraWidthAndHeight();

            if(targets.Count == 0)
                targets.Add(GameObject.FindGameObjectWithTag("Player").transform);

        }

        void FindCameraWidthAndHeight()
        {
            height = 2f * Camera.main.orthographicSize;
            width = height * Camera.main.aspect;
            heightExtents = height * 0.5f;
            widthExtents = width * 0.5f;
        }

        // Update is called once per frame
        void Update()
        {
            curBounds = CameraManager.currentBounds;

            if (InBounds())
                lerpSpd = inBoundsLerpSpd;
            else
                lerpSpd = transitionLerpSpd;

            MoveTowardTargetPosition();
        }

        // finds average of the targets in the target list. checks to see if in bounds and then clamps the final return vector2
        public Vector2 GetAverageTargetPosition(bool clampedToCameraZone)
        {
            Vector2 newTargetPos = Vector2.zero;
            if(targets.Count <= 1)
            {
                newTargetPos = targets[0].position;
            }
            else
            {
                foreach (Transform target in targets)
                {
                    newTargetPos += (Vector2)target.position;
                }
            }

            newTargetPos /= targets.Count;

            if(clampedToCameraZone)
            {
                newTargetPos = ClampCameraPosition(newTargetPos);
            }

            return newTargetPos;
        }

        public Vector2 ClampCameraPosition(Vector2 positionToClamp)
        {
            Vector2 newPos = positionToClamp;

            // clamp X
            newPos.x = Mathf.Clamp(newPos.x, curBounds.center.x - curBounds.extents.x + widthExtents, curBounds.center.x + curBounds.extents.x - widthExtents);

            // clamp y
            newPos.y = Mathf.Clamp(newPos.y, curBounds.center.y - curBounds.extents.y + heightExtents, curBounds.center.y + curBounds.extents.y - heightExtents);


            return newPos;
        }

        public bool InBounds()
        {
            bool inBounds = true;

            if (transform.position.x < curBounds.center.x - curBounds.extents.x + widthExtents || transform.position.x > curBounds.center.x + curBounds.extents.x - widthExtents)
                inBounds = false;

            if (transform.position.y < curBounds.center.y - curBounds.extents.y + heightExtents || transform.position.y > curBounds.center.y + curBounds.extents.y - heightExtents)
                inBounds = false;

            return inBounds;
        }

        void MoveTowardTargetPosition()
        {
            if (targets.Count == 0)
                return;

            // checks to see if has bounds. If true, return clamped bounds, if not return unclamped.
            Vector3 newMovePos = Vector2.Lerp(transform.position, GetAverageTargetPosition(true), lerpSpd*Time.deltaTime);
            // keeps z position the same as it was. (z must be less that other gameobjects in order to render right. ( normally camera is set to -10 ) )
            newMovePos.z = transform.position.z;
            transform.position = newMovePos;
        }
    }
}
