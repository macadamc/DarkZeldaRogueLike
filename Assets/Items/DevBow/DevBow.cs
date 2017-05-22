using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevBow : Bow {

    sMap map;

    public override void Start(Entity entity)
    {
        base.Start(entity);
        map = GameObject.Find("Manager").GetComponent<GameManager>().mapManager.map;
    }

    public override void OnHit(Collider2D other, Entity entity, GameObject AttackObject)
    {
        base.OnHit(other, entity, AttackObject);
        if (other.transform.root.name == "MapChunks")
        {
            // remove tile at this location.
            Vector2 vel = AttackObject.GetComponent<Rigidbody2D>().velocity.normalized;
            if (vel.x != .5f) { vel.Set(Mathf.RoundToInt(vel.x), vel.y); }
            if (vel.y != .5f) { vel.Set(vel.x, Mathf.RoundToInt(vel.y)); }

            int x = Mathf.FloorToInt(AttackObject.transform.position.x) + (map.width/2);
            int y = Mathf.FloorToInt(AttackObject.transform.position.y) + (map.height/2);
            map["Walls", x, y] = 0;
        }
    }
}
