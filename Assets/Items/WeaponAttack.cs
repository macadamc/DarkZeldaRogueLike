using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WeaponAttack : MonoBehaviour {
    [HideInInspector]
    public Entity owner;
    [HideInInspector]
    public Destructable d_owner;

    void Awake()
    {
        if (owner == null)
        {
            owner = transform.parent.GetComponent<Entity>();
            GetComponent<Collider2D>().isTrigger = true;
            d_owner = transform.parent.GetComponent<Destructable>();
        }
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        owner.wep.OnHit(other, owner, gameObject);
    }
}
