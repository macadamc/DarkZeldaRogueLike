using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WeaponAttack : MonoBehaviour {
    public Entity owner;

    public Destructable d_owner;
    public ActiveItem weapon;

    public virtual void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    void Start()
    {
        if (owner == null)
        {
            owner = transform.parent.GetComponent<Entity>();
            d_owner = transform.parent.GetComponent<Destructable>();
        }
    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null)
            return;
        if (owner == null)
            return;

        if (other.gameObject == owner.gameObject)
            return;

        if (owner == null)
            return;

        if (d_owner == null)
            return;

        weapon.OnHit(other, owner, gameObject);
    }
}
