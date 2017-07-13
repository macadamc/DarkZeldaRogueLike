using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "ShadyPixel/FSM/Actions/Random Move Action")]
public class RandomMoveAction : Action {

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

        stateMachine.stateTimers.Add("randomMoveTimer", Time.time);
        stateMachine.stateTimers.Add("randomStepTimer", Time.time);
    }

    public override void UpdateAction(StateMachine stateMachine)
    {
        base.UpdateAction(stateMachine);

        if (stateMachine.entity.stunLocked || PauseManager.gamePaused)
            return;

        if (Time.time >= stateMachine.stateTimers["randomStepTimer"])
        {
            if (step)
                stateMachine.entity.LerpStop();

            if (Time.time >= stateMachine.stateTimers["randomMoveTimer"])
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
        stateMachine.stateTimers.Remove("randomMoveTimer");
        stateMachine.stateTimers.Remove("randomStepTimer");
    }

    void SetStepTimer(StateMachine stateMachine)
    {
        stateMachine.stateTimers["randomStepTimer"] = Time.time + stepTime + Random.Range(-stepTimeRandomness, stepTimeRandomness);
        stateMachine.entity.stepOnGround = false;
    }

    public Vector2 GetMoveVector(StateMachine stateMachine)
    {
        Vector2 randomVector = Random.insideUnitCircle;
        if (randomVector.magnitude < deadZone)
            randomVector = Vector2.zero;

        return randomVector * (moveStr + Random.Range(-moveStrRandomness, moveStrRandomness));
    }

    public void SetMoveTimer(StateMachine stateMachine)
    {
        stateMachine.stateTimers["randomMoveTimer"] = Time.time + timeBetweenMovements + Random.Range(-movementTimerRandomness, movementTimerRandomness);
    }



}
