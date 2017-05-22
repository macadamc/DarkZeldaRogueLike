using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScriptableAnimationController : ScriptableObject {

    public abstract void Animate(Entity entity);
}
