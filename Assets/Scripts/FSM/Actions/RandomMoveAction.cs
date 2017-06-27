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

    float stepTimer;

    float moveTimer;


    public override void BeginAction(StateMachine stateMachine)
    {
        base.BeginAction(stateMachine);
    }

    public override void UpdateAction(StateMachine stateMachine)
    {
        base.UpdateAction(stateMachine);



        if (stateMachine.entity.stunLocked || PauseManager.gamePaused)
            return;

        if (stepTimer <= 0)
        {
            if (step)
                stateMachine.entity.LerpStop();

            if (moveTimer > 0)
            {
                moveTimer -= Time.deltaTime;
            }
            else
            {
                Move(stateMachine);
            }

        }
        else
        {
            stepTimer -= Time.deltaTime;
        }

    }

    public void Move(StateMachine stateMachine)
    {
        if (additiveVelocity)
            stateMachine.entity.moveVector += GetMoveVector();
        else
            stateMachine.entity.moveVector = GetMoveVector();

        SetMoveTimer();

        if (step)
            SetStepTimer(stateMachine);

    }

    public override void EndAction(StateMachine stateMachine)
    {
        base.EndAction(stateMachine);
    }

    void SetStepTimer(StateMachine stateMachine)
    {
        stepTimer = stepTime + Random.Range(-stepTimeRandomness, stepTimeRandomness);
        stateMachine.entity.stepOnGround = false;
    }

    public Vector2 GetMoveVector()
    {
        Vector2 randomVector = Random.insideUnitCircle;
        if (randomVector.magnitude < deadZone)
            randomVector = Vector2.zero;

        return randomVector * (moveStr + Random.Range(-moveStrRandomness, moveStrRandomness));
    }

    public void SetMoveTimer()
    {
        moveTimer = timeBetweenMovements + Random.Range(-movementTimerRandomness, movementTimerRandomness);
    }



}
