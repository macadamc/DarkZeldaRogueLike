using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff : MeleeBase {

    new public StaffSO itemData;

    public override void Start(Entity entity)
    {
        base.Start(entity);
        itemData = (StaffSO)base.itemData;
    }

    public override void OnAttackEnd(Entity entity)
    {
        base.OnAttackEnd(entity);
        if (triggerHoldAtk)
        {
            CreateProjectile(entity, entity.lookDir * 4f);
        }
    }

    public void CreateProjectile(Entity entity, Vector2 velocityVector)
    {
        GameObject arrow = (GameObject)GameObject.Instantiate(itemData.spell, entity.transform.position+(Vector3)entity.atkPos ,entity.transform.rotation,entity.transform);
        arrow.transform.localPosition = entity.atkPos;
        arrow.GetComponent<Projectile>().owner = entity;
        arrow.GetComponent<Projectile>().moveVector = velocityVector;
        arrow.GetComponent<Projectile>().d_owner = entity.gameObject.GetComponent<Destructable>();
        arrow.GetComponent<Projectile>().weapon = this;
    }


}
