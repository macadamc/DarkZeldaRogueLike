using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour{

    public bool aiActive;

    public State startState;
    public State remainState;
    public State curState;
    State lastState;

    [HideInInspector]
    public Entity entity;
    [HideInInspector]
    public Transform targetTransform;
    public bool visionToTarget;

    public void Start()
    {
        entity = gameObject.GetComponent<Entity>();
        curState = startState;
    }

    public void TransitionToState(State nextState)
    {
        if (nextState != remainState)
        {
            curState.EndState(this);
            curState = nextState;
            curState.BeginState(this);
        }
    }

    void Update()
    {
        if (!aiActive)
            return;

        curState.UpdateState(this);
    }

}
