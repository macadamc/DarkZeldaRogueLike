using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPanel : MonoBehaviour {

    public RectTransform rectTransform;

    public Text itemNameText;
    public Text itemTypeText;
    public Text itemDescriptionText;

    void Start()
    {
        HidePanel();
    }


    public ItemPanel(Vector2 pos, ItemDescription itemDescription)
    {
        rectTransform.anchoredPosition = pos;
        rectTransform.sizeDelta = itemDescription.itemPanelSize;
        itemNameText.text = itemDescription.itemName;
        itemTypeText.text = itemDescription.itemType.ToString();
        itemDescriptionText.text = itemDescription.itemDescription;
    }

    public void SetPanel(Vector2 pos, ItemDescription itemDescription)
    {
        gameObject.SetActive(true);

        rectTransform.anchoredPosition = pos;
        rectTransform.sizeDelta = itemDescription.itemPanelSize;
        itemNameText.text = itemDescription.itemName;
        itemTypeText.text = itemDescription.itemType.ToString();
        itemDescriptionText.text = itemDescription.itemDescription;
    }
    public void HidePanel()
    {
        gameObject.SetActive(false);
    }



}
