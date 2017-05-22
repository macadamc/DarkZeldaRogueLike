using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu (menuName ="ShadyPixel/FiniteStateMachine/Actions/Random Jump Action")]
public class RandomJumpAction : Action {

    public float timeBetweenMovements;
    public float timerRandomness;

    public float jumpTimeMin, jumpTimeMax;

    [Range(0,1)]
    public float deadZone = 0.25f;

    public override void StartAction(StateController controller)
    {

    }

    public override void Act(StateController controller)
    {
        Init(controller);
        RandomMove(controller);
    }

    private void Init(StateController controller)
    {
        if (!controller.floatDictionary.ContainsKey("randomJumpTimer"))
        {
            //if key does not exist
            //create keys
            controller.floatDictionary.Add("randomJumpTimer", RandomMoveTimerValue());
        }
        if (!controller.floatDictionary.ContainsKey("isJumping"))
        {
            //if key does not exist
            //create keys
            controller.floatDictionary.Add("isJumping", 0);
        }
    }

    private void RandomMove(StateController controller)
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

            if (controller.floatDictionary["randomJumpTimer"] > 0)
            {
                controller.floatDictionary["randomJumpTimer"] -= Time.deltaTime;
                controller.entity.LerpStop();
            }
            else
            {
                //timer less than 0
                controller.floatDictionary["randomJumpTimer"] = RandomMoveTimerValue();
                SetRandomTargetPos(controller);
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

    private void SetRandomTargetPos(StateController controller)
    {
        Vector2 randomVector = UnityEngine.Random.insideUnitCircle;
        if (randomVector.magnitude < deadZone)
            randomVector = Vector2.zero;

        controller.targetPos = (Vector2)controller.transform.position + (randomVector * controller.entity.stats.visionDistance);
    }

    private bool HasVisionToTargetPos(StateController controller)
    {
        Vector2 raycastVector = (controller.targetPos - (Vector2)controller.transform.position).normalized;
        float distance = Vector2.Distance(controller.transform.position, controller.targetPos);
        RaycastHit2D hit = (Physics2D.Raycast(controller.transform.position, raycastVector, distance, controller.entity.stats.obsLayer));

        if (!hit)
            return true;
        else
            return false;
    }
}
