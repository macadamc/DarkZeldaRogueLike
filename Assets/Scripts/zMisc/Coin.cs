using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : BasePickup {

    public Animator anim;

    public override void Awake()
    {
        base.Awake();
        anim = transform.GetChild(0).GetComponent<Animator>();
    }

    public int amount
    {
        get
        {
            return Mathf.FloorToInt(anim.GetFloat("Amount"));
        }
        set
        {
            anim.SetFloat("Amount", value);
        }
    }

    public override void Interact(Entity other)
    {
        GameManager.GM.audioManager.PlaySFX(pickupSfx);
        Destroy(gameObject);

    }


}
