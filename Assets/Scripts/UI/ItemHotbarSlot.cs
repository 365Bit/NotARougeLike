using UnityEngine;
using UnityEngine.UI;
using System;

using TMPro;

public class ItemHotbarSlot : MonoBehaviour
{
    public Color backgroundColor;
    public Color backgroundColorSelected;

    public GameObject itemText;
    public GameObject itemCount;

    private GameObject itemModel = null;

    public void SetItem(ItemDefinition item, int count) {
        if (itemModel != null) Destroy(itemModel);

        if (count > 0 && item != null) {
            if (item.itemModelUI != null)
                itemModel = Instantiate(item.itemModelUI, transform);
            else 
                itemModel = null;
            itemText.GetComponent<TMP_Text>().text = item.displayName;
            itemCount.GetComponent<TMP_Text>().text = count.ToString();
        } else {
            itemModel = null;
            itemText.GetComponent<TMP_Text>().text = "";
            itemCount.GetComponent<TMP_Text>().text = "";
        }
    }

    public void SetCurrency(Currency currency, int count) {
        itemModel = null;
        itemText.GetComponent<TMP_Text>().text = Enum.GetName(typeof(Currency), currency);
        if (count > 0) {
            itemCount.GetComponent<TMP_Text>().text = count.ToString();
        } else {
            itemCount.GetComponent<TMP_Text>().text = "";
        }
    }

    public void SetSelected(bool selected) {
        GetComponent<Image>().color = selected ? backgroundColorSelected : backgroundColor;
    }
}
