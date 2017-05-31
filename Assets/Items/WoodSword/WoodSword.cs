using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShadyPixel.CameraSystem;

[Serializable]
public class WoodSword : MeleeBase {

    public override void OnAttackEnd(Entity entity)
    {
        base.OnAttackEnd(entity);
        if(triggerHoldAtk)
        {
            entity.AddKnockback(entity.lookDir * 25f);
        }
    }

}