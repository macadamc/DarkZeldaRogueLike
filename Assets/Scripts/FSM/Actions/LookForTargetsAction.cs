using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ShadyPixel/FSM/Actions/Look For Targets")]
public class LookForTargetsAction : Action {

    public string[] targetTags;

    public bool faceTarget;

    public override void BeginAction(StateMachine stateMachine)
    {
        base.BeginAction(stateMachine);
    }

    public override void UpdateAction(StateMachine stateMachine)
    {
        base.UpdateAction(stateMachine);

        CheckVision(stateMachine);
    }

    public override void EndAction(StateMachine stateMachine)
    {
        base.EndAction(stateMachine);
    }

    public void CheckVision(StateMachine stateMachine)
    {
        Entity entity = stateMachine.entity;

        stateMachine.visionToTarget = false;
        entity.stateMachineVisionToTarget = false;

        Collider2D[] objs = Physics2D.OverlapCircleAll(entity.transform.position, entity.stats.visionDistance, entity.stats.visionLayer);

        foreach (Collider2D col in objs)
        {
            RaycastHit2D hit = Physics2D.Raycast(entity.transform.position, (col.transform.position - entity.transform.position).normalized, entity.stats.visionDistance, entity.stats.visionLayer);

            for (int i = 0; i < targetTags.Length; i++)
            {
                if (hit.collider!=null && hit.collider.tag == targetTags[i])
                {
                    stateMachine.targetTransform = hit.collider.transform;
                    entity.stateMachineTargetTransform = hit.collider.transform;
                    stateMachine.visionToTarget = true;
                    entity.stateMachineVisionToTarget = true;
                }

            }
        }
    }
}
