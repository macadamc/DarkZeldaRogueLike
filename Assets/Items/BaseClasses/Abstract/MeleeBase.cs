﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShadyPixel.CameraSystem;
[Serializable]
public abstract class MeleeBase : ActiveItem {

    new MeleeBaseSO itemData;
    GameObject HeldSprite;
    SpriteRenderer sprite;

    public override void Start(Entity entity)
    {
        base.Start(entity);
        itemData = (MeleeBaseSO)base.itemData;


    }


    public override void OnAttackTriggered(Entity entity)
    {
        base.OnAttackTriggered(entity);
        if (HeldSprite != null)
        {
            OnAttackEnd(entity);
        }

        HeldSprite = (GameObject)GameObject.Instantiate(itemData.HeldObj, entity.gameObject.transform);

        HeldSprite.transform.position = entity.atkPos;

        sprite = HeldSprite.GetComponent<SpriteRenderer>();
        if (sprite == null)
            sprite = HeldSprite.GetComponentInChildren<SpriteRenderer>();

        sprite.flipX = entity.rend.flipX;
        //HeldSprite.transform.rotation = GetQuaternionFromEntityLookDirection(entity);
    }

    public override void OnAttackHeld(Entity entity)
    {
        if (HeldSprite != null)
        {
            HeldSprite.transform.position = entity.transform.position;
            sprite.flipX = entity.rend.flipX;
            //HeldSprite.transform.rotation = GetQuaternionFromEntityLookDirection(entity);
        }
        //entity.curMaxSpd = entity.stats.moveSpeed / 8;
    }

    public override void OnAttackEnd(Entity entity)
    {
        base.OnAttackEnd(entity);
        //create the "weapon swing" gameobject.
        GameObject g = (GameObject)GameObject.Instantiate(itemData.attackObject, entity.gameObject.transform);
        //set the attacks position and rotation to match the entitys atkPos and.. TODO : look direction?
        g.transform.localPosition = entity.atkPos;
        g.transform.rotation = GetQuaternionFromEntityLookDirection(entity);
        entity.attack = true;
        g.GetComponent<WeaponAttack>().weapon = this;
        DestroyHeldSprite();

    }

    public override void OnHit(Collider2D other, Entity entity, GameObject AttackObject)
    {
        //test to see if the other collider is a entity.
        Entity e = other.GetComponent<Entity>();
        Destructable d = other.GetComponent<Destructable>();
        CameraShake shake = Camera.main.GetComponent<CameraShake>();
        if (e != null && e != entity)
        {
            GameManager.GM.pauseManager.StartCoroutine(GameManager.GM.pauseManager.HitPause(0.05f));
            e.AddKnockback((e.transform.position - entity.transform.position).normalized * itemData.knockBack);
            //e.AddKnockback(entity.lookDir * itemData.knowckBack);
            e.StunLock(0.2f);   //should have stunlock value in weapon maybe
            e.ModifyHealth(-itemData.baseDamage);

            shake.Shake(itemData.screenshakeStr, 0.1f);
        }
        else if (d != null)
        {
            d.ModifyHealth(-itemData.baseDamage);

            shake.Shake(itemData.screenshakeStr/2, 0.1f);
        }
    }

    public override void OnEquip(Entity entity)
    {
    }

    public override void OnUnequip(Entity entity)
    {
    }

    public void DestroyHeldSprite()
    {
        if (HeldSprite != null)
        {
            GameObject.Destroy(HeldSprite);
        }
    }
}