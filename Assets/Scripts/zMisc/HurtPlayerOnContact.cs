using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShadyPixel.CameraSystem;

public class HurtPlayerOnContact : MonoBehaviour {

    public int damage;
    public float knockback;

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Player")
        {
            Entity entity = col.gameObject.GetComponent<Entity>();

            if (entity == null)
                return;

            CameraShake shake = Camera.main.GetComponent<CameraShake>();

            entity.AddKnockback((col.transform.position- transform.position).normalized * knockback);
            entity.StunLock(knockback / 20);   //should have stunlock value in weapon maybe
            entity.ModifyHealth(-damage);
            GameManager.GM.pauseManager.StartCoroutine(GameManager.GM.pauseManager.HitPause(0.1f));
            shake.Shake(knockback / 50, 0.1f);
        }
    }
}
