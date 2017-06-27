using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "ShadyPixel/FSM/New State")]
public class State : ScriptableObject {

    public Action[] actions;
    public Transition[] transitions;

    public virtual void BeginState(StateMachine stateMachine) {

        if (actions.Length == 0)
            return;

        foreach (Action a in actions)
            a.BeginAction(stateMachine);
    }

    public virtual void UpdateState(StateMachine stateMachine) {

        if (actions.Length == 0)
            return;

        foreach (Action a in actions)
            a.UpdateAction(stateMachine);

        CheckTransitions(stateMachine);

    }

    public virtual void EndState(StateMachine stateMachine) {

        if (actions.Length == 0)
            return;

        foreach (Action a in actions)
            a.EndAction(stateMachine);

    }

    private void CheckTransitions(StateMachine stateMachine)
    {
        for (int i = 0; i < transitions.Length; i++)
        {
            bool decisionSucceeded = transitions[i].decision.Decide(stateMachine);

            if (decisionSucceeded)
            {
                stateMachine.TransitionToState(transitions[i].trueState);
            }
            else
            {
                stateMachine.TransitionToState(transitions[i].falseState);
            }
        }
    }


}
