using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "ShadyPixel/FSM/Decisions/Distance To Target")]
public class DistanceToTargetDesicion : Decision {

    public float distanceToCheck;

    public override bool Decide(StateMachine stateMachine)
    {
        return CheckDistance(stateMachine);
    }

    public bool CheckDistance(StateMachine stateMachine)
    {
        if (stateMachine.visionToTarget == false)
            return false;
        else
        {
            if (Vector2.Distance(stateMachine.transform.position, stateMachine.targetTransform.position) <= distanceToCheck)
                return true;
            else
                return false;
        }
    }
}
