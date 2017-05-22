using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Item {
    public ItemSO itemData;

    public abstract void Start(Entity entity);

    public abstract void OnEquip(Entity entity);

    public abstract void OnUnequip(Entity entity);
}
