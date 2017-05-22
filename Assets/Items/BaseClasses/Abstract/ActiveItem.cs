using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ActiveItem : Item {

    public abstract void OnAttackTriggered(Entity entity);

    public abstract void OnAttackHeld(Entity entity);

    public abstract void OnAttackEnd(Entity entity);

    public abstract void OnHit(Collider2D other, Entity entity, GameObject AttackObject);

    public Quaternion GetQuaternionFromEntityLookDirection(Entity entity)
    {
        float angle = Mathf.Atan2(entity.lookDir.y, entity.lookDir.x) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(angle, Vector3.forward);

    }
}