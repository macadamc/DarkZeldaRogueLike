using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RangedBase : ActiveItem {

    new public RangedBaseSO itemData;
    GameObject HeldSprite;

    public override void OnAttackTriggered(Entity entity)
    {
        if (HeldSprite != null)
        {
            OnAttackEnd(entity);
        }

        HeldSprite = (GameObject)GameObject.Instantiate(itemData.HeldObj, entity.gameObject.transform);
        HeldSprite.transform.localPosition = entity.atkPos;
        HeldSprite.transform.rotation = GetQuaternionFromEntityLookDirection(entity);
        entity.statMods.maxSpeed -= entity.stats.moveSpeed / 8;
    }

    public override void OnAttackHeld(Entity entity)
    {
        if (HeldSprite != null)
        {
            HeldSprite.transform.localPosition = entity.atkPos;
            HeldSprite.transform.rotation = GetQuaternionFromEntityLookDirection(entity);
        }
        //entity.curMaxSpd = entity.stats.moveSpeed / 8;
    }

    public override void OnAttackEnd(Entity entity)
    {
        CreateProjectile(entity, (entity.lookDir * itemData.baseSpeed) + entity.moveVector);
        DestroyHeldSprite();
        entity.statMods.maxSpeed += entity.stats.moveSpeed / 8;
    }

    public override void OnHit(Collider2D other, Entity entity, GameObject AttackObject)
    {
        Entity e = other.GetComponent<Entity>();
        Destructable d = other.GetComponent<Destructable>();
        WeaponAttack attack = AttackObject.GetComponent<WeaponAttack>();

        if(attack.owner != e)
        {
            if (e != null && e != entity)
            {
                e.AddKnockback((e.transform.position - entity.transform.position).normalized * itemData.knockBack);
                e.StunLock(itemData.knockBack / 20);
                e.ModifyHealth(-itemData.baseDamage);
                GameObject.Destroy(AttackObject);
                return;
            }
        }
        if(attack.d_owner != d)
        {
            if (d != null)
            {
                d.ModifyHealth(-itemData.baseDamage);
                GameObject.Destroy(AttackObject);
                return;
            }
        }
        if (other.gameObject.transform.root.gameObject.name == "MapChunks")
        {
            GameObject.Destroy(AttackObject);
        }
    }

    public override void Start(Entity entity)
    {
        itemData = (RangedBaseSO)base.itemData;
    }

    public override void OnEquip(Entity entity)
    {
        //throw new NotImplementedException();
    }

    public override void OnUnequip(Entity entity)
    {
        //throw new NotImplementedException();
    }

    public void CreateProjectile(Entity entity, Vector2 velocityVector)
    {
        GameObject arrow = (GameObject)GameObject.Instantiate(itemData.projectile, entity.transform);
        arrow.transform.localPosition = entity.atkPos;
        arrow.GetComponent<WeaponAttack>().owner = entity;
        arrow.GetComponent<DestroyAfterTime>().time = itemData.MaxFlightTime;
        arrow.GetComponent<Rigidbody2D>().velocity = velocityVector;
        arrow.GetComponent<WeaponAttack>().d_owner = entity.gameObject.GetComponent<Destructable>();
    }
    public void DestroyHeldSprite()
    {
        if (HeldSprite != null)
        {
            GameObject.Destroy(HeldSprite);
        }
    }
}
