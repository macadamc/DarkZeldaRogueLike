using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShadyPixel.CameraSystem;
[Serializable]
public abstract class MeleeBase : ActiveItem {

    new MeleeBaseSO itemData;

    public override void Start(Entity entity)
    {
        itemData = (MeleeBaseSO)base.itemData;
    }

    public override void OnAttackEnd(Entity entity)
    {
        //create the "weapon swing" gameobject.
        GameObject g = (GameObject)GameObject.Instantiate(itemData.HeldObj, entity.gameObject.transform);
        //set the attacks position and rotation to match the entitys atkPos and.. TODO : look direction?
        g.transform.localPosition = entity.atkPos;
        g.transform.rotation = GetQuaternionFromEntityLookDirection(entity);
        entity.attack = true;
        entity.attackDelay = itemData.AttackDelay;
        entity.curMaxSpd = entity.stats.moveSpeed;
    }

    public override void OnHit(Collider2D other, Entity entity, GameObject AttackObject)
    {
        //test to see if the other collider is a entity.
        Entity e = other.GetComponent<Entity>();
        Destructable d = other.GetComponent<Destructable>();
        if (e != null && e != entity)
        {
            GameManager.GM.pauseManager.StartCoroutine(GameManager.GM.pauseManager.HitPause(0.05f));
            e.AddKnockback((e.transform.position - entity.transform.position).normalized * itemData.knockBack);
            //e.AddKnockback(entity.lookDir * itemData.knowckBack);
            e.StunLock(itemData.knockBack / 20);   //should have stunlock value in weapon maybe
            e.ModifyHealth(-itemData.baseDamage);

            CameraShake shake = Camera.main.GetComponent<CameraShake>();
            shake.Shake(0.1f, 0.1f);

            return;
        }
        else if (d != null)
        {
            d.ModifyHealth(-itemData.baseDamage);
        }
    }

    public override void OnEquip(Entity entity)
    {
    }

    public override void OnUnequip(Entity entity)
    {
    }

    public override void OnAttackTriggered(Entity entity)
    {
        entity.curMaxSpd = entity.stats.moveSpeed / 1.5f;
    }

    public override void OnAttackHeld(Entity entity)
    {
        entity.curMaxSpd = entity.stats.moveSpeed / 1.5f;
    }
}