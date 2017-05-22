using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slow : ActiveEffect {

    float percentage;

    public Slow(float duration, float percentage, Entity target)
    {
        this.target = target;
        this.duration = duration;
        this.percentage = percentage;

        target.StartCoroutine(this.Update());
    }

    public override IEnumerator Update()
    {
        Debug.Log("Slowed :" + target.name);
        float oldSpd = target.curMaxSpd;
        target.curMaxSpd = target.stats.moveSpeed * (1f - percentage);

        yield return new WaitForSeconds(duration);

        target.curMaxSpd = oldSpd;
        
    }
}
