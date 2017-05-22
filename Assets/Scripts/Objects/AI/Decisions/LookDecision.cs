using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName ="ShadyPixel/FiniteStateMachine/Decisions/Look Decision")]
public class LookDecision : Decision {

    public string[] targetTags;

    public override bool Decide(StateController controller)
    {
        bool targetVisable = Look(controller);
        return targetVisable;

    }

    private bool Look(StateController controller)
    {
        Entity entity = controller.entity;
        bool sawTarget = false;

        if (entity == null)
            return false;

        Collider2D[] targets = Physics2D.OverlapCircleAll(entity.transform.position,entity.stats.visionDistance,entity.stats.visionLayer);
        if(targets.Length > 0)
        {
            for (int i = 0; i < targets.Length; i++)
            {
                Vector2 raycastVector = (targets[i].gameObject.transform.position - controller.transform.position).normalized;
                RaycastHit2D hit = (Physics2D.Raycast(entity.transform.position, raycastVector, entity.stats.visionDistance, entity.stats.visionLayer));

                for (int j = 0; j < targetTags.Length; j++)
                {
                    if (hit)
                    {
                        if (hit.collider.gameObject.tag == targetTags[j])
                        {
                            sawTarget = true;
                            controller.targetPos = hit.transform.position;
                            Debug.DrawLine(controller.transform.position, hit.transform.position, Color.green);
                        }
                    }
                }
            }

        }

        return sawTarget;

    }

}
