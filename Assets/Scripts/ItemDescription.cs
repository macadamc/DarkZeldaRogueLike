using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemDescription {

    public string itemName;

    public enum ItemType { Active,Passive };
    public ItemType itemType;

    [TextArea(3,5)]
    public string itemDescription;

    public Vector2 itemPanelSize;

}
