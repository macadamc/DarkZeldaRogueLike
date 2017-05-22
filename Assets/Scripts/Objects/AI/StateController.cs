using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ShadyPixel.Astar;
//using Complete;

[RequireComponent(typeof(Entity))]
[RequireComponent(typeof(Pathfinder))]
public class StateController : MonoBehaviour {

    public TransitionController transitionController;

    public State currentState;
    public State remainState;

    public Vector2 targetPos;

    //public Dictionary<string, string> blackboardValues = new Dictionary<string, string>();
    public Dictionary<string, float> floatDictionary = new Dictionary<string, float>();

    public bool waiting;

    public List<GameObject> targets;
	public bool aiActive;

    bool start;
    [HideInInspector]
    public Entity entity;
    [HideInInspector]
    public Pathfinder pathfinder;

    bool init;


    void Awake () 
	{
        targetPos = transform.position;
        entity = GetComponent<Entity>();
        pathfinder = GetComponent<Pathfinder>();
        init = true;
	}

    void Update()
    {
        if (!init)
            return;

        if (!aiActive)
            return;

        if (PauseManager.gamePaused || entity.stunLocked || !entity.rend.isVisible)
            return;

        if(!start)
        {
            currentState.OnStart(this);
            start = true;
        }

        CheckTransitions();

        currentState.UpdateState(this);
    }

    void OnDrawGizmos()
    {
        if(currentState != null)
        {
            Gizmos.color = currentState.gizmoColor;
            Gizmos.DrawWireSphere(transform.position, 1.25f);
        }
    }

    public void TransitionToState(State nextState)
    {
        if (nextState != remainState)
        {
            currentState = nextState;
        }
    }

    private void CheckTransitions()
    {
        for (int i = 0; i < transitionController.transitions.Length; i++)
        {
            bool allTrue = true;

            for (int j = 0; j < transitionController.transitions[i].decisions.Length; j++)
            {
                bool decisionSucceeded = transitionController.transitions[i].decisions[j].Decide(this);

                if (decisionSucceeded == false)
                    allTrue = false;
            }


            if (allTrue)
            {
                TransitionToState(transitionController.transitions[i].trueState);
            }
            else
            {
                TransitionToState(transitionController.transitions[i].falseState);
            }

        }
    }

    public float DistanceToTargetPos()
    {
        float distance = Vector2.Distance(transform.position, targetPos);
        return distance;
    }

    public void Something()
    {

    }

}