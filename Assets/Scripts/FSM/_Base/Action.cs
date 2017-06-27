using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : ScriptableObject {

    public virtual void BeginAction(StateMachine stateMachine) { }

    public virtual void UpdateAction(StateMachine stateMachine) { }

    public virtual void EndAction(StateMachine stateMachine) { }
         
}
