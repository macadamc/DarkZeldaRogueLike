using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBuffInteractable : BaseInteractable {
    [Range(0, 60 * 10)]
    public float durationInSeconds;

    [Range(-1,1)]
    public float modifier;

    public override void Interact(Entity other)
    {
        Debug.Log("SpeedBuff Gained :" + other.name);
        new Slow(durationInSeconds, -modifier, other);
        other.targetInteractable = null;
        GetComponentInParent<SpriteRenderer>().color = new Color(.6f, .2f, .2f);
        trigger.enabled = false;
    }
}
