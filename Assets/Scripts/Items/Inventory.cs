using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public int numHotbarSlots;
    public int numItemSlots;
    public ItemContainer container { get; private set;}

    // amount of currencies
    public int numCurrencySlots = Enum.GetValues(typeof(Currency)).Length;
    public Dictionary<Currency, int> currency {get; private set;}

    public void Start() {
        container = new(numItemSlots);
        currency = new();
        foreach(Currency val in Enum.GetValues(typeof(Currency)))
            currency.Add(val, 4);
    }
}

[System.Serializable]
public class ItemSlot {
    public ItemDefinition storedItem = null;
    public int count = 0;
}


[System.Serializable]
public class ItemContainer {
    public ItemSlot[] slots;

    public ItemContainer(int count) {
        slots = new ItemSlot[count];
        for (int i = 0; i < count; i++) 
        {
            slots[i] = new();
        }
    }

    public ItemSlot this[int slot] {
        get => slots[slot];
    }


    private void PrintState() {
        StringBuilder output = new();
        for (int i = 0; i < slots.Length; i++) {
            output.AppendFormat("{0} : {1}({2}x), ", i, (slots[i].storedItem != null) ? slots[i].storedItem.name : "none", slots[i].count);
        }
        Debug.Log("inventory: " + output.ToString());
    }

    public void SwapItems(int first, int second) {
        if (first == second) return;

        var tmp = slots[first];
        slots[first] = slots[second];
        slots[second] = tmp;
    }

    public void ConsumeItem(int slot) {
        if (slots[slot].count > 0)
        {
            slots[slot].count -= 1;
            if(slots[slot].count == 0)
                slots[slot].storedItem = null;
        }
        PrintState();
    }

    public void AddItem(int slot, ItemDefinition item, int count) {
        // there was a different item, overwrite it
        if (slots[slot].storedItem != item)
            slots[slot].count = 0;
        slots[slot].storedItem = item;
        slots[slot].count += count;
        PrintState();
    }

    // Selects an appropriate slot to add item, i.e. either a slot containing same item type or a free one
    public void AddItem(ItemDefinition item, int count) {
        int slot = GetSlotContaining(item);
        if (slot < 0)
            slot = GetFreeSlot();
        if (slot >= 0)
            AddItem(slot, item, count);
    }

    public int GetSlotContaining(ItemDefinition item, int requiredRemainingCapacity = 0) {
        for (int i = 0; i < slots.Length; i++) {
            if (slots[i].storedItem == item && slots[i].count <= item.maxPerInventorySlot - requiredRemainingCapacity) {
                return i;
            }
        }
        return -1;
    }

    public int GetFreeSlot() {
        for (int i = 0; i < slots.Length; i++) {
            if (slots[i].count == 0)
                return i;
        }
        return -1;
    }
}
