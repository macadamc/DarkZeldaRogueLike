using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashBoots : PassiveItem
{
    new DashBootsSO itemData;

    public override void OnEquip(Entity entity)
    {
        entity.statMods.maxSpeed += entity.stats.moveSpeed * itemData.SpeedMod;
    }

    public override void OnUnequip(Entity entity)
    {
        entity.statMods.maxSpeed -= entity.stats.moveSpeed * itemData.SpeedMod;
    }

    public override void Start(Entity entity)
    {
        itemData = (DashBootsSO)base.itemData;
    }
}
