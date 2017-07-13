using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour {

    public bool aiActive;

    public State startState;
    public State remainState;
    public State curState;
    public State lastState;


    [HideInInspector]
    public Entity entity;


    [HideInInspector]
    public Transform targetTransform;
    public bool visionToTarget;

    public Dictionary<string, float> stateTimers = new Dictionary<string, float>();

    public void Awake()
    {
        entity = gameObject.GetComponent<Entity>();
        curState = startState;
        curState.BeginState(this);
    }

    public void TransitionToState(State nextState)
    {
        if (nextState != remainState)
        {
            curState.EndState(this);
            lastState = curState;
            curState = nextState;
            curState.BeginState(this);
        }
    }

    public void Update()
    {
        if (entity.rend.isVisible == false)
            return;

        if (PauseManager.gamePaused == true)
            return;

        if (!aiActive)
            return;

        curState.UpdateState(this);
    }

}
