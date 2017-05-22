using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonBow : Bow {

    new PoisonBowSO itemData;

    public override void Start(Entity entity)
    {
        base.Start(entity);
        itemData = (PoisonBowSO)base.itemData;
    }

    public override void OnHit(Collider2D other, Entity entity, GameObject AttackObject)
    {
        Entity o = other.GetComponent<Entity>();
        WeaponAttack attack = AttackObject.GetComponent<WeaponAttack>();

        if (o != null && o != entity)
        {
            new Poison(itemData.PoisonDamage, itemData.PoisonDuration, o);
        }

        base.OnHit(other, entity, AttackObject);
    }
}
