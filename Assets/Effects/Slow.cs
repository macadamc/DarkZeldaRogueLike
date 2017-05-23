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
        float spdMod = target.stats.moveSpeed * percentage;

        target.statMods.maxSpeed -= spdMod;
        target.statMods.attackDelay += percentage;

        yield return new WaitForSeconds(duration);

        target.statMods.maxSpeed += spdMod;
        target.statMods.attackDelay -= percentage;
    }
}
