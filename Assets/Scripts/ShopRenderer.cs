using System;
using System.Linq;
using UnityEngine;
using TMPro;

public class ShopRenderer : MonoBehaviour
{
    [System.Serializable]
    public struct Item {
        [Range(0.0f, 1f)]
        public float probability;
        public string name;
        public Vector3 scale;
        public GameObject prefab;
        public int cost;
    };

    [System.Serializable]
    public struct ItemSlot {
        public Vector3 position;
    }


    [Header("Definitions")]
    public Item[] items;


    [Header("Item Slots")]
    public ItemSlot[] itemSlots;

    [Header("Rendering")]
    public GameObject itemSlotPrefab;
    // where to instantiate item slots
    public Transform slotDisplayParent;
    public Vector3 itemScale;

    //
    private int[] availableItems;
    private float probabilitySum;

    public void Awake() {
        // has to be done on Instantiation (before Start())
        probabilitySum = 0.0f;
        foreach(var item in items)
            probabilitySum += item.probability;

        availableItems = new int[itemSlots.Length];
    }


    public void ShowItems() {
        foreach(Transform child in slotDisplayParent) {
            Destroy(child);
        }

        int index = 0;
        foreach(var (item,slot) in availableItems.Zip(itemSlots, (a,b) => (a,b))) {
            if (index == -1) {
                // no item
            } else {
                var slotInstance = Instantiate(itemSlotPrefab, slotDisplayParent).transform;
                slotInstance.localPosition = slot.position;
                slotInstance.GetComponent<ShopItemSlot>().SetItem(index, items[item]);
            }
            index++;
        }
    }

    public void AddRandomItems() {
        for (int slot = 0; slot < itemSlots.Length; slot++) {
            availableItems[slot] = -1;

            var random = UnityEngine.Random.Range(0f,probabilitySum);
            for (int item = 0; item < items.Length; item++) {
                Debug.Log("Comparing " + random + " with " + items[item].probability);
                if (random <= items[item].probability) {
                    availableItems[slot] = item;
                    break;
                }
                random -= items[item].probability;
            }
        }

        ShowItems();
    }

}
