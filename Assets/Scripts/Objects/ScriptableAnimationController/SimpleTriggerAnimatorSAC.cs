using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "ShadyPixel/ScriptableAnimationControllers/SimpleTriggerAnimator")]
public class SimpleTriggerAnimatorSAC : ScriptableAnimationController {

    public override void Animate(Entity entity)
    {
        if (entity.attack)
            entity.anim.SetTrigger("attack");

    }

}
