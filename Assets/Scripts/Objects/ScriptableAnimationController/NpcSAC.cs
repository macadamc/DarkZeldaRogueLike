using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "ShadyPixel/ScriptableAnimationControllers/Human")]
public class NpcSAC : ScriptableAnimationController {

    public override void Animate(Entity entity)
    {
        entity.anim.SetFloat("speed", entity.rb.velocity.magnitude);

        if(!entity.strafe)
            entity.anim.SetFloat("inputY", entity.lookDir.y);

        entity.anim.SetBool("hold", entity.holding);

        if (entity.attack)
            entity.anim.SetTrigger("attack");

    }

}
