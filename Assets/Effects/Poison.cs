using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Poison : ActiveEffect
{
    public int damage;

    public Poison(int damage, float duration, Entity target)
    {
        this.target = target;
        this.damage = damage;
        this.duration = duration;

        target.StartCoroutine(this.Update());
    }

    public override IEnumerator Update()
    {
        float tickRate = duration / (float)damage;
        WaitForSeconds Wait = new WaitForSeconds(tickRate);
        int ticks = 0;

        while (ticks < damage && target.isActiveAndEnabled == true)
        {
            target.ModifyHealth(-1);
            ticks++;

            yield return Wait;
        }
    }
}