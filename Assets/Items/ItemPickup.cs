using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class ItemPickup : BasePickup
{

    public static Assembly asm;
    public ItemSO itemData;
    bool inTrigger;
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
        //idk if this is needed but just to make sure that you cant jam the interact button and create more than one Item.
        owner.targetInteractable = null;

        // do item pickup stuff..
        Debug.Log(string.Format("{0} picked up {1}", owner.name, itemData.name));

        Item item = CreateItem(itemData.name, owner);// create the runtime version of the item.
        owner.inventory.Add(item);

        if (item is ActiveItem)
        {
            owner.Equip((ActiveItem)item, activeItemSlot);
        }

        inTrigger = false;
        GameManager.GM.itemPanel.HidePanel();
        Destroy(gameObject);
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<PlayerController>())
            {
                GameManager.GM.itemPanel.SetPanel(transform.position, itemData.itemDescription);
                inTrigger = true;
            }
        }
    }

    public override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<PlayerController>())
            {
                GameManager.GM.itemPanel.HidePanel();
                inTrigger = false;
            }
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