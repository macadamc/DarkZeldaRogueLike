using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName ="ShadyPixel/FiniteStateMachine/New State")]
public class State : ScriptableObject
{
    public Action[] actions;
    //public Transition[] transitions;
    public Color gizmoColor = Color.grey;

    public void UpdateState(StateController controller)
    {
        DoActions(controller);
    }

    private void DoActions(StateController controller)
    {
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i].Act(controller);
        }
    }


    public void OnStart(StateController controller)
    {
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i].StartAction(controller);
        }
    }
}
