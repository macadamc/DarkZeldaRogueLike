using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBounce : MonoBehaviour {

    public Rigidbody2D rb;

    void Awake ()
    {
        rb = GetComponentInParent<Rigidbody2D>();
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        rb.velocity = -rb.velocity;
    }
}
