using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu (menuName ="ShadyPixel/FiniteStateMachine/Actions/Chase Action")]
public class ChaseAction : Action {

    public float timeBetweenMovements;
    public float timerRandomness;
    public float distanceThreshold = 1.1f;
    public float movePositionRandomness;

    public override void StartAction(StateController controller)
    {

    }

    public override void Act(StateController controller)
    {
        Init(controller);
        Chase(controller);
    }

    private void Init(StateController controller)
    {
        if (!controller.floatDictionary.ContainsKey("chaseTimer"))
        {
            //if key does not exist
            //create keys
            controller.floatDictionary.Add("chaseTimer", RandomMovementTimerValue());
        }
    }

    private void Chase(StateController controller)
    {
        if(controller.pathfinder.followingPath)
        {
            return;
        }
        else
        {
            if (controller.floatDictionary["chaseTimer"] > 0)
            {
                //if timer greater than 0
                controller.floatDictionary["chaseTimer"] = controller.floatDictionary["chaseTimer"] - Time.deltaTime;
            }
            else
            {
                //timer less than 0
                controller.floatDictionary["chaseTimer"] = RandomMovementTimerValue();
                controller.pathfinder.FindPathAndMoveTowardPosition(controller.targetPos+RandomPositionOffset());
            }
        }
    }

    float RandomMovementTimerValue()
    {
        return timeBetweenMovements + UnityEngine.Random.Range(-timerRandomness, timerRandomness);
    }
    Vector2 RandomPositionOffset()
    {
        Vector2 randomPos = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
        if (randomPos.magnitude > 1)
            randomPos.Normalize();
        randomPos *= movePositionRandomness;
        return randomPos;
    }
}
