using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ShadyPixel/FSM/Actions/Chase Action")]
public class ChaseAction : Action
{

    public float timeBetweenMovements;
    public float movementTimerRandomness;

    public float moveStr;
    public float moveStrRandomness;
    [Range(0, 1)]

    public float deadZone;

    public bool additiveVelocity;
    public bool step;
    public float stepTime;
    public float stepTimeRandomness;

    public override void BeginAction(StateMachine stateMachine)
    {
        base.BeginAction(stateMachine);

        stateMachine.stateTimers.Add("chaseMoveTimer", Time.time);
        stateMachine.stateTimers.Add("chaseStepTimer", Time.time);
    }

    public override void UpdateAction(StateMachine stateMachine)
    {
        base.UpdateAction(stateMachine);

        if (stateMachine.entity.stunLocked || PauseManager.gamePaused)
            return;

        if (Time.time >= stateMachine.stateTimers["chaseStepTimer"])
        {
            if (step)
                stateMachine.entity.LerpStop();

            if (Time.time >= stateMachine.stateTimers["chaseMoveTimer"])
            {
                Move(stateMachine);
            }
        }

    }

    public void Move(StateMachine stateMachine)
    {
        if (additiveVelocity)
            stateMachine.entity.moveVector += GetMoveVector(stateMachine);
        else
            stateMachine.entity.moveVector = GetMoveVector(stateMachine);

        SetMoveTimer(stateMachine);

        if (step)
            SetStepTimer(stateMachine);

    }

    public override void EndAction(StateMachine stateMachine)
    {
        base.EndAction(stateMachine);
        stateMachine.stateTimers.Remove("chaseMoveTimer");
        stateMachine.stateTimers.Remove("chaseStepTimer");
    }

    void SetStepTimer(StateMachine stateMachine)
    {
        stateMachine.stateTimers["chaseStepTimer"] = Time.time + stepTime + Random.Range(-stepTimeRandomness, stepTimeRandomness);
        stateMachine.entity.stepOnGround = false;
    }

    public Vector2 GetMoveVector(StateMachine stateMachine)
    {
        if(stateMachine.visionToTarget)
        {
            Vector2 fleeVector = (stateMachine.targetTransform.position - stateMachine.entity.transform.position).normalized;
            return fleeVector * (moveStr + Random.Range(-moveStrRandomness, moveStrRandomness));
        }
        else
        {
            return Vector2.zero;
        }
    }

    public void SetMoveTimer(StateMachine stateMachine)
    {
        stateMachine.stateTimers["chaseMoveTimer"] = Time.time + timeBetweenMovements + Random.Range(-movementTimerRandomness, movementTimerRandomness);
    }

}