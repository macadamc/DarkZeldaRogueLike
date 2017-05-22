using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "ShadyPixel/ScriptableAnimationControllers/Spider")]
public class SpiderSAC : ScriptableAnimationController {

    public override void Animate(Entity entity)
    {
        if(entity.controller.floatDictionary.ContainsKey("isJumping") && entity.controller.floatDictionary["isJumping"] > 0.1)
        {
            entity.anim.SetBool("isJumping", true);
            //entity.anim.SetBool("inAir", true);
        }
        else
        {
            entity.anim.SetBool("isJumping", false);
            //entity.anim.SetBool("inAir", false);
        }


    }

}
