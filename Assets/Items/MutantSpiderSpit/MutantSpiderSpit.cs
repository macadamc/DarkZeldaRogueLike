using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutantSpiderSpit : MeleeBase
{

    new public MutantSpiderSpitSO itemData;

    public override void Start(Entity entity)
    {
        base.Start(entity);
        itemData = (MutantSpiderSpitSO)base.itemData;
    }

    public override void OnAttackEnd(Entity entity)
    {
        base.OnAttackEnd(entity);

        if (entity.stateMachineVisionToTarget == false)
            return;

        Debug.Log("mutantspiderspit");

        entity.StartCoroutine(ShootRadial(entity, itemData.spell, 90, 3f, 2, 0.4f, 3, 0f, 1));
        //entity.StartCoroutine(Shoot(entity, itemData.spell, 3.5f, 4, 0.3f, 10f, 0.4f));
    }
    /*
    public void CreateProjectile(Entity entity, Vector2 velocityVector)
    {
        GameObject arrow = (GameObject)GameObject.Instantiate(itemData.spell, entity.transform.position + (Vector3)entity.atkPos, entity.transform.rotation, entity.transform);
        arrow.transform.localPosition = entity.atkPos;
        arrow.GetComponent<Projectile>().owner = entity;
        arrow.GetComponent<Projectile>().moveVector = velocityVector;
        arrow.GetComponent<Projectile>().destroyAfterTime = 2f;
        arrow.GetComponent<Projectile>().d_owner = entity.gameObject.GetComponent<Destructable>();
        arrow.GetComponent<Projectile>().weapon = this;
        arrow.transform.SetParent(null);
    }
    */
    public IEnumerator Shoot(Entity entity, GameObject projectile, float speed, int bullets, float startDelay, float randomSpread , float timeBetweenShots)
    {
        yield return new WaitForSeconds(startDelay);

        for (int i = 0; i < bullets; i++)
        {
            Vector2 shootVector = (entity.stateMachineTargetTransform.transform.position - entity.transform.position).normalized;
            float shootAngle = Mathf.Atan2(shootVector.y, shootVector.x) * Mathf.Rad2Deg;

            GameObject obj = (GameObject)GameObject.Instantiate(projectile, entity.transform.position + (Vector3)entity.atkPos, entity.transform.rotation, entity.transform);
            obj.transform.Rotate(Vector3.forward, shootAngle + Random.Range(-randomSpread,randomSpread));
            obj.transform.localPosition = entity.atkPos;
            obj.GetComponent<Projectile>().owner = entity;
            obj.GetComponent<Projectile>().moveVector = obj.transform.right * speed;
            obj.GetComponent<Projectile>().destroyAfterTime = 2f;
            obj.GetComponent<Projectile>().d_owner = entity.gameObject.GetComponent<Destructable>();
            obj.GetComponent<Projectile>().weapon = this;
            obj.transform.SetParent(null);
            yield return new WaitForSeconds(timeBetweenShots);
        }
        yield return null;
    }


    public IEnumerator ShootRadial(Entity entity, GameObject projectile, float angleRange, float speed, int bullets, float startDelay, int repeat, float rotateAmount, int bulletChange)
    {
        Vector2 shootVector = (entity.stateMachineTargetTransform.transform.position - entity.transform.position).normalized;
        float shootAngle = Mathf.Atan2(shootVector.y, shootVector.x) * Mathf.Rad2Deg;
        float additionalRotation = 0;
        int curBullets = bullets;

        for(int r = 0; r < repeat; r++)
        {
            entity.attack = true;
            yield return new WaitForSeconds(startDelay);

            float angleRangeMod = angleRange / curBullets;
            float newAngleRange = angleRange - angleRangeMod;

            float perBulletAngle = newAngleRange / (curBullets - 1);
            float startAngle = newAngleRange * -0.5f;

            for (int i = 0; i < curBullets; i++)
            {
                GameObject obj = (GameObject)GameObject.Instantiate(projectile, entity.transform.position + (Vector3)entity.atkPos, entity.transform.rotation, entity.transform);
                obj.transform.Rotate(Vector3.forward, shootAngle + additionalRotation);

                if(curBullets > 1)
                    obj.transform.Rotate(Vector3.forward, startAngle + i * perBulletAngle);

                obj.transform.localPosition = entity.atkPos;
                obj.GetComponent<Projectile>().owner = entity;
                obj.GetComponent<Projectile>().moveVector = obj.transform.right * speed;
                obj.GetComponent<Projectile>().destroyAfterTime = 2f;
                obj.GetComponent<Projectile>().d_owner = entity.gameObject.GetComponent<Destructable>();
                obj.GetComponent<Projectile>().weapon = this;
                obj.transform.SetParent(null);
            }

            additionalRotation += rotateAmount;
            curBullets += bulletChange;
        }
        yield return null;
    }



}
