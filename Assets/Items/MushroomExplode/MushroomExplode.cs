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

        entity.StopAllCoroutines();
        entity.StartCoroutine(ShootRadial(entity, itemData.spell, 4f, 8, 0.2f));
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
    public IEnumerator Shoot(Entity entity, GameObject projectile, float speed, int bullets, float startDelay, float timeBetweenShots)
    {
        yield return new WaitForSeconds(startDelay);

        for (int i = 0; i < bullets; i++)
        {
            Vector2 shootVector = (entity.stateMachineTargetTransform.transform.position - entity.transform.position).normalized;

            GameObject obj = (GameObject)GameObject.Instantiate(projectile, entity.transform.position + (Vector3)entity.atkPos, entity.transform.rotation, entity.transform);
            obj.transform.localPosition = entity.atkPos;
            obj.GetComponent<Projectile>().owner = entity;
            obj.GetComponent<Projectile>().moveVector = shootVector * speed;
            obj.GetComponent<Projectile>().destroyAfterTime = 2f;
            obj.GetComponent<Projectile>().d_owner = entity.gameObject.GetComponent<Destructable>();
            obj.GetComponent<Projectile>().weapon = this;
            obj.transform.SetParent(null);
            yield return new WaitForSeconds(timeBetweenShots);
        }
        yield return null;
    }


    public IEnumerator ShootRadial(Entity entity, GameObject projectile, float speed, int bullets, float startDelay)
    {
        Vector2 shootVector = (entity.stateMachineTargetTransform.transform.position - entity.transform.position).normalized;
        //Vector2 dir = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right;

        yield return new WaitForSeconds(startDelay);
        /*
        float degree = angleRange / bullets;
        for (float i = -angleRange / 2f; i < angleRange / 2f; i += degree)
        */
        float degree = 360 / bullets;
        for (float i = -360 / 2f; i < 360 / 2f; i += degree)
        {
            Quaternion rotation = Quaternion.AngleAxis(i, entity.transform.forward);
            //Vector2 dir = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right;
            GameObject obj = (GameObject)GameObject.Instantiate(projectile, entity.transform.position + (Vector3)entity.atkPos, entity.transform.rotation * rotation, entity.transform);
            obj.transform.localPosition = entity.atkPos;
            obj.GetComponent<Projectile>().owner = entity;
            obj.GetComponent<Projectile>().moveVector = obj.transform.right * speed;
            obj.GetComponent<Projectile>().destroyAfterTime = 2f;
            obj.GetComponent<Projectile>().d_owner = entity.gameObject.GetComponent<Destructable>();
            obj.GetComponent<Projectile>().weapon = this;
            obj.transform.SetParent(null);
        }
        yield return null;
    }



}
