using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePickup : BaseInteractable
{
    
    public Shadow shadow;
    Rigidbody2D rb;
    public float lerpSpd;
    public AudioClip pickupSfx;

    public override void Awake()
    {
        base.Awake();
        shadow = GetComponent<Shadow>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        coll.isTrigger = false;
    }

    public virtual void FixedUpdate()
    {

        if (shadow.objOffset <= .01f)
        {
            coll.isTrigger = true;
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, lerpSpd * Time.deltaTime);
        }
        else if (coll.isTrigger)
        {
            coll.isTrigger = false;
        }

    }

    public void OncollisionEnter2D(Collision2D other)
    {
        if (other.transform.root.name == "MapChunks")
        {
            rb.velocity = -rb.velocity;
        }
    }
}