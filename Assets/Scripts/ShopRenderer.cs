using System;
using System.Linq;
using UnityEngine;
using TMPro;

public class ShopRenderer : MonoBehaviour
{
    [System.Serializable]
    public struct ItemSlotConfig {
        public Vector3 position;
    }

    [Header("General")]
    public ItemDefinitions itemDefinitions;

    [Header("Item Slots")]
    public int numItemSlots;
    private ItemContainer items;


    [Header("Rendering")]
    public ItemSlotConfig[] slotProperties;
    public GameObject itemSlotPrefab;
    // where to instantiate item slots
    public Transform slotDisplayParent;
    public Vector3 itemScale;


    public void Awake() {
        itemDefinitions = GameObject.Find("Definitions").GetComponent<ItemDefinitions>();

        items = new(numItemSlots);
    }

    public void ShowItems() {
        foreach(Transform child in slotDisplayParent) {
            Destroy(child);
        }

        foreach(var (slot, slotDisplay) in items.slots.Zip(slotProperties, (a,b)=>(a,b))) {
            if (slot.storedItem == null) {
                // no item
            } else {
                Transform slotInstance = Instantiate(itemSlotPrefab, slotDisplayParent).transform;
                slotInstance.localPosition = slotDisplay.position;
                slotInstance.GetComponent<ShopItemSlot>().SetItem(slot.storedItem, slot.count);
            }
        }
    }

    // TODO: extract to dungeon creator
    public void AddRandomItems() {
        foreach (ItemSlot slot in items.slots) {
            slot.storedItem = null;

            // select random definition
            float random = UnityEngine.Random.Range(0f,1f);
            foreach (ItemDefinition def in itemDefinitions.definitions) {
                if (random <= def.shopProbability) {
                    slot.storedItem = def;
                    slot.count = 1;
                    break;
                }
                random -= def.shopProbability;
            }
        }

        ShowItems();
    }

}
