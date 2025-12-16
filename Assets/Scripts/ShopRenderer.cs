using System;
using System.Linq;
using UnityEngine;
using TMPro;

public class ShopRenderer : MonoBehaviour
{
    private ItemContainer items;

    [Header("Rendering")]
    private Transform[] slotTransforms;
    public GameObject itemSlotPrefab;
    // where to instantiate item slots
    public Transform slotDisplayParent;

    public void ShowItems() {
        // get transforms from children
        if (slotTransforms == null) {
            slotTransforms = new Transform[slotDisplayParent.childCount];

            // 
            for (int i = 0; i < slotDisplayParent.childCount; i++) {
                slotTransforms[i] = slotDisplayParent.GetChild(i);
            }
        }

        // destroy old slots
        foreach(Transform child in slotDisplayParent) {
            Destroy(child.gameObject);
        }

        // create new slots
        foreach(var (slot, slotTransform) in items.slots.Zip(slotTransforms, (a,b)=>(a,b))) {
            if (slot.storedItem == null || slot.count == 0) {
                // no item
            } else {
                Transform slotInstance = Instantiate(itemSlotPrefab, slotDisplayParent).transform;
                slotInstance.localPosition = slotTransform.localPosition;
                slotInstance.localRotation = slotTransform.localRotation;
                slotInstance.localScale = slotTransform.localScale;
                slotInstance.GetComponent<ShopItemSlot>().SetItem(slot.storedItem, slot.count);
            }
        }
    }

    public void SetItems(ItemContainer items) {
        this.items = items;
        if (slotDisplayParent.childCount < items.slots.Length)
            this.items.Resize(slotDisplayParent.childCount);
        ShowItems();
    }

    public void RemoveItem(int slot) {
        if (items == null) return;

        items.ConsumeItem(slot);
        ShowItems();
    }
}
