using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action : ScriptableObject
{
    public virtual void StartAction(StateController controller)
    {

    }

    public abstract void Act(StateController controller);


}
