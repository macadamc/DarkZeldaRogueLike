using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ActiveItem : Item {

    internal float speedModifier;

    public override void Start(Entity entity)
    {
        ActiveItemSO itemData = (ActiveItemSO)this.itemData;
        speedModifier = (float)Math.Round(entity.stats.moveSpeed * (1f - itemData.attackingMoveSpeed), 2);
    }

    public virtual void OnAttackTriggered(Entity entity)
    {
        entity.statMods.maxSpeed -= speedModifier;
    }

    public abstract void OnAttackHeld(Entity entity);

    public virtual void OnAttackEnd(Entity entity)
    {
        entity.attackDelay = ((ActiveItemSO)itemData).AttackDelay + entity.statMods.attackDelay;
        entity.statMods.maxSpeed += speedModifier;
    }

    public abstract void OnHit(Collider2D other, Entity entity, GameObject AttackObject);

    public Quaternion GetQuaternionFromEntityLookDirection(Entity entity)
    {
        float angle = Mathf.Atan2(entity.lookDir.y, entity.lookDir.x) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(angle, Vector3.forward);

    }
}