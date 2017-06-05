using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtOnContact : MonoBehaviour {

    public int damage;
    public float knockback;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    //this function hurts all objects that can be hurt.
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Player" || col.gameObject.tag == "Entity" || col.gameObject.tag == "Desctructable")
        {
            Entity entity = col.gameObject.GetComponent<Entity>();
            Destructable des;

            if (entity == null)
            {
                //if entity is null, check for destructable instead
                des = col.gameObject.GetComponent<Destructable>();

                if (des == null)
                    return;
                else
                {
                    //damage if it is a destructable
                    des.ModifyHealth(-damage);
                }
            }
            else
            {
                //if entity is not null, damage, and add knockback and stunlock
                entity.AddKnockback((col.transform.position - transform.position).normalized * knockback);
                entity.StunLock(knockback / 20);   //should have stunlock value in weapon maybe
                entity.ModifyHealth(-damage);
            }
        }

    }
}
