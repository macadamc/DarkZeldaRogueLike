using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "ShadyPixel/FSM/Decisions/Vision To Target")]
public class VisionToTargetDecision : Decision {

    public override bool Decide(StateMachine stateMachine)
    {
        return CheckVision(stateMachine);
    }

    public bool CheckVision(StateMachine stateMachine)
    {
        bool canSeeObject = stateMachine.visionToTarget;

        return canSeeObject;
    }
}
