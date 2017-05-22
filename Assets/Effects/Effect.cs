using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Effect {

    [System.NonSerialized]
    public Entity target;

    public virtual void OnStart() { }

    public virtual void OnEnd() { }
}
