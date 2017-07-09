using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "ShadyPixel/FSM/Actions/Attack Target")]
public class AttackTargetAction : Action {

    //public GameObject bulletObject;

    public float timeBetweenAttacks;
    public float timerRandomness;

    float shotTimer;

    public override void BeginAction(StateMachine stateMachine)
    {
        base.BeginAction(stateMachine);
    }

    public override void UpdateAction(StateMachine stateMachine)
    {
        base.UpdateAction(stateMachine);

        if (stateMachine.entity.stunLocked || PauseManager.gamePaused || !stateMachine.visionToTarget)
            return;

        if (shotTimer > 0)
        {
            shotTimer -= Time.deltaTime;
        }
        else
        {
            Attack(stateMachine);
        }
    }

    public override void EndAction(StateMachine stateMachine)
    {
        base.EndAction(stateMachine);
    }

    public void Attack(StateMachine stateMachine)
    {
        SetShootTimer(stateMachine);
        stateMachine.entity.weapons[0].OnAttackEnd(stateMachine.entity);
        Debug.Log("shoot");
        /*
        Vector2 shootVector = new Vector2();
        shootVector = stateMachine.targetTransform.position - stateMachine.transform.position;
        shootVector.Normalize();

        GameObject bullet = (GameObject)GameObject.Instantiate(bulletObject, stateMachine.entity.transform.position, stateMachine.entity.transform.rotation, stateMachine.entity.transform);
        bullet.SetActive(false);
        bullet.transform.localPosition = stateMachine.entity.atkPos;
        bullet.GetComponent<Projectile>().owner = stateMachine.entity;
        bullet.GetComponent<Projectile>().moveVector = shootVector * 10;
        bullet.GetComponent<Projectile>().destroyAfterTime = 2f;
        bullet.GetComponent<Projectile>().d_owner = stateMachine.entity.gameObject.GetComponent<Destructable>();
        bullet.SetActive(true);
        */

    }

    public void SetShootTimer(StateMachine stateMachine)
    {
        shotTimer = timeBetweenAttacks + Random.Range(-timerRandomness, timerRandomness);
    }


}