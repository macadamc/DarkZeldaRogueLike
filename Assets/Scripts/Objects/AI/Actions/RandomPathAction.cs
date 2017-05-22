using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu (menuName ="ShadyPixel/FiniteStateMachine/Actions/Random Path Action")]
public class RandomPathAction : Action {

    public float timeBetweenMovements;
    public float timerRandomness;
    public float distanceThreshold = 1.1f;

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
        if (!controller.floatDictionary.ContainsKey("randomPathTimer"))
        {
            //if key does not exist
            //create keys
            controller.floatDictionary.Add("randomPathTimer", RandomMovementTimerValue());
        }
    }

    private void RandomMove(StateController controller)
    {
        if(controller.pathfinder.followingPath)
        {
            return;
        }
        else
        {
            if (controller.floatDictionary["randomPathTimer"] > 0)
            {
                //if timer greater than 0
                controller.floatDictionary["randomPathTimer"] = controller.floatDictionary["randomPathTimer"] - Time.deltaTime;
            }
            else
            {
                //timer less than 0
                controller.floatDictionary["randomPathTimer"] = RandomMovementTimerValue();
                SetRandomTargetPos(controller);
                controller.pathfinder.FindPathAndMoveTowardPosition(controller.targetPos);
            }
        }
    }

    float RandomMovementTimerValue()
    {
        return timeBetweenMovements + UnityEngine.Random.Range(-timerRandomness, timerRandomness);
    }

    private void SetRandomTargetPos(StateController controller)
    {
        controller.targetPos = (Vector2)controller.transform.position + (UnityEngine.Random.insideUnitCircle * controller.entity.stats.visionDistance);
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
