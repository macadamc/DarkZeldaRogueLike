using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu (menuName ="ShadyPixel/FiniteStateMachine/Actions/Jump Toward Target Action")]
public class JumpTowardTargetPos : Action {

    public float timeBetweenMovements;
    public float timerRandomness;

    public float jumpTimeMin, jumpTimeMax;

    public float randomness = 1f;

    public override void StartAction(StateController controller)
    {

    }

    public override void Act(StateController controller)
    {
        Init(controller);
        JumpTowardTarget(controller);
    }

    private void Init(StateController controller)
    {
        if (!controller.floatDictionary.ContainsKey("jumpTimer"))
        {
            //if key does not exist
            //create keys
            controller.floatDictionary.Add("jumpTimer", RandomMoveTimerValue());
        }
        if (!controller.floatDictionary.ContainsKey("isJumping"))
        {
            //if key does not exist
            //create keys
            controller.floatDictionary.Add("isJumping", 0);
        }
    }

    private void JumpTowardTarget(StateController controller)
    {
        //is jumping
        if (controller.floatDictionary["isJumping"] > 0.05)
        {
            controller.floatDictionary["isJumping"] -= Time.deltaTime;
            MoveToPosition(controller);
            return;
        }
        //not jumping
        else
        {
            controller.floatDictionary["isJumping"] = 0;

            if (controller.floatDictionary["jumpTimer"] > 0)
            {
                controller.floatDictionary["jumpTimer"] -= Time.deltaTime;
                controller.entity.LerpStop();
            }
            else
            {
                //timer less than 0
                controller.floatDictionary["jumpTimer"] = RandomMoveTimerValue();
                controller.floatDictionary["isJumping"] = RandomJumpTimerValue();
            }
        }
    }

    float RandomMoveTimerValue()
    {
        return timeBetweenMovements + UnityEngine.Random.Range(-timerRandomness, timerRandomness);
    }
    float RandomJumpTimerValue()
    {
        return UnityEngine.Random.Range(jumpTimeMin, jumpTimeMax);
    }

    void MoveToPosition(StateController controller)
    {
        Vector2 movevector = (controller.targetPos - (Vector2)controller.entity.transform.position);
        if (movevector.magnitude > 1)
            movevector.Normalize();

        controller.entity.moveVector = movevector * controller.entity.stats.moveSpeed;
    }
}
