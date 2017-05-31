using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePickup : BaseInteractable
{
    
    public Shadow shadow;
    Rigidbody2D rb;
    public float lerpSpd;
    public AudioClip pickupSfx;

    public Collider2D coll;

    public override void Awake()
    {
        base.Awake();
        shadow = GetComponent<Shadow>();
        rb = GetComponent<Rigidbody2D>();

        coll = transform.FindChild("Shadow").GetComponent<Collider2D>();
    }

    public virtual void FixedUpdate()
    {
        if (shadow.objOffset <= .01f && shadow.yVel <= 0.01f)
        {
            coll.enabled = false;
            trigger.enabled = true;

            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, lerpSpd * Time.deltaTime);
        }
        else if (trigger.enabled)
        {
            trigger.enabled = false;
            coll.enabled = true;

            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player == null)
                return;

            OnTriggerExit2D(player.GetComponent<Collider2D>());
        }

    }
}