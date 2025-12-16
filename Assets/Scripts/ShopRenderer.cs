using System;
using System.Linq;
using UnityEngine;
using TMPro;

public class ShopRenderer : MonoBehaviour
{
    public ItemContainer Items;

    [Header("Rendering")]
    private TransformData[] slotTransforms;
    public GameObject itemSlotPrefab;
    // how to instantiate item slots
    struct TransformData {
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Vector3 localScale;
    }
    public Transform slotDisplayParent;

    public void ShowItems() {
        // get transforms from children
        if (slotTransforms == null) {
            slotTransforms = new TransformData[slotDisplayParent.childCount];

            // 
            for (int i = 0; i < slotDisplayParent.childCount; i++) {
                Transform t = slotDisplayParent.GetChild(i);
                slotTransforms[i].localPosition = t.localPosition;
                slotTransforms[i].localRotation = t.localRotation;
                slotTransforms[i].localScale = t.localScale;
            }
        }

        // destroy old slots
        foreach(Transform child in slotDisplayParent) {
            Destroy(child.gameObject);
        }

        // create new slots
        for (int slotIndex = 0; slotIndex < Items.slots.Length && slotIndex < slotTransforms.Length; slotIndex++) {
            ItemSlot slot = Items[slotIndex];
            TransformData slotTransform = slotTransforms[slotIndex];

            if (slot.storedItem == null || slot.count == 0) {
                // no item
            } else {
                Transform slotInstance = Instantiate(itemSlotPrefab, slotDisplayParent).transform;
                slotInstance.localPosition = slotTransform.localPosition;
                slotInstance.localRotation = slotTransform.localRotation;
                slotInstance.localScale = slotTransform.localScale;
                slotInstance.GetComponent<ShopItemSlot>().Init(this, slotIndex);
            }
        }
    }

    public void SetItems(ItemContainer items) {
        this.Items = items;
        ShowItems();
    }

    public void RemoveItem(int slot) {
        if (Items == null) return;

        Items.ConsumeItem(slot);
        ShowItems();
    }
}
