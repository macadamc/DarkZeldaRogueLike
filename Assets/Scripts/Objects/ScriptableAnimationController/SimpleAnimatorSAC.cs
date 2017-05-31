using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "ShadyPixel/ScriptableAnimationControllers/SimpleAnimator")]
public class SimpleAnimatorSAC : ScriptableAnimationController {

    public override void Animate(Entity entity)
    {
        if(entity.rb.velocity.magnitude > 0.05)
        {
            entity.anim.SetBool("walking", true);
        }
        else
        {
            entity.anim.SetBool("walking", false);
        }

        if (entity.attack)
            entity.anim.SetTrigger("attack");

    }

}
