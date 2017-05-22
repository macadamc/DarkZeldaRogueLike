using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadyPixel.Astar
{
    [RequireComponent(typeof(Entity))]
    public class Pathfinder : MonoBehaviour
    {
        const float minPathUpdateTime = 0.2f;
        const float pathUpdateMoveThreshold = 0.5f;

        public bool drawPathGizmos = true;

        public float turnSpeed = 3f;
        public float turnDistance= 1.5f;
        public float stopDistance = 3f;

        public bool followingPath;

        public Vector2[] curWaypoints;

        Entity entity;
        Vector2 moveDir;
        float modifiedTurnSpeed;
        int curIndex;
        Path path;
        Vector2 targetPos;

        void Awake()
        {
            entity = GetComponent<Entity>();
            targetPos = transform.position;
        }

        public void OnPathFound(Vector2[] waypoints, bool pathSuccessful)
        {
            if (pathSuccessful)
            {
                path = new Path(waypoints, transform.position,turnDistance,stopDistance);
                StopCoroutine("FollowPath");
                StartCoroutine("FollowPath");
                curWaypoints = path.lookPoints;
                //Debug.Log("End Node: "+path.finishLineIndex +".");
            }
        }

        public void FindPathAndMoveTowardPosition(Vector2 targetPosition)
        {
            targetPos = targetPosition;
            PathRequestManager.RequestPath(new PathRequest(transform.position, targetPos, OnPathFound));
        }

        /*
        IEnumerator UpdatePath()
        {
            if (Time.timeSinceLevelLoad < 0.5f)
            {
                yield return new WaitForSeconds(0.3f);
            }
            PathRequestManager.RequestPath(new PathRequest(transform.position, targetPos, OnPathFound));

            float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
            Vector2 targetPosOld = targetPos;

            while (true)
            {
                yield return new WaitForSeconds(minPathUpdateTime);

                if ((targetPos - targetPosOld).sqrMagnitude > sqrMoveThreshold)
                {
                    PathRequestManager.RequestPath(new PathRequest(transform.position, targetPos, OnPathFound));
                    targetPosOld = targetPos;
                }
            }
        }
        */

        IEnumerator FollowPath()
        {
            followingPath = true;
            int pathIndex = 0;
            curIndex = pathIndex;
            float speedPercent = 1f;
            float modifiedSpeed = 0f;

            while (followingPath)
            {
                while (path.turnBoundaries[pathIndex].HasCrossedLine((Vector2)transform.position))
                {
                    if (pathIndex == path.finishLineIndex)
                    {
                        followingPath = false;
                        entity.moveVector = Vector2.zero;
                        break;
                    }
                    else {
                        pathIndex++;
                        curIndex = pathIndex;
                        //Debug.Log("Going to Node: " + pathIndex + ".");
                        break;
                    }
                }

                if (followingPath)
                {
                    if(entity.rb.velocity.magnitude > 1)
                    {
                        modifiedTurnSpeed = turnSpeed * 2;
                    }
                    else
                    {
                        modifiedTurnSpeed = turnSpeed;
                    }

                    if (pathIndex >= path.slowDownIndex && stopDistance > 0)
                    {
                        speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(transform.position) / stopDistance);
                        if (speedPercent < 0.1f)
                        {
                            followingPath = false;
                            entity.moveVector = Vector2.zero;
                            break;
                        }
                    }
                    //Tutorial Code
                    //Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - (Vector2)transform.position);
                    //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                    //transform.Translate(Vector2.right * Time.deltaTime * speed * speedPercent, Space.Self);
                    modifiedSpeed += entity.stats.acceleration*Time.deltaTime;
                    modifiedSpeed = Mathf.Clamp(modifiedSpeed, 0, entity.stats.moveSpeed);
                    Vector2 targetMoveDir = (path.lookPoints[pathIndex] - (Vector2)transform.position).normalized;
                    moveDir = Vector2.Lerp(moveDir, targetMoveDir, Time.deltaTime * modifiedTurnSpeed);
                    entity.moveVector = moveDir * (modifiedSpeed * speedPercent);
                    entity.lookDir = moveDir.normalized;
                }

                yield return null;
            }
        }

        void OnDrawGizmosSelected()
        {
            if (!drawPathGizmos)
                return;

            if (path != null)
            {
                DrawPathWithGizmos(curIndex);
            }
        }

        public void DrawPathWithGizmos(int curIndexInPath)
        {
            Gizmos.color = Color.black;
            /*
            foreach (Vector2 p in lookPoints)
            {
                Gizmos.DrawCube(p, Vector2.one);
            }
            */
            for (int i = curIndexInPath; i < path.lookPoints.Length; i++)
            {
                Gizmos.DrawCube(path.lookPoints[i], Vector2.one);
            }
            Gizmos.color = Color.cyan;
            /*
            foreach (Line l in turnBoundaries)
            {
                l.DrawWithGizmos(10);
            }
            */

        }

        /*
        void OnCollisionEnter2D(Collision2D collision)
        {

            for (int i = 0; i < obsLayers.Length; i++)
            {
                if (collision.gameObject.layer == obsLayers[i])
                {
                    StopCoroutine("FollowPath");
                    //StartCoroutine("UpdatePath");
                }
            }
        }
        */
    }
}