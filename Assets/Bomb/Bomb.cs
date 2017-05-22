using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : ActiveItem {

    new BombSO itemData;

    public override void Start(Entity entity)
    {
        itemData = (BombSO)base.itemData;
    }

    public override void OnAttackEnd(Entity entity)
    {
        throw new NotImplementedException();
    }

    public override void OnAttackHeld(Entity entity)
    {
        throw new NotImplementedException();
    }

    public override void OnAttackTriggered(Entity entity)
    {
        throw new NotImplementedException();
    }

    public override void OnEquip(Entity entity)
    {
        throw new NotImplementedException();
    }

    public override void OnHit(Collider2D other, Entity entity, GameObject AttackObject)
    {
        throw new NotImplementedException();
    }

    public override void OnUnequip(Entity entity)
    {
        throw new NotImplementedException();
    }
}
