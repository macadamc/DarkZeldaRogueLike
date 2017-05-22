using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "ShadyPixel/EntityData/Stats")]
[System.Serializable]
public class Stats : ScriptableObject {

    public float health;
    public float acceleration;
    public float moveSpeed;
    public float knockbackDeceleration = 0.05f;
    public LayerMask visionLayer;
    public LayerMask obsLayer;
    public LayerMask softCollisions;
    public float visionDistance;
    public float fieldOfView;
    public SpawnObject[] spawnOnCreate;
    public SpawnObject[] spawnOnDeath;

}
