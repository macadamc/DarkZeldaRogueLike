using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : WeaponAttack {

    public float destroyAfterTime;
    public SpawnObject[] spawnOnDeath;

    public Vector2 moveVector;
    Rigidbody2D rb;

    public override void Awake()
    {
        base.Awake();
    }

    

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = moveVector;
    }

    // Update is called once per frame
    void Update () {

        if (destroyAfterTime > 0)
            destroyAfterTime -= Time.deltaTime;
        else
            SpawnAndDestroy();

		
	}

    public override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        /*
        Entity oe = other.GetComponent<Entity>();
        if (oe != null && oe == owner)
            return;

        SpawnAndDestroy();
        */
    }

   public void SpawnAndDestroy()
    {
        foreach(SpawnObject so in spawnOnDeath)
        {
            Instantiate(so.objectToSpawn, (Vector2)transform.position + so.positionOffset + (Random.insideUnitCircle * so.positionRandomness), transform.rotation);
        }
        Destroy(gameObject);
    }
}
