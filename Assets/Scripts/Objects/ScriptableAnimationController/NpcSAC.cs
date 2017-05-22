using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "ShadyPixel/ScriptableAnimationControllers/Human")]
public class NpcSAC : ScriptableAnimationController {

    public override void Animate(Entity entity)
    {
        entity.anim.SetFloat("speed", entity.rb.velocity.magnitude);
        entity.anim.SetFloat("inputX", entity.lookDir.x);
        entity.anim.SetFloat("inputY", entity.lookDir.y);

        if (entity.attack)
            entity.anim.SetTrigger("attack");

    }

}
