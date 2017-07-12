using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomExplode : MeleeBase
{

    new public MushroomExplodeSO itemData;

    public override void Start(Entity entity)
    {
        base.Start(entity);
        itemData = (MushroomExplodeSO)base.itemData;
    }

    public override void OnAttackEnd(Entity entity)
    {
        base.OnAttackEnd(entity);

        if (entity.stateMachineVisionToTarget == false)
            return;

        Debug.Log("mushroomexplode");

        entity.StartCoroutine(ShootRadial(entity, itemData.spell, 360, 2.5f, 4, 0.3f, 3, 30));
        //entity.StartCoroutine(Shoot(entity, itemData.spell, 3.5f, 4, 0.3f, 10f, 0.4f));
        entity.attack = true;
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


    public IEnumerator ShootRadial(Entity entity, GameObject projectile, float angleRange, float speed, int bullets, float startDelay, int repeat, float rotateAmount)
    {
        Vector2 shootVector = (entity.stateMachineTargetTransform.transform.position - entity.transform.position).normalized;
        float shootAngle = Mathf.Atan2(shootVector.y, shootVector.x) * Mathf.Rad2Deg;

        float angleRangeMod = angleRange / bullets;
        float newAngleRange = angleRange - angleRangeMod;

        float perBulletAngle = newAngleRange / (bullets - 1);
        float startAngle = newAngleRange * -0.5f;

        float additionalRotation = 0;

        yield return new WaitForSeconds(startDelay);

        for(int r = 0; r < repeat; r++)
        {
            for (int i = 0; i < bullets; i++)
            {
                GameObject obj = (GameObject)GameObject.Instantiate(projectile, entity.transform.position + (Vector3)entity.atkPos, entity.transform.rotation, entity.transform);
                obj.transform.Rotate(Vector3.forward, shootAngle + additionalRotation);
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
            yield return new WaitForSeconds(startDelay);
        }
        yield return null;
    }



}
