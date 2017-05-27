using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class ItemPickup : BasePickup
{
    static Assembly asm;

    public ItemSO itemData;
    bool inTrigger;
    [System.NonSerialized]
    public int activeItemSlot;    

    public override void Awake()
    {
        base.Awake();
        if (asm == null)
        {
            asm = Assembly.Load("Assembly-CSharp");
        }
    }

    void Update()
    {
        if (inTrigger)
        {
            GameManager.GM.itemPanel.SetPanel(transform.position, itemData.itemDescription);
        }
    }

    public override void Interact(Entity owner)
    {
        Item item = CreateItem(itemData.name, owner);// create the runtime version of the item.

        if (item is ActiveItem)
        {
            owner.weapons[activeItemSlot].OnUnequip(owner);

            Object prefab = owner.weapons[activeItemSlot].itemData.onGroundPrefab;
            InGameObjectManager.CreatePickup(prefab, transform.position, 30, 15);
            owner.Equip((ActiveItem)item, activeItemSlot);
        }

        else if (item is PassiveItem)
        {
            owner.inventory.Add((PassiveItem)item);
            item.OnEquip(owner);
        }

        OnTriggerExit2D(owner.GetComponent<Collider2D>());
        Destroy(gameObject);
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        if (other.CompareTag("Player"))
        {
            GameManager.GM.itemPanel.SetPanel(transform.position, itemData.itemDescription);
            inTrigger = true;
        }
    }

    public override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
        if (other.CompareTag("Player"))
        {
            GameManager.GM.itemPanel.HidePanel();
            inTrigger = false;
        }
    }

    Item CreateItem(string name, Entity owner)
    {
        //create the runtime counterpart of the ScriptableObject held in itemData.
        Item i = (Item)asm.CreateInstance(name);
        i.itemData = itemData;
        i.Start(owner);
        return i;
    }

    
}