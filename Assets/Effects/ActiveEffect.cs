using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActiveEffect : Effect
{
    public float duration;

    public virtual IEnumerator Update() { return null; }

    public override void OnStart()
    {
        target.activeEffects.Add(this);
    }

    public override void OnEnd()
    {
        target.activeEffects.Remove(this);
    }
}