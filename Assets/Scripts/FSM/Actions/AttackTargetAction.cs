using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "ShadyPixel/FSM/Actions/Attack Target")]
public class AttackTargetAction : Action {

    //public GameObject bulletObject;

    public float timeBetweenAttacks;
    public float timerRandomness;

    public override void BeginAction(StateMachine stateMachine)
    {
        base.BeginAction(stateMachine);
        stateMachine.stateTimers.Add("nextAttackAction", Time.time);
    }

    public override void UpdateAction(StateMachine stateMachine)
    {
        base.UpdateAction(stateMachine);

        if (stateMachine.entity.stunLocked || PauseManager.gamePaused || !stateMachine.visionToTarget)
            return;

        if(stateMachine.stateTimers.ContainsKey("nextAttackAction"))
        {
            if (Time.time > stateMachine.stateTimers["nextAttackAction"])
            {
                Attack(stateMachine);
            }
        }
    }

    public override void EndAction(StateMachine stateMachine)
    {
        base.EndAction(stateMachine);
        stateMachine.stateTimers.Remove("nextAttackAction");
    }

    public void Attack(StateMachine stateMachine)
    {
        if (stateMachine.entity.weapons[0] == null)
        {
            return;
        }
        stateMachine.stateTimers["nextAttackAction"] = NextActionTime();
        stateMachine.entity.weapons[0].OnAttackEnd(stateMachine.entity);
        Debug.Log("shoot");
    }

    public float NextActionTime()
    {
        return Time.time + timeBetweenAttacks + Random.Range(-timerRandomness, timerRandomness);
    }

}