using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu (menuName ="ShadyPixel/FiniteStateMachine/Actions/Random Move Action")]
public class RandomMoveAction : Action {

    public float timeBetweenMovements;
    public float timerRandomness;

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
        if (!controller.floatDictionary.ContainsKey("randomMoveTimer"))
        {
            //if key does not exist
            //create keys
            controller.floatDictionary.Add("randomMoveTimer", RandomMovementTimerValue());
        }
    }

    private void RandomMove(StateController controller)
    {
        if (controller.floatDictionary["randomMoveTimer"] > 0)
        {
            //if timer greater than 0
            controller.floatDictionary["randomMoveTimer"] = controller.floatDictionary["randomMoveTimer"] - Time.deltaTime;
            MoveToPosition(controller);
        }
        else
        {
            //timer less than 0
            controller.floatDictionary["randomMoveTimer"] = RandomMovementTimerValue();
            SetRandomTargetPos(controller);
        }
    }

    float RandomMovementTimerValue()
    {
        return timeBetweenMovements + UnityEngine.Random.Range(-timerRandomness, timerRandomness);
    }

    void MoveToPosition(StateController controller)
    {
        if (HasVisionToTargetPos(controller) == false)
            SetRandomTargetPos(controller);

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
